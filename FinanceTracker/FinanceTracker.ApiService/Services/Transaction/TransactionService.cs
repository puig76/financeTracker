using System;
using FinanceTracker.ApiService.Data;
using FinanceTracker.ApiService.Mapping;
using FinanceTracker.ApiService.Models.Transactions;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.ApiService.Services.Transaction;

public class TransactionService(FinanceDBContext context) : ITransactionService
{
    public async Task<TransactionDTO> CreateTransactionAsync(NewTransactionDTO newTransaction, string userId)
    {
        var exists = await context.Categories.FindAsync(newTransaction.CategoryId);
        if (exists == null)
        {
            throw new ArgumentException("Invalid category ID");
        }

        var existingTransaction = await context.Transactions
            .FirstOrDefaultAsync(t => t.Amount == newTransaction.Amount &&
                                      t.Date == newTransaction.Date &&
                                      t.Description == newTransaction.Description &&
                                      t.UserId == Guid.Parse(userId));

        if (existingTransaction != null)
        {
            throw new InvalidOperationException("Duplicate transaction detected");
        }

        var entity = newTransaction.ToEntity(userId);

        context.Transactions.Add(entity);
        await context.SaveChangesAsync();

        return entity.ToDTO();
    }

    public async Task<bool> DeleteTransactionAsync(string id)
    {
        var entity = await context.Transactions.FindAsync(Guid.Parse(id));
        if (entity != null)
        {
            context.Transactions.Remove(entity);
            await context.SaveChangesAsync();

            return true;
        }
        else
        {
            return false;
        }
    }

    public async Task<IEnumerable<TransactionDTO>> GetAllTransactionsAsync(string userId)
    {
        var entities = await context.Transactions
            .Where(t => t.UserId == Guid.Parse(userId))
            .ToListAsync();

        return entities.Select(e => e.ToDTO());
    }

    public async Task<TransactionDTO?> GetTransactionByIdAsync(string id)
    {
        var entity = await context.Transactions.FindAsync(Guid.Parse(id));
        return entity?.ToDTO();
    }

    public async Task<IEnumerable<TransactionDTO>> GetTransactionsByCategoryIdAsync(int categoryId, string userId)
    {
        var entities = await context.Transactions
            .Where(t => t.CategoryId == categoryId && t.UserId == Guid.Parse(userId))
            .ToListAsync();

        return entities.Select(e => e.ToDTO());
    }

    public async Task<bool> UpdateTransactionAsync(string id, UpdateTransactionDTO updatedTransaction)
    {
        var entity = await context.Transactions.FindAsync(Guid.Parse(id));
        if (entity == null)
        {
            return false;
        }

        entity.UpdateFromDTO(updatedTransaction);
        await context.SaveChangesAsync();

        return true;
    }
}
