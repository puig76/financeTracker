namespace FinanceTracker.ApiService.Models.Transactions;

public record class DateFilterDTO(
    DateOnly StartDate,
    DateOnly EndDate
);
