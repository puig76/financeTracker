
using FinanceTracker.Web.Models;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace FinanceTracker.Web.Clients;

public class AccessTokenProvider(ProtectedSessionStorage seccureSessionStorage) : IAccessTokenProvider
{
    public async Task<string?> GetAccessTokenAsync()
    {
        try
        {
            var result = await seccureSessionStorage.GetAsync<string>("token");
            return result.Success ? result.Value : null;
        }
        catch 
        {
            return null;
        }
    }

    public async Task<string?> GetRefreshTokenAsync()
    {
        try
        {
            var result = await seccureSessionStorage.GetAsync<string>("refreshtoken");
            return result.Success ? result.Value : null;
        }
        catch 
        {
            return null;
        }
    }

    public async Task SetAccessTokenAsync(TokenResponse token)
    {
        try
        {
            await seccureSessionStorage.SetAsync("token", token.AccessToken);
            await seccureSessionStorage.SetAsync("refreshtoken", token.RefreshToken);
        }
        catch (Exception)
        {
            // ignored
        }
        // notify auth state provider so UI updates

    }

    public async Task RemoveTokensAsync()
    {
        try
        {
            await seccureSessionStorage.DeleteAsync("token");
            await seccureSessionStorage.DeleteAsync("refreshtoken");
        }
        catch
        {
            // ignored
        }

    }
}
