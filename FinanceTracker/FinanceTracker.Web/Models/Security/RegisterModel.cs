using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Web.Models;

public class RegisterModel
{
    [Required]
    public required string Username { get; set; }
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string? Email { get; set; } = string.Empty;
    [Required]
    public required string Password { get; set; }
    [Required]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public required string ConfirmPassword { get; set; } 
}