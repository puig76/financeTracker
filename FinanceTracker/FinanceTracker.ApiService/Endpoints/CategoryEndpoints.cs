using FinanceTracker.ApiService.Data;
using FinanceTracker.ApiService.Models.Categories;
using FinanceTracker.ApiService.Services.Category;
namespace FinanceTracker.ApiService.Endpoints;

public static class CategoryEndpoints
{
    public static RouteGroupBuilder MapCategoryEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/categories");

        group.MapGet("/", async (ICategoryService categoryService) =>
        {
            var categories = await categoryService.GetAllCategoriesAsync();
            return Results.Ok(categories);
        });

        group.MapGet("/{id}", async (int id, ICategoryService categoryService) =>
        {
            var category = await categoryService.GetCategoryByIdAsync(id);
            return category is not null ? Results.Ok(category) : Results.NotFound();
        });

        group.MapPost("/", async (NewCategoryDTO newCategory, ICategoryService categoryService) =>
        {
            var category = await categoryService.CreateCategoryAsync(newCategory);
            return Results.Created($"/categories/{category.Id}", category);
        }).RequireAuthorization();

        group.MapPut("/{id}", async (int id, NewCategoryDTO updatedCategory, ICategoryService categoryService) =>
        {
            var success = await categoryService.UpdateCategoryAsync(id, updatedCategory);
            return success ? Results.Ok() : Results.NotFound();
        }).RequireAuthorization();

        group.MapDelete("/{id}", async (int id, FinanceDBContext db, ICategoryService categoryService) =>
        {
            var success = await categoryService.DeleteCategoryAsync(id);
            return success ? Results.NoContent() : Results.NotFound();
        }).RequireAuthorization();

        return group;
    }
}
