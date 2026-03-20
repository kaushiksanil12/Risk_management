using ERMS.API.Helpers;
using ERMS.API.Models.Request;
using ERMS.API.Models.Response;
using ERMS.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ERMS.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly IUserPermissionService _permService;

        public AuthController(IAuthService authService, IUserService userService, IUserPermissionService permService)
        {
            _authService = authService;
            _userService = userService;
            _permService = permService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var result = await _authService.LoginAsync(request, ip);

            if (!result.Success)
                return StatusCode(result.StatusCode, result);

            // Enrich response with full user details
            var userResult = await _userService.GetByIdAsync(result.Data!.UserId);
            if (userResult.Success && userResult.Data != null)
            {
                result.Data.FullName = userResult.Data.FullName;
                result.Data.AdminFlag = userResult.Data.AdminFlag;
                result.Data.Email = userResult.Data.Email;
            }

            return Ok(result);
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            return Ok(ApiResponse<bool>.Ok(true, "Logged out successfully."));
        }
    }
}
