using ERMS.API.Helpers;
using ERMS.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ERMS.API.Controllers
{
    [ApiController]
    [Route("api/dashboard")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary([FromQuery] int userId, [FromQuery] string adminFlag = "N")
        {
            var result = await _dashboardService.GetSummaryAsync(userId, adminFlag);
            return Ok(result);
        }
    }
}
