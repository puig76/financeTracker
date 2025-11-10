using FinanceTracker.ApiService.Data;
using FinanceTracker.ApiService.Models.Categories;
using FinanceTracker.ApiService.Services.Category;
namespace FinanceTracker.ApiService.Endpoints;

public static class CategoryEndpoints
{
    public static RouteGroupBuilder MapCategoryEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/categories").WithTags("Categories");

        group.MapGet("/", async (ICategoryService categoryService) =>
        {
            var categories = await categoryService.GetAllCategoriesAsync();
            return Results.Ok(categories);
        })
        .WithName("GetAllCategories")
        .WithSummary("Gets all categories")
        .WithDescription("Retrieves a list of all categories available in the system.");

        group.MapGet("/{id}", async (int id, ICategoryService categoryService) =>
        {
            var category = await categoryService.GetCategoryByIdAsync(id);
            return category is not null ? Results.Ok(category) : Results.NotFound();
        })
        .WithName("GetCategoryById")
        .WithSummary("Gets a category by ID")
        .WithDescription("Retrieves the details of a specific category using its unique identifier.");

        group.MapPost("/", async (NewCategoryDTO newCategory, ICategoryService categoryService) =>
        {
            var category = await categoryService.CreateCategoryAsync(newCategory);
            return Results.Created($"/categories/{category.Id}", category);
        }).RequireAuthorization()
        .WithName("CreateCategory")
        .WithSummary("Creates a new category")
        .WithDescription("Creates a new category with the provided details.");

        group.MapPut("/{id}", async (int id, NewCategoryDTO updatedCategory, ICategoryService categoryService) =>
        {
            var success = await categoryService.UpdateCategoryAsync(id, updatedCategory);
            return success ? Results.Ok() : Results.NotFound();
        }).RequireAuthorization()
        .WithName("UpdateCategory")
        .WithSummary("Updates an existing category")
        .WithDescription("Updates the details of an existing category identified by its unique identifier.");

        group.MapDelete("/{id}", async (int id, FinanceDBContext db, ICategoryService categoryService) =>
        {
            var success = await categoryService.DeleteCategoryAsync(id);
            return success ? Results.NoContent() : Results.NotFound();
        }).RequireAuthorization()
        .WithName("DeleteCategory")
        .WithSummary("Deletes a category")
        .WithDescription("Removes a specific category from the system using its unique identifier.");

        return group;
    }
}
