using System;
using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Web.Models.Transactions;

public class NewTransaction
{
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
    public decimal Amount { get; set; }
    [Required]
    public DateOnly Date { get; set; }
    public string Description { get; set; } = string.Empty;
    [Required]
    public int CategoryId { get; set; }
}
