using LuxeLookAPI.DTO;
using LuxeLookAPI.Models;
using LuxeLookAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LuxeLookAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly CategoryService _categoryService;
        private readonly DataContext _context;
        public CategoryController(CategoryService categoryService, DataContext dataContext)
        {
            _categoryService = categoryService;
            _context = dataContext;
        }

        // POST: api/Category/add
        [HttpPost("add")]
        public async Task<IActionResult> AddCategory([FromBody] string request)
        {
            if (string.IsNullOrWhiteSpace(request))
                return BadRequest(new { message = "Category name is required." });

            try
            {
                var category = await _categoryService.AddCategoryAsync(request);
                return Ok(new { Status = 200, Success = true, Data = category });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Status = 500, Success = false, Message = ex.Message });
            }
        }

        // POST: api/Category/add-instance
        [HttpPost("add-instance")]
        public async Task<IActionResult> AddCategoryInstance([FromQuery] Guid catId, [FromBody] string instanceName)
        {
            if (string.IsNullOrWhiteSpace(instanceName))
                return BadRequest(new { message = "Instance name is required." });

            try
            {
                var instance = await _categoryService.AddCategoryInstanceAsync(catId, instanceName);
                return Ok(new { Status = 200, Success = true, Data = instance });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Status = 500, Success = false, Message = ex.Message });
            }
        }

        // GET: api/Category/instances/{catId}
        [HttpGet("instances/{catId}")]
        public async Task<IActionResult> GetCategoryInstances(Guid catId)
        {
            try
            {
                var instances = await _categoryService.GetCategoryInstancesByCategoryIdAsync(catId);
                return Ok(new { Status = 200, Success = true, Data = instances });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Status = 500, Success = false, Message = ex.Message });
            }
        }
        [HttpGet("GetAllCategoryInstances")]    
        public async Task<IActionResult> GetAllCategoryInstances()
        {
            try
            {
                var instances = await _categoryService.GetCategoryInstances();
                return Ok(new { Status = 200, Success = true, Data = instances });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Status = 500, Success = false, Message = ex.Message });
            }
        }
        // GET: api/Category/all
        [HttpGet("all")]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                var categories = await _categoryService.GetAllCategoriesAsync();
                return Ok(new { Status = 200, Success = true, Data = categories });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Status = 500, Success = false, Message = ex.Message });
            }
        }
        [HttpGet("GetAllBrands")]
        public async Task<IActionResult> GetAllBrands()
        {
            try
            {
                var brands = await _context.Brands.ToListAsync();
                return Ok(new { Status = 200, Success = true, Data = brands });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Status = 500, Success = false, Message = ex.Message });
            }
        }
        [HttpPost("add-with-instances")]
        public async Task<IActionResult> AddCategoryWithInstances( AddCategoryWithInstancesDto request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.CatName))
                return BadRequest(new { message = "Category name is required." });

            try
            {
                var category = await _categoryService.AddCategoryWithInstancesAsync(request);
                return Ok(new { Status = 200, Success = true, Data = category });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Status = 500, Success = false, Message = ex.Message });
            }
        }

        // ✅ Add Brand
        [HttpPost("add-brand")]
        public async Task<IActionResult> AddBrand( string brandName)
        {
            if (string.IsNullOrWhiteSpace(brandName))
                return BadRequest(new { message = "Brand name is required." });

            try
            {
                var brand = await _categoryService.AddBrandAsync(brandName);
                return Ok(new { Status = 200, Success = true, Data = brand });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Status = 500, Success = false, Message = ex.Message });
            }
        }
    }
}
