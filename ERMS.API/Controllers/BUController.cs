using ERMS.API.Helpers;
using ERMS.API.Models.Request;
using ERMS.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ERMS.API.Controllers
{
    [ApiController]
    [Route("api/bu")]
    public class BUController : ControllerBase
    {
        private readonly IBUService _buService;

        public BUController(IBUService buService)
        {
            _buService = buService;
        }

        private int GetUserId() => int.TryParse(Request.Headers[ApiConstants.HeaderUserId].FirstOrDefault(), out var id) ? id : 0;

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string? search, [FromQuery] string? status)
        {
            var result = await _buService.SearchAsync(search, status);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _buService.GetByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] BURequest request)
        {
            var userId = GetUserId();
            var result = await _buService.CreateAsync(request, userId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] BURequest request)
        {
            var userId = GetUserId();
            var result = await _buService.UpdateAsync(id, request, userId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> Dropdown()
        {
            var result = await _buService.GetDropdownAsync();
            return Ok(result);
        }
    }
}
