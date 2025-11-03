using System;
using System.Security.Claims;
using FinanceTracker.Web.Models;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace FinanceTracker.Web.Clients;

public class AuthApiClient(HttpClient httpClient, IAccessTokenProvider storage)
{

    public async Task<TokenResponse> LoginAsync(string name, string password)
    {
        var response = await httpClient.PostAsJsonAsync("auth/login", new { UserName = name, Password = password });
        response.EnsureSuccessStatusCode();
        var token = await response.Content.ReadFromJsonAsync<TokenResponse>();
        
        return token ?? throw new InvalidOperationException("Failed to deserialize token response.");
    }

    public async Task RegisterAsync(string name, string password, string email)
    {
        var response = await httpClient.PostAsJsonAsync("auth/register", new { UserName = name, Email = email, Password = password });
        response.EnsureSuccessStatusCode();
    }

    public async Task<TokenResponse> RefreshTokenAsync(string userId, string refreshToken)
    {
        var response = await httpClient.PostAsJsonAsync("auth/refresh-token", new { UserId = userId, RefreshToken = refreshToken });
        response.EnsureSuccessStatusCode();
        var token = await response.Content.ReadFromJsonAsync<TokenResponse>();

        return token ?? throw new InvalidOperationException("Failed to deserialize token response.");
    }

    public async Task<UserInfo> GetCurrentUserAsync()
    {
        await AddAuthorization(); 
        var response = await httpClient.GetAsync("auth/me");
        response.EnsureSuccessStatusCode();
        var userInfo = await response.Content.ReadFromJsonAsync<UserInfo>();

        return userInfo ?? throw new InvalidOperationException("Failed to deserialize user info response.");
    }

    public async Task ChangePasswordAsync(string currentPassword, string newPassword)
    {
        await AddAuthorization();

        var response = await httpClient.PatchAsJsonAsync("auth/change-password", new { CurrentPassword = currentPassword, NewPassword = newPassword });
        response.EnsureSuccessStatusCode();
    }

    public async Task<bool> UpdateUserProfileAsync(UpdateUser updateUser)
    {
        await AddAuthorization();

        var response = await httpClient.PutAsJsonAsync("auth/update-profile", updateUser);
        return response.IsSuccessStatusCode;
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
                var newToken = await RefreshTokenAsync(userId, refresh);
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
