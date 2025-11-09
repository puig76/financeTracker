
using FinanceTracker.ApiService.Entities;
using FinanceTracker.ApiService.Models.Transactions;

namespace FinanceTracker.ApiService.Mapping;

public static class TransactionMapper
{
    public static TransactionDTO ToDTO(this Transaction transaction)
    {
        return new TransactionDTO(
            transaction.Id,
            transaction.Amount,
            transaction.Date,
            transaction.Description
        );
    }

    public static Transaction ToEntity(this NewTransactionDTO newTransaction, string userId)
    {
        return new Transaction
        {
            Amount = newTransaction.Amount,
            Date = newTransaction.Date,
            Description = newTransaction.Description,
            CategoryId = newTransaction.CategoryId,
            UserId = Guid.Parse(userId)
        };
    }

    public static void UpdateFromDTO(this Transaction transaction, UpdateTransactionDTO updatedTransaction)
    {
        transaction.Amount = updatedTransaction.Amount;
        transaction.Date = updatedTransaction.Date;
        transaction.Description = updatedTransaction.Description;
        transaction.CategoryId = updatedTransaction.CategoryId;
    }
}
