namespace FinanceTracker.ApiService.Models;

public record class TokenResponseDTO(
    string AccessToken,
    string RefreshToken
);