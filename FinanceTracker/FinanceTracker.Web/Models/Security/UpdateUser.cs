using System;
using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Web.Models;

public class UpdateUser
{   
    public string? Username { get; set; }
    
    [EmailAddress]
    public string? Email { get; set; }
}
