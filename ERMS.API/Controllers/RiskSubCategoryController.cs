using ERMS.API.Helpers;
using ERMS.API.Models.Request;
using ERMS.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ERMS.API.Controllers
{
    [ApiController]
    [Route("api/risksubcategory")]
    public class RiskSubCategoryController : ControllerBase
    {
        private readonly IRiskSubCategoryService _service;

        public RiskSubCategoryController(IRiskSubCategoryService service)
        {
            _service = service;
        }

        private int GetUserId() => int.TryParse(Request.Headers[ApiConstants.HeaderUserId].FirstOrDefault(), out var id) ? id : 0;

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string? search, [FromQuery] string? status, [FromQuery] int? riskCatId)
        {
            var result = await _service.SearchAsync(search, status, riskCatId);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] RiskSubCategoryRequest request)
        {
            var userId = GetUserId();
            var result = await _service.CreateAsync(request, userId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] RiskSubCategoryRequest request)
        {
            var userId = GetUserId();
            var result = await _service.UpdateAsync(id, request, userId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> Dropdown([FromQuery] int riskCatId, [FromQuery] string mode = "ACTIVE")
        {
            var result = await _service.GetDropdownAsync(riskCatId, mode);
            return Ok(result);
        }
    }
}
