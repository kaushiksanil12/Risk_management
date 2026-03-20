using ERMS.API.Helpers;
using ERMS.API.Models.Request;
using ERMS.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ERMS.API.Controllers
{
    [ApiController]
    [Route("api/permission")]
    public class UserPermissionController : ControllerBase
    {
        private readonly IUserPermissionService _service;

        public UserPermissionController(IUserPermissionService service)
        {
            _service = service;
        }

        private int GetUserId() => int.TryParse(Request.Headers[ApiConstants.HeaderUserId].FirstOrDefault(), out var id) ? id : 0;

        [HttpGet("byuser/{userId}")]
        public async Task<IActionResult> GetByUser(int userId)
        {
            var result = await _service.GetByUserAsync(userId);
            return Ok(result);
        }

        [HttpPost("assign")]
        public async Task<IActionResult> Assign([FromBody] UserPermissionRequest request)
        {
            var userId = GetUserId();
            var result = await _service.AssignAsync(request, userId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("update/{permissionId}")]
        public async Task<IActionResult> UpdateRole(int permissionId, [FromBody] UserPermissionRequest request)
        {
            var userId = GetUserId();
            var result = await _service.UpdateRoleAsync(permissionId, request.Role, userId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("fy/dropdown")]
        public async Task<IActionResult> FYDropdown()
        {
            var result = await _service.GetFYDropdownAsync();
            return Ok(result);
        }
    }
}
