using System;
using FinanceTracker.ApiService.Data;
using FinanceTracker.ApiService.Entities;
using FinanceTracker.ApiService.Mapping;
using FinanceTracker.ApiService.Models.Categories;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.ApiService.Endpoints;

public static class CategoryEndpoints
{
    public static RouteGroupBuilder MapCategoryEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/categories");

        group.MapGet("/", async (FinanceDBContext db) =>
        {
            var categories = await db.Categories.ToListAsync();
            return Results.Ok(categories);
        });

        group.MapGet("/{id}", async (int id, FinanceDBContext db) =>
        {
            var category = await db.Categories.FindAsync(id);
            return category is not null ? Results.Ok(category) : Results.NotFound();
        });

        group.MapPost("/", async (NewCategoryDTO newCategory, FinanceDBContext db) =>
        {
            var category = newCategory.ToEntity();
            db.Categories.Add(category);
            await db.SaveChangesAsync();
            return Results.Created($"/categories/{category.Id}", category);
        }).RequireAuthorization();

        group.MapPut("/{id}", async (int id, NewCategoryDTO updatedCategory, FinanceDBContext db) =>
        {
            var category = await db.Categories.FindAsync(id);
            if (category is null) return Results.NotFound();

            category.Name = updatedCategory.Name;
            category.Description = updatedCategory.Description;

            await db.SaveChangesAsync();
            return Results.Ok(category);
        }).RequireAuthorization();

        group.MapDelete("/{id}", async (int id, FinanceDBContext db) =>
        {
            var category = await db.Categories.FindAsync(id);
            if (category is not null)
                db.Categories.Remove(category);
                
            await db.SaveChangesAsync();
            return Results.NoContent();
        }).RequireAuthorization();

        return group;
    }
}
