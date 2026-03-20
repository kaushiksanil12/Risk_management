using ERMS.API.Helpers;
using ERMS.API.Models.Request;
using ERMS.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ERMS.API.Controllers
{
    [ApiController]
    [Route("api/risk")]
    public class RiskController : ControllerBase
    {
        private readonly IRiskService _riskService;

        public RiskController(IRiskService riskService)
        {
            _riskService = riskService;
        }

        private int GetUserId() => int.TryParse(Request.Headers[ApiConstants.HeaderUserId].FirstOrDefault(), out var id) ? id : 0;
        private string GetAdminFlag() => Request.Headers[ApiConstants.HeaderAdminFlag].FirstOrDefault() ?? "N";

        [HttpGet("all")]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? buId, [FromQuery] string? fyId,
            [FromQuery] int? riskCatId, [FromQuery] string? status, [FromQuery] string? search)
        {
            var userId = GetUserId();
            var adminFlag = GetAdminFlag();
            var result = await _riskService.SearchAsync(userId, adminFlag, buId, fyId, riskCatId, status, search);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _riskService.GetByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] RiskRequest request)
        {
            var userId = GetUserId();
            var result = await _riskService.CreateAsync(request, userId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] RiskRequest request)
        {
            var userId = GetUserId();
            var result = await _riskService.UpdateAsync(id, request, userId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("submit/{id}")]
        public async Task<IActionResult> Submit(int id)
        {
            var userId = GetUserId();
            var result = await _riskService.SubmitAsync(id, userId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("review/{id}")]
        public async Task<IActionResult> Review(int id, [FromBody] RiskReviewRequest request)
        {
            var userId = GetUserId();
            var result = await _riskService.ReviewAsync(id, request, userId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("close/{id}")]
        public async Task<IActionResult> Close(int id)
        {
            var userId = GetUserId();
            var result = await _riskService.CloseAsync(id, userId);
            return StatusCode(result.StatusCode, result);
        }
    }
}
