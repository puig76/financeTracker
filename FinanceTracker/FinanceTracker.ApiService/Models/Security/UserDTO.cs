using System;

namespace FinanceTracker.ApiService.Models;

public class UserDTO
{
    public Guid Id { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
