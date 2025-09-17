using LuxeLookAPI.DTO;
using LuxeLookAPI.Services;
using LuxeLookAPI.Share;
using Microsoft.AspNetCore.Mvc;

namespace LuxeLookAPI.Controllers;

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
    [HttpGet("bystatus")]
    public async Task<IActionResult> GetAllProductsbystatus(int pageNumber = 1, int pageSize = 10, string language = "us", string status ="")
    {
        try
        {
            var products = await _productService.GetAllProductsAsync1(pageNumber, pageSize, language, status);

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
    [HttpGet]
    public async Task<IActionResult> GetAllProducts(int pageNumber = 1, int pageSize = 10, string language = "us")
    {
        try
        {
            var products = await _productService.GetAllProductsAsync(pageNumber, pageSize, language);

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
    public async Task<IActionResult> GetProductsByCategory(Guid catId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string language = "us")
    {
        try
        {
            var products = await _productService.GetProductsByCatIdAsync(catId, pageNumber, pageSize, language);

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

    // GET: api/Product/withCatInstance/{catId}
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
    [HttpGet("getproductbyCatInstance/{catId}")]
    public async Task<IActionResult> GetProductsWithCat(Guid catId)
    {
        try
        {
            var products = await _productService.GetProductsWithCatInstance(catId);
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
    // GET: api/Product/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById(Guid id)
    {
        try
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound(new ResponseDTO
                {
                    Status = APIStatus.Successful,
                    Message = Messages.NoData,
                    Data = null
                });

            return Ok(new ResponseDTO
            {
                Status = APIStatus.Successful,
                Message = Messages.Result,
                Data = product
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

    // POST: api/Product
    [HttpPost]
    public async Task<IActionResult> AddProduct([FromBody] AddProductDTO dto)
    {
        try
        {
            var result = await _productService.AddProductAsync(dto);
            if (!result)
                return BadRequest(new ResponseDTO
                {
                    Status = APIStatus.Error,
                    Data = null
                });

            return Ok(new ResponseDTO
            {
                Status = APIStatus.Successful,
                Data = dto
            });
        }
        catch
        {
            return StatusCode(500, new ResponseDTO
            {
                Status = APIStatus.SystemError,
                Data = null
            });
        }
    }

    // PUT: api/Product
    [HttpPut]
    public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductDTO dto)
    {
        try
        {
            var result = await _productService.UpdateProductAsync(dto);
            if (!result)
                return NotFound(new ResponseDTO
                {
                    Status = APIStatus.Successful,
                    Message = Messages.NoData,
                    Data = null
                });

            return Ok(new ResponseDTO
            {
                Status = APIStatus.Successful,
                Message = Messages.AddSucess,
                Data = dto
            });
        }
        catch
        {
            return StatusCode(500, new ResponseDTO
            {
                Status = APIStatus.SystemError,
                Data = null
            });
        }
    }

    // DELETE: api/Product/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        try
        {
            var result = await _productService.DeleteProductAsync(id);
            if (!result)
                return NotFound(new ResponseDTO
                {
                    Status = APIStatus.Successful,
                    Message = Messages.NoData,
                    Data = null
                });

            return Ok(new ResponseDTO
            {
                Status = APIStatus.Successful,
                Data = id
            });
        }
        catch
        {
            return StatusCode(500, new ResponseDTO
            {
                Status = APIStatus.SystemError,
                Data = null
            });
        }
    }

    // POST: api/Product/upload
    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Upload(IFormFile image)
    {
        var uploader = new ImageUploader();
        var imageUrl = await uploader.UploadImageAsync(image);
        return Ok(new { url = imageUrl });
    }
    [HttpGet("Supplierhistory")]
    public async Task<IActionResult> GetSupplierHistory()
    {
        try
        {
            var history = await _productService.GetSupplierHistoryAsync();
            return Ok(new { Status = 200, Success = true, Data = history });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Status = 500, Success = false, Message = ex.Message });
        }
    }
}
