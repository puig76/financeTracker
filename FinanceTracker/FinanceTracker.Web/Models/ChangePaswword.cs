using System;
using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Web.Models;

public class ChangePaswword
{
    [Required]
    public string CurrentPassword { get; set; } = string.Empty;
    
    [Required]
    public string NewPassword { get; set; } = string.Empty;
    
    [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
    public required string ConfirmPassword { get; set; } = string.Empty;
    
}
