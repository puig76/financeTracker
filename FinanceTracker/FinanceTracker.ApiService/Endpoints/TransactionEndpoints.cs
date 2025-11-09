
using System.Security.Claims;
using FinanceTracker.ApiService.Models.Transactions;
using FinanceTracker.ApiService.Services;

namespace FinanceTracker.ApiService.Endpoints;

public static class TransactionEndpoints
{
    public static RouteGroupBuilder MapTransactionEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/transactions").WithTags("Transactions");

        group.MapGet("/", async (ClaimsPrincipal user,ITransactionService transactionService) =>
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim is null)
            {
                return Results.Unauthorized();
            }

            var userId = userIdClaim.Value;
            var transactions = await transactionService.GetAllTransactionsAsync(userId);
            return Results.Ok(transactions);
        })
        .WithName("GetAllTransactions")
        .WithSummary("Retrieves all transactions.")
        .WithDescription("Fetches a list of all financial transactions recorded in the system.");

        group.MapGet("/{id}", async (string id, ITransactionService transactionService) =>
        {
            var transaction = await transactionService.GetTransactionByIdAsync(id);
            return transaction is not null ? Results.Ok(transaction) : Results.NotFound();
        })
        .WithName("GetTransactionById")
        .WithSummary("Retrieves a transaction by ID.")
        .WithDescription("Fetches the details of a specific financial transaction using its unique identifier.");

        group.MapPost("/", async (NewTransactionDTO newTransaction, ClaimsPrincipal user, ITransactionService transactionService) =>
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim is null)
            {
                return Results.Unauthorized();
            }

            var userId = userIdClaim.Value;

            var createdTransaction = await transactionService.CreateTransactionAsync(newTransaction, userId);
            return Results.Created($"/transactions/{createdTransaction.Id}", createdTransaction);
        })
        .WithName("CreateTransaction")
        .WithSummary("Creates a new transaction.")
        .WithDescription("Adds a new financial transaction to the system.");

        group.MapGet("/by-category/{categoryId}", async (int categoryId, ClaimsPrincipal user, ITransactionService transactionService) =>
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim is null)
            {
                return Results.Unauthorized();
            }

            var userId = userIdClaim.Value;
            var transactions = await transactionService.GetTransactionsByCategoryIdAsync(categoryId, userId);
            return Results.Ok(transactions);
        })
        .WithName("GetTransactionsByCategoryId")
        .WithSummary("Retrieves transactions by category ID.")
        .WithDescription("Fetches a list of financial transactions that belong to a specific category using the category's unique identifier.");
        
        group.MapPost("/by-date-range", async (DateFilterDTO dateFilter, ClaimsPrincipal user, ITransactionService transactionService) =>
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim is null)
            {
                return Results.Unauthorized();
            }

            var userId = userIdClaim.Value;
            var transactions = await transactionService.GetTransactionsByDateRangeAsync(dateFilter.StartDate, dateFilter.EndDate, userId);
            return Results.Ok(transactions);
        }).WithName("GetTransactionsByDateRange")
        .WithSummary("Retrieves transactions by date range.")
        .WithDescription("Fetches a list of financial transactions that fall within a specified date range.");

        group.MapDelete("/{id}", async (string id, ITransactionService transactionService) =>
        {
            var deleted = await transactionService.DeleteTransactionAsync(id);
            return deleted ? Results.NoContent() : Results.NotFound();
        })
        .WithName("DeleteTransaction")
        .WithSummary("Deletes a transaction.")
        .WithDescription("Removes a specific financial transaction from the system using its unique identifier.");
        
        group.MapPut("/{id}", async (string id, UpdateTransactionDTO updatedTransaction, ITransactionService transactionService) =>
        {
            var updated = await transactionService.UpdateTransactionAsync(id, updatedTransaction);
            return updated ? Results.NoContent() : Results.NotFound();
        })
        .WithName("UpdateTransaction")
        .WithSummary("Updates a transaction.")
        .WithDescription("Modifies the details of an existing financial transaction using its unique identifier.");

        return group;
    }
}
