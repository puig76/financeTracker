using System;

namespace FinanceTracker.Web.Models.Transactions;

public class Transaction
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public DateOnly Date { get; set; }
    public string Description { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
}
