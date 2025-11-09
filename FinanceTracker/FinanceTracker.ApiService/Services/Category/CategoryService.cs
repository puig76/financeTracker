using System;
using FinanceTracker.ApiService.Data;
using FinanceTracker.ApiService.Mapping;
using FinanceTracker.ApiService.Models.Categories;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.ApiService.Services.Category;

public class CategoryService(FinanceDBContext context) : ICategoryService
{
    public async Task<CategoryDTO> CreateCategoryAsync(NewCategoryDTO newCategory)
    {
        var category = newCategory.ToEntity();

        context.Categories.Add(category);
        await context.SaveChangesAsync();

        return category.ToDTO();
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        var category = await context.Categories.FindAsync(id);
        if (category == null) return false;

        context.Categories.Remove(category);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync()
    {
        var categories = await context.Categories.ToListAsync();
        return categories.Select(c => c.ToDTO());
    }

    public async Task<CategoryDTO?> GetCategoryByIdAsync(int id)
    {
        var category = await context.Categories.FindAsync(id);
        return category?.ToDTO();
    }

    public async Task<bool> UpdateCategoryAsync(int id, NewCategoryDTO updatedCategory)
    {
        var category = await context.Categories.FindAsync(id);
        if (category == null) return false;

        category.Name = updatedCategory.Name;
        category.Description = updatedCategory.Description;

        context.Categories.Update(category);
        await context.SaveChangesAsync();
        return true;
    }
}
