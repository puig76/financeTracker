using System;
using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Web.Models.Categories;

public class NewCategoryModel
{
    [Required]
    public required string Name { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
}
