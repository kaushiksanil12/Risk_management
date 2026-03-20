using ERMS.API.Helpers;
using ERMS.API.Models.Request;
using ERMS.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ERMS.API.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        private int GetUserId() => int.TryParse(Request.Headers[ApiConstants.HeaderUserId].FirstOrDefault(), out var id) ? id : 0;

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string? search, [FromQuery] string? status)
        {
            var result = await _userService.SearchAsync(search, status);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _userService.GetByIdAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] UserRequest request)
        {
            var userId = GetUserId();
            var result = await _userService.CreateAsync(request, userId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UserRequest request)
        {
            var userId = GetUserId();
            var result = await _userService.UpdateAsync(id, request, userId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("unlock/{id}")]
        public async Task<IActionResult> Unlock(int id)
        {
            var userId = GetUserId();
            var result = await _userService.UnlockAsync(id, userId);
            return Ok(result);
        }

        [HttpGet("dropdown")]
        public async Task<IActionResult> Dropdown()
        {
            var result = await _userService.GetDropdownAsync();
            return Ok(result);
        }
    }
}
