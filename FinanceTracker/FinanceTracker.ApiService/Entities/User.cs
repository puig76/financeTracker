using System;

namespace FinanceTracker.ApiService.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string Email { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public String Role { get; set; } = "User";
    public String? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }   
}
