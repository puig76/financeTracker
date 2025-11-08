using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.ApiService.Models.Categories;

public record class NewCategoryDTO(
    [Required] string Name,
    string? Description
);
