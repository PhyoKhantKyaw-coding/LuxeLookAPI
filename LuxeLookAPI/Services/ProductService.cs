using LuxeLookAPI.DTO;
using LuxeLookAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LuxeLookAPI.Services;

public class ProductService
{
    private readonly DataContext _context;
    public ProductService(DataContext context)
    {
        _context = context;
    }

    // 1. Get all products
    public async Task<List<GetProductDTO>> GetAllProductsAsync()
    {
        var products = from p in _context.Products
                       join c in _context.CategoryInstances
                           on p.CatInstanceId equals c.CatInstanceId into pc
                       from c in pc.DefaultIfEmpty()
                       join b in _context.Brands
                           on p.BrandId equals b.BrandId into pb
                       from b in pb.DefaultIfEmpty()
                       where p.ActiveFlag == true
                       select new GetProductDTO
                       {
                           ProductId = p.ProductId,
                           CatInstanceName = c != null ? c.CatInstanceName : null,
                           BrandName = b != null ? b.BrandName : null,
                           ProductName = p.ProductName,
                           ProductDescription = p.ProductDescription,
                           StockQTY = p.StockQTY,
                           Cost = p.Cost,
                           Price = p.Price,
                           ProductImageUrl = p.ProductImageUrl
                       };

        return await products.ToListAsync();
    }

    // 2. Get products by Category Id
    public async Task<GetProductByCatIdDTO> GetProductsByCatIdAsync(Guid catId)
    {
        var products = from p in _context.Products
                       join c in _context.CategoryInstances
                           on p.CatInstanceId equals c.CatInstanceId
                       join b in _context.Brands
                           on p.BrandId equals b.BrandId into pb
                       from b in pb.DefaultIfEmpty()
                       where c.CatId == catId && p.ActiveFlag == true
                       select new GetProductDTO
                       {
                           ProductId = p.ProductId,
                           CatInstanceName = c.CatInstanceName,
                           BrandName = b != null ? b.BrandName : null,
                           ProductName = p.ProductName,
                           ProductDescription = p.ProductDescription,
                           StockQTY = p.StockQTY,
                           Cost = p.Cost,
                           Price = p.Price,
                           ProductImageUrl = p.ProductImageUrl
                       };

        return new GetProductByCatIdDTO
        {
            CatId = catId,
            Products = await products.ToListAsync()
        };
    }

    // 3. Get products grouped by Category Instance
    public async Task<List<GetProductWithCatInstanceByCatIDDTO>> GetProductsWithCatInstanceAsync(Guid catId)
    {
        return await _context.CategoryInstances
            .Where(ci => ci.CatId == catId)
            .Select(ci => new GetProductWithCatInstanceByCatIDDTO
            {
                CatInstanceId = ci.CatInstanceId,
                CatInstanceName = ci.CatInstanceName,
                Products = _context.Products
                    .Where(p => p.CatInstanceId == ci.CatInstanceId && p.ActiveFlag == true)
                    .Select(p => new GetProductDTO2
                    {
                        productID = p.ProductId,
                        productName = p.ProductName
                    }).ToList()
            }).ToListAsync();
    }

    // 4. Add Product
    public async Task<bool> AddProductAsync(AddProductDTO dto)
    {
        var product = new ProductModel
        {
            ProductId = Guid.NewGuid(),
            CatInstanceId = dto.CatInstanceId,
            BrandId = dto.BrandId,
            ProductName = dto.ProductName,
            ProductDescription = dto.ProductDescription,
            StockQTY = dto.StockQTY,
            Cost = dto.Cost,
            Price = dto.Price,
            ProductImageUrl = dto.ProductImageUrl,
            CreatedAt = DateTime.UtcNow,
            ActiveFlag = true
        };

        _context.Products.Add(product);
        return await _context.SaveChangesAsync() > 0;
    }

    // 5. Update Product
    public async Task<bool> UpdateProductAsync(UpdateProductDTO dto)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.ProductId == dto.ProductId && p.ActiveFlag == true);

        if (product == null) return false;

        product.CatInstanceId = dto.CatInstanceId;
        product.BrandId = dto.BrandId;
        product.ProductName = dto.ProductName;
        product.ProductDescription = dto.ProductDescription;
        product.StockQTY = dto.StockQTY;
        product.Cost = dto.Cost;
        product.Price = dto.Price;
        product.ProductImageUrl = dto.ProductImageUrl;
        product.UpdatedAt = DateTime.UtcNow;

        _context.Products.Update(product);
        return await _context.SaveChangesAsync() > 0;
    }

    // 6. Delete Product (soft delete with ActiveFlag)
    public async Task<bool> DeleteProductAsync(Guid productId)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == productId);
        if (product == null) return false;

        product.ActiveFlag = false;
        product.UpdatedAt = DateTime.UtcNow;

        _context.Products.Update(product);
        return await _context.SaveChangesAsync() > 0;
    }

    // 7. Get Product by Id
    public async Task<GetProductDTO?> GetProductByIdAsync(Guid productId)
    {
        var product = await (from p in _context.Products
                             join c in _context.CategoryInstances
                                 on p.CatInstanceId equals c.CatInstanceId into pc
                             from c in pc.DefaultIfEmpty()
                             join b in _context.Brands
                                 on p.BrandId equals b.BrandId into pb
                             from b in pb.DefaultIfEmpty()
                             where p.ProductId == productId && p.ActiveFlag == true
                             select new GetProductDTO
                             {
                                 ProductId = p.ProductId,
                                 CatInstanceName = c != null ? c.CatInstanceName : null,
                                 BrandName = b != null ? b.BrandName : null,
                                 ProductName = p.ProductName,
                                 ProductDescription = p.ProductDescription,
                                 StockQTY = p.StockQTY,
                                 Cost = p.Cost,
                                 Price = p.Price,
                                 ProductImageUrl = p.ProductImageUrl
                             }).FirstOrDefaultAsync();

        return product;
    }
}
