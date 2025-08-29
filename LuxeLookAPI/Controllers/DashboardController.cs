using LuxeLookAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LuxeLookAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class DashboardController : ControllerBase
    {
        private readonly DashboardService _dashboardService;

        public DashboardController(DashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        // GET: api/Dashboard
        [HttpGet("Dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            try
            {
                var dashboard = await _dashboardService.GetDashboardDataAsync();
                return Ok(new
                {
                    Status = 200,
                    Success = true,
                    Data = dashboard
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
}