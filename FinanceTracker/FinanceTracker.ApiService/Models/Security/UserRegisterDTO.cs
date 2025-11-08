using System;

namespace FinanceTracker.ApiService.Models;

public class UserRegisterDTO
{
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;   
}
