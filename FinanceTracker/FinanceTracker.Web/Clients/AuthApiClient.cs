using System;
using FinanceTracker.Web.Models;

namespace FinanceTracker.Web.Clients;

public class AuthApiClient(HttpClient httpClient)
{

    public async Task<string> LoginAsync(string name, string password)
    {
        var response = await httpClient.PostAsJsonAsync("auth/login", new { UserName = name, Password = password });
        response.EnsureSuccessStatusCode();
        var token = await response.Content.ReadFromJsonAsync<TokenResponse>();

        return token.AccessToken ?? throw new InvalidOperationException("Token not received from the server.");
    }

    public async Task RegisterAsync(string name, string password, string email)
    {
        var response = await httpClient.PostAsJsonAsync("auth/register", new { UserName = name, Email = email, Password = password });
        response.EnsureSuccessStatusCode();
    }
}


