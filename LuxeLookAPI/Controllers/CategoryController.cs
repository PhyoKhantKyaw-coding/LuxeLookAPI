using LuxeLookAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LuxeLookAPI.Controllers;

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

    // POST: api/Category
    [HttpPost("add")]
    public async Task<IActionResult> AddCategory( string request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request))
            return BadRequest(new { message = "Category name is required." });

        try
        {
            var category = await _categoryService.AddCategoryAsync(request);
            return Ok(new
            {
                Status = 200,
                Success = true,
                Data = category
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Status = 500,
                Success = false,
                Message = ex.Message
            });
        }
    }
}
