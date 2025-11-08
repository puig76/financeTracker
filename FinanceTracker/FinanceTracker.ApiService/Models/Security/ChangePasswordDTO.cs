namespace FinanceTracker.ApiService.Models;

public record class ChangePasswordDTO(
    string CurrentPassword,
    string NewPassword
);
