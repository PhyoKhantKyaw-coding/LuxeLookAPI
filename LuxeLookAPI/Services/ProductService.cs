using LuxeLookAPI.DTO;
using LuxeLookAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LuxeLookAPI.Services
{
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
        public async Task<GetProductByCatIdDTO> GetProductsByCatIdAsync(Guid catId)
        {
            var products = from p in _context.Products
                           join c in _context.CategoryInstances
                               on p.CatInstanceId equals c.CatInstanceId
                           join b in _context.Brands
                               on p.BrandId equals b.BrandId into pb
                           from b in pb.DefaultIfEmpty()
                           where c.CatId == catId
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
                        .Where(p => p.CatInstanceId == ci.CatInstanceId)
                        .Select(p => new GetProductDTO2
                        {
                            productID = p.ProductId,
                            productName = p.ProductName
                        }).ToList()
                }).ToListAsync();
        }
    }
}
