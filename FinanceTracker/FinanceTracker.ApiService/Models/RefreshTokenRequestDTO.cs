namespace FinanceTracker.ApiService.Models;

public record class RefreshTokenRequestDTO(
    Guid UserId,
    string RefreshToken
);
