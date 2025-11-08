namespace FinanceTracker.Web.Models;

public record class TokenResponse(
    string AccessToken,
    string RefreshToken
);
