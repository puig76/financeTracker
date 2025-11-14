using System.Net.Http.Json;
using System.Security.Claims;
using FinanceTracker.Web.Models.Transactions;

namespace FinanceTracker.Web.Clients;

public class TransactionApiClient(HttpClient httpClient, IAccessTokenProvider storage, AuthApiClient authApiClient)
{
    public async Task<List<Transaction>> GetAllTransactionsAsync()
    {
        await AddAuthorization();
        return await httpClient.GetFromJsonAsync<List<Transaction>>("transactions")
               ?? [];
    }

    public async Task<Transaction?> GetTransactionByIdAsync(string id)
    {
        await AddAuthorization();
        return await httpClient.GetFromJsonAsync<Transaction>($"transactions/{id}");
    }

    public async Task<Transaction?> CreateTransactionAsync(NewTransaction transaction)
    {
        await AddAuthorization();
        var response = await httpClient.PostAsJsonAsync("transactions", transaction);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Transaction>();
    }

    public async Task<bool> DeleteTransactionAsync(string id)
    {
        await AddAuthorization();
        var response = await httpClient.DeleteAsync($"transactions/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateTransactionAsync(string id, NewTransaction transaction)
    {
        await AddAuthorization();
        var response = await httpClient.PutAsJsonAsync($"transactions/{id}", transaction);
        return response.IsSuccessStatusCode;
    }

    public async Task<List<Transaction>?> GetTransactionsByDateRangeAsync(DateOnly startDate, DateOnly endDate)
    {
        await AddAuthorization();
        var response = await httpClient.PostAsJsonAsync($"transactions/by-date-range", new { startDate, endDate });
        return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<List<Transaction>>() : null;
    }
    
    private async Task AddAuthorization()
    {
        var token = await storage.GetAccessTokenAsync();
        if (token is null) return;
        if (TokenValidator.IsTokenExpired(token))
        {
            var refresh = await storage.GetRefreshTokenAsync();
            if (refresh != null)
            {
                var userId = TokenValidator.GetJWT(token).Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
                var newToken = await authApiClient.RefreshTokenAsync(userId, refresh);
                if (newToken != null)
                {
                    await storage.SetAccessTokenAsync(newToken);
                    token = newToken.AccessToken;
                }
            }
        }

        httpClient.DefaultRequestHeaders.Authorization =
            token is not null ? new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token) : null;
    }

}
