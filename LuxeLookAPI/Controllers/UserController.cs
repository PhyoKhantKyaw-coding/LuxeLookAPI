using LuxeLookAPI.DTO;
using LuxeLookAPI.Models;
using LuxeLookAPI.Services;
using LuxeLookAPI.Share;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace LuxeLookAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly UserService _userService;
    private readonly CommonTokenReader _tokenReader;
    public UserController(UserService userService, CommonTokenReader commonTokenReader)
    {
        _userService = userService;
        _tokenReader = commonTokenReader;
    }

    // Register new user
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] AddUserDTO dto)
    {
        try
        {
            var user = await _userService.AddUserAsync(dto);
            return Ok(user);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // Login user
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO dto)
    {
        var response = await _userService.Loginweb(dto);
        if (response == null)
            return Unauthorized(new { message = "Invalid email or password" });

        return Ok(response);
    }

    // Get all users (Admin only)
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userService.GetAllUser();
        return Ok(users);
    }

    // Get user by ID (Admin or the same user)
    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
            return NotFound();

        return Ok(user);
    }
    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromBody] string email, string otp)
    {
        try
        {
            var result = await _userService.VerifyEmail(email, otp);
            return Ok(new { message = result });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    [HttpPost("resend-otp")]
    public async Task<IActionResult> ResendOTP([FromBody] string email)
    {
        try
        {
            var response = await _userService.ResentOTP(email);
            if (response.Status == APIStatus.Error)
                return BadRequest(new { message = response.Message });
            return Ok(new { message = response.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    [Authorize]
    [HttpGet("me")]
    public IActionResult GetMyInfo()
    {
        var (userId, role, userName) = _tokenReader.GetUserFromContext();

        if (userId == null)
            return Unauthorized();

        return Ok(new { userId, role, userName });
    }

}
