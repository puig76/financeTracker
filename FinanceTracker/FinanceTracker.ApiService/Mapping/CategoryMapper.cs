using System;
using FinanceTracker.ApiService.Entities;
using FinanceTracker.ApiService.Models.Categories;

namespace FinanceTracker.ApiService.Mapping;

public static class CategoryMapper
{
    public static CategoryDTO ToDTO(this Category categoryEntity)
    {
        if (categoryEntity == null) throw new ArgumentNullException(nameof(categoryEntity));

        return new CategoryDTO(
            categoryEntity.Id,
            categoryEntity.Name,
            categoryEntity.Description
        );
    }

    public static Category ToEntity(this CategoryDTO categoryModel)
    {
        if (categoryModel == null) throw new ArgumentNullException(nameof(categoryModel));

        return new Category()
        {
            Id = categoryModel.Id,
            Name = categoryModel.Name,
            Description = categoryModel.Description
        };
    }

    public static Category ToEntity(this NewCategoryDTO newCategoryModel)
    {
        if (newCategoryModel == null) throw new ArgumentNullException(nameof(newCategoryModel));

        return new Category()
        {
            Name = newCategoryModel.Name,
            Description = newCategoryModel.Description
        };
    }
}
