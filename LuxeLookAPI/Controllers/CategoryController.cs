using LuxeLookAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LuxeLookAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly CategoryService _categoryService;

        public CategoryController(CategoryService categoryService)
        {
            _categoryService = categoryService;
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
    }
}
