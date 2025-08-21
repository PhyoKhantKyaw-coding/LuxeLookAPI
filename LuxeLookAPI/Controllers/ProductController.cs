using LuxeLookAPI.DTO;
using LuxeLookAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace LuxeLookAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _productService;
        public ProductController(ProductService productService)
        {
            _productService = productService;
        }

        // GET: api/Product
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                var products = await _productService.GetAllProductsAsync();
                if (products == null || !products.Any())
                    return Ok(new ResponseDTO
                    {
                        Status = APIStatus.Successful,
                        Message = Messages.NoData,
                        Data = null
                    });

                return Ok(new ResponseDTO
                {
                    Status = APIStatus.Successful,
                    Message = Messages.Result,
                    Data = products
                });
            }
            catch
            {
                return StatusCode(500, new ResponseDTO
                {
                    Status = APIStatus.SystemError,
                    Message = Messages.ErrorWhileFetchingData,
                    Data = null
                });
            }
        }

        // GET: api/Product/byCategory/{catId}
        [HttpGet("byCategory/{catId}")]
        public async Task<IActionResult> GetProductsByCategory(Guid catId)
        {
            try
            {
                var products = await _productService.GetProductsByCatIdAsync(catId);
                if (products.Products == null || !products.Products.Any())
                    return Ok(new ResponseDTO
                    {
                        Status = APIStatus.Successful,
                        Message = Messages.NoData,
                        Data = null
                    });

                return Ok(new ResponseDTO
                {
                    Status = APIStatus.Successful,
                    Message = Messages.Result,
                    Data = products
                });
            }
            catch
            {
                return StatusCode(500, new ResponseDTO
                {
                    Status = APIStatus.SystemError,
                    Message = Messages.ErrorWhileFetchingData,
                    Data = null
                });
            }
        }

        // GET: api/Product/byCatInstance/{catId}
        [HttpGet("withCatInstance/{catId}")]
        public async Task<IActionResult> GetProductsWithCatInstance(Guid catId)
        {
            try
            {
                var products = await _productService.GetProductsWithCatInstanceAsync(catId);
                if (products == null || !products.Any())
                    return Ok(new ResponseDTO
                    {
                        Status = APIStatus.Successful,
                        Message = Messages.NoData,
                        Data = null
                    });

                return Ok(new ResponseDTO
                {
                    Status = APIStatus.Successful,
                    Message = Messages.Result,
                    Data = products
                });
            }
            catch
            {
                return StatusCode(500, new ResponseDTO
                {
                    Status = APIStatus.SystemError,
                    Message = Messages.ErrorWhileFetchingData,
                    Data = null
                });
            }
        }
    }
}
