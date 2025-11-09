using System;
using FinanceTracker.ApiService.Models.Transactions;

namespace FinanceTracker.ApiService.Services;

public interface ITransactionService
{
    Task<TransactionDTO> CreateTransactionAsync(NewTransactionDTO newTransaction, string userId);
    Task<TransactionDTO?> GetTransactionByIdAsync(string id);
    Task<IEnumerable<TransactionDTO>> GetAllTransactionsAsync(string userId);
    Task<IEnumerable<TransactionDTO>> GetTransactionsByCategoryIdAsync(int categoryId, string userId);
    Task<bool> UpdateTransactionAsync(string id, UpdateTransactionDTO updatedTransaction);
    Task<bool> DeleteTransactionAsync(string id);
}
