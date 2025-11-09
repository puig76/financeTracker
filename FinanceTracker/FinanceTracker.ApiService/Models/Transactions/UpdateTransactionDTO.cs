namespace FinanceTracker.ApiService.Models.Transactions;

public record class UpdateTransactionDTO(
    decimal Amount,
    string Description,
    DateOnly Date,
    int CategoryId
);

