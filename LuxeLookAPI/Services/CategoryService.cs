using LuxeLookAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LuxeLookAPI.Services
{
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

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return category;
        }

        // Add category instance
        public async Task<CategoryInstance> AddCategoryInstanceAsync(Guid catId, string instanceName)
        {
            if (string.IsNullOrWhiteSpace(instanceName))
                throw new ArgumentException("Category instance name cannot be empty");

            var category = await _context.Categories.FirstOrDefaultAsync(c => c.CatId == catId && c.ActiveFlag);
            if (category == null)
                throw new ArgumentException("Category not found or inactive");

            var instance = new CategoryInstance
            {
                CatInstanceId = Guid.NewGuid(),
                CatId = catId,
                CatInstanceName = instanceName
            };

            _context.CategoryInstances.Add(instance);
            await _context.SaveChangesAsync();

            return instance;
        }

        // Get category instances by category id
        public async Task<List<CategoryInstance>> GetCategoryInstancesByCategoryIdAsync(Guid catId)
        {
            return await _context.CategoryInstances
                .Where(ci => ci.CatId == catId)
                .ToListAsync();
        }

        // Get all categories
        public async Task<List<CategoryModel>> GetAllCategoriesAsync()
        {
            return await _context.Categories
                .Where(c => c.ActiveFlag)
                .ToListAsync();
        }
    }
}
