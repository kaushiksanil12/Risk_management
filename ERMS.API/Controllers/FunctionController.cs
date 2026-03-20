using ERMS.API.Helpers;
using ERMS.API.Models.Request;
using ERMS.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ERMS.API.Controllers
{
    [ApiController]
    [Route("api/function")]
    public class FunctionController : ControllerBase
    {
        private readonly IFunctionService _service;

        public FunctionController(IFunctionService service)
        {
            _service = service;
        }

        private int GetUserId() => int.TryParse(Request.Headers[ApiConstants.HeaderUserId].FirstOrDefault(), out var id) ? id : 0;

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string? search, [FromQuery] string? status)
        {
            var result = await _service.SearchAsync(search, status);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] FunctionRequest request)
        {
            var userId = GetUserId();
            var result = await _service.CreateAsync(request, userId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] FunctionRequest request)
        {
            var userId = GetUserId();
            var result = await _service.UpdateAsync(id, request, userId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> Dropdown()
        {
            var result = await _service.GetDropdownAsync();
            return Ok(result);
        }
    }
}
