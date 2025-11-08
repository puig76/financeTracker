using System;
using System.Security.Claims;
using FinanceTracker.Web.Models.Categories;

namespace FinanceTracker.Web.Clients;

public class CategoryApiClient(HttpClient httpClient, IAccessTokenProvider storage, AuthApiClient authApiClient)
{
    public async Task<IQueryable<CategoryModel>> GetCategoriesAsync()
    {
        await AddAuthorization();
        var response = await httpClient.GetAsync("categories");
        response.EnsureSuccessStatusCode();
        var categories = await response.Content.ReadFromJsonAsync<List<CategoryModel>>();

        return categories?.AsQueryable() ?? Enumerable.Empty<CategoryModel>().AsQueryable();
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
