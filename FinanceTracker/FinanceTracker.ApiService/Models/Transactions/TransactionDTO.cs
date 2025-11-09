namespace FinanceTracker.ApiService.Models.Transactions;

public record class TransactionDTO(
    Guid Id,
    decimal Amount,
    DateOnly Date,
    string Description,
    int CategoryId
);
