using LuxeLookAPI.Models;
using System;

namespace LuxeLookAPI.Services;

public class CategoryService
{

    private readonly DataContext _context;

    public CategoryService(DataContext context)
    {
        _context = context;
    }

    // Add new category
    public async Task<CategoryModel> AddCategoryAsync(string catName)
    {
        if (string.IsNullOrWhiteSpace(catName))
            throw new ArgumentException("Category name cannot be empty");

        var category = new CategoryModel
        {
            CatId = Guid.NewGuid(),
            CatName = catName,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ActiveFlag = true
        };

        _context.Categories.Add(category); // tblCategory DbSet
        await _context.SaveChangesAsync();

        return category;
    }
}
