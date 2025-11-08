namespace FinanceTracker.ApiService.Models.Categories;

public record class CategoryDTO(
    int Id,
    string Name,
    string? Description
);
