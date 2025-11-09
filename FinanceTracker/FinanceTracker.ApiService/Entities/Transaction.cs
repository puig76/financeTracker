using System;

namespace FinanceTracker.ApiService.Entities;

public class Transaction
{  
    public Guid Id { get; set; }   
    public decimal Amount { get; set; }   
    public DateOnly Date { get; set; }   
    public string Description { get; set; } = string.Empty;   
    public int CategoryId { get; set; }   
    public Category? Category { get; set; }
    public Guid UserId { get; set; }   
    public User? User { get; set; }
}
