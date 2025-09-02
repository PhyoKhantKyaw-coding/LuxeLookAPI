using LuxeLookAPI.DTO;
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
        public async Task<List<CategoryInstance>> GetCategoryInstances()
        {
            return await _context.CategoryInstances.ToListAsync();
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
        public async Task<CategoryModel> AddCategoryWithInstancesAsync(AddCategoryWithInstancesDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.CatName))
                throw new ArgumentException("Category name cannot be empty");

            var category = new CategoryModel
            {
                CatId = Guid.NewGuid(),
                CatName = dto.CatName,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                ActiveFlag = true
            };

            _context.Categories.Add(category);

            // add instances
            foreach (var instanceName in dto.InstanceNames)
            {
                if (!string.IsNullOrWhiteSpace(instanceName))
                {
                    var instance = new CategoryInstance
                    {
                        CatInstanceId = Guid.NewGuid(),
                        CatId = category.CatId,
                        CatInstanceName = instanceName
                    };
                    _context.CategoryInstances.Add(instance);
                }
            }

            await _context.SaveChangesAsync();
            return category;
        }
        public async Task<BrandModel> AddBrandAsync(string brandName)
        {
            if (string.IsNullOrWhiteSpace(brandName))
                throw new ArgumentException("Brand name cannot be empty");

            var brand = new BrandModel
            {
                BrandId = Guid.NewGuid(),
                BrandName = brandName,
            };

            _context.Brands.Add(brand);
            await _context.SaveChangesAsync();

            return brand;
        }

    }
}
