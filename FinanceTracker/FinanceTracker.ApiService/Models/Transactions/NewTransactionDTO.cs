namespace FinanceTracker.ApiService.Models.Transactions;

public record class NewTransactionDTO(
    decimal Amount,
    DateOnly Date,
    string Description,
    int CategoryId
);
