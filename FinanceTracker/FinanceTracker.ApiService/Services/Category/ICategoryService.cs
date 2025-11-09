using System;
using FinanceTracker.ApiService.Models.Categories;

namespace FinanceTracker.ApiService.Services.Category;

public interface ICategoryService
{
    Task<CategoryDTO> CreateCategoryAsync(NewCategoryDTO newCategory);
    Task<bool> DeleteCategoryAsync(int id);
    Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync();
    Task<CategoryDTO?> GetCategoryByIdAsync(int id);
    Task<bool> UpdateCategoryAsync(int id, NewCategoryDTO updatedCategory);
}
