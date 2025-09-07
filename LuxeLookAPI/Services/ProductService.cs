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

    // 1. Get all products with pagination + currency + supplier
    public async Task<List<GetProductDTO>> GetAllProductsAsync(int pageNumber, int pageSize, string language)
    {
        var query = from p in _context.Products
                    join c in _context.CategoryInstances
                        on p.CatInstanceId equals c.CatInstanceId into pc
                    from c in pc.DefaultIfEmpty()
                    join b in _context.Brands
                        on p.BrandId equals b.BrandId into pb
                    from b in pb.DefaultIfEmpty()
                    join s in _context.Suppliers
                        on p.SupplierId equals s.SupplierId into ps
                    from s in ps.DefaultIfEmpty()
                    where p.ActiveFlag == true
                    select new
                    {
                        p,
                        CatInstanceName = c != null ? c.CatInstanceName : null,
                        BrandName = b != null ? b.BrandName : null,
                        SupplierName = s != null ? s.SupplierName : null
                    };

        var products = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return products.Select(x =>
        {
            var (convertedCost, symbol) = CurrencyHelper.Convert(language, x.p.Cost ?? 0);
            var (convertedPrice, _) = CurrencyHelper.Convert(language, x.p.Price ?? 0);

            return new GetProductDTO
            {
                ProductId = x.p.ProductId,
                CatInstanceName = x.CatInstanceName,
                BrandName = x.BrandName,
                SupplierName = x.SupplierName,
                ProductName = x.p.ProductName,
                ProductDescription = x.p.ProductDescription,
                StockQTY = x.p.StockQTY,
                Cost = convertedCost,
                Price = convertedPrice,
                CurrencySymbol = symbol,
                ProductImageUrl = x.p.ProductImageUrl
            };
        }).ToList();
    }

    // 2. Get products by Category Id with pagination + currency + supplier
    public async Task<GetProductByCatIdDTO> GetProductsByCatIdAsync(Guid catId, int pageNumber, int pageSize, string language)
    {
        var query = from p in _context.Products
                    join c in _context.CategoryInstances
                        on p.CatInstanceId equals c.CatInstanceId
                    join b in _context.Brands
                        on p.BrandId equals b.BrandId into pb
                    from b in pb.DefaultIfEmpty()
                    join s in _context.Suppliers
                        on p.SupplierId equals s.SupplierId into ps
                    from s in ps.DefaultIfEmpty()
                    where c.CatId == catId && p.ActiveFlag == true
                    select new
                    {
                        p,
                        CatInstanceName = c.CatInstanceName,
                        BrandName = b != null ? b.BrandName : null,
                        SupplierName = s != null ? s.SupplierName : null
                    };

        var products = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new GetProductByCatIdDTO
        {
            CatId = catId,
            Products = products.Select(x =>
            {
                var (convertedCost, symbol) = CurrencyHelper.Convert(language, x.p.Cost ?? 0);
                var (convertedPrice, _) = CurrencyHelper.Convert(language, x.p.Price ?? 0);

                return new GetProductDTO
                {
                    ProductId = x.p.ProductId,
                    CatInstanceName = x.CatInstanceName,
                    BrandName = x.BrandName,
                    SupplierName = x.SupplierName,
                    ProductName = x.p.ProductName,
                    ProductDescription = x.p.ProductDescription,
                    StockQTY = x.p.StockQTY,
                    Cost = convertedCost,
                    Price = convertedPrice,
                    CurrencySymbol = symbol,
                    ProductImageUrl = x.p.ProductImageUrl
                };
            }).ToList()
        };
    }

    // 3. Get products grouped by Category Instance + supplier
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
    public async Task<List<GetProductDTO>> GetProductsWithCatInstance(Guid catId)
    {
        return await _context.Products
            .Where(p => p.ActiveFlag == true &&
                        _context.CategoryInstances.Any(ci => ci.CatInstanceId == p.CatInstanceId && ci.CatId == catId))
            .Select(p => new GetProductDTO
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                ProductDescription = p.ProductDescription,
                StockQTY = p.StockQTY,
                Cost = p.Cost,
                Price = p.Price,
                ProductImageUrl = p.ProductImageUrl,

                // join category instance
                CatInstanceName = _context.CategoryInstances
                    .Where(ci => ci.CatInstanceId == p.CatInstanceId)
                    .Select(ci => ci.CatInstanceName)
                    .FirstOrDefault(),

                // join brand
                BrandName = _context.Brands
                    .Where(b => b.BrandId == p.BrandId)
                    .Select(b => b.BrandName)
                    .FirstOrDefault(),

                // join supplier (if you have Supplier table, update accordingly)
                SupplierName = _context.Users
                    .Where(u => u.UserId == p.SupplierId) // 🔹 adjust this if Supplier is mapped differently
                    .Select(u => u.UserName)
                    .FirstOrDefault(),

                // join currency (if you have Currency table, adjust accordingly)
                CurrencySymbol = "MMK" // or query from currency table if exists
            })
            .ToListAsync();
    }

    // 4. Add Product (with SupplierId)
    public async Task<bool> AddProductAsync(AddProductDTO dto)
    {
        var product = new ProductModel
        {
            ProductId = Guid.NewGuid(),
            CatInstanceId = dto.CatInstanceId,
            BrandId = dto.BrandId,
            SupplierId = dto.SupplierId,  // <--- added
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

    // 5. Update Product (with SupplierId)
    public async Task<bool> UpdateProductAsync(UpdateProductDTO dto)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.ProductId == dto.ProductId && p.ActiveFlag == true);

        if (product == null) return false;

        product.CatInstanceId = dto.CatInstanceId;
        product.BrandId = dto.BrandId;
        product.SupplierId = dto.SupplierId; // <--- added
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

    // 6. Delete Product (soft delete, no change needed)
    public async Task<bool> DeleteProductAsync(Guid productId)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == productId);
        if (product == null) return false;

        product.ActiveFlag = false;
        product.UpdatedAt = DateTime.UtcNow;

        _context.Products.Update(product);
        return await _context.SaveChangesAsync() > 0;
    }

    // 7. Get Product by Id (include SupplierName)
    public async Task<GetProductDTO?> GetProductByIdAsync(Guid productId)
    {
        var product = await (from p in _context.Products
                             join c in _context.CategoryInstances
                                 on p.CatInstanceId equals c.CatInstanceId into pc
                             from c in pc.DefaultIfEmpty()
                             join b in _context.Brands
                                 on p.BrandId equals b.BrandId into pb
                             from b in pb.DefaultIfEmpty()
                             join s in _context.Suppliers
                                 on p.SupplierId equals s.SupplierId into ps
                             from s in ps.DefaultIfEmpty()
                             where p.ProductId == productId && p.ActiveFlag == true
                             select new GetProductDTO
                             {
                                 ProductId = p.ProductId,
                                 CatInstanceName = c != null ? c.CatInstanceName : null,
                                 BrandName = b != null ? b.BrandName : null,
                                 SupplierName = s != null ? s.SupplierName : null,
                                 ProductName = p.ProductName,
                                 ProductDescription = p.ProductDescription,
                                 StockQTY = p.StockQTY,
                                 Cost = p.Cost,
                                 Price = p.Price,
                                 ProductImageUrl = p.ProductImageUrl
                             }).FirstOrDefaultAsync();

        return product;
    }
    // 8. Get Supplier History (Supplier Name + Product Count)
    public async Task<List<SupplierHistoryDTO>> GetSupplierHistoryAsync()
    {
        var query = from s in _context.Suppliers
                    join p in _context.Products
                        on s.SupplierId equals p.SupplierId into sp
                    select new SupplierHistoryDTO
                    {
                        SupplierName = s.SupplierName,
                        ProductCount = sp.Count(p => p.ActiveFlag == true)
                    };

        return await query.ToListAsync();
    }

}
