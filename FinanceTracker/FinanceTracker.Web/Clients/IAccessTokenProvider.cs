using System;
using FinanceTracker.Web.Models;

namespace FinanceTracker.Web.Clients;

public interface IAccessTokenProvider
{
    Task<string?> GetAccessTokenAsync();
    Task<string?> GetRefreshTokenAsync();
    Task SetAccessTokenAsync(TokenResponse token);
    Task RemoveTokensAsync();
}
