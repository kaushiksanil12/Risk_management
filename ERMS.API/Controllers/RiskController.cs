using ERMS.API.Helpers;
using ERMS.API.Models.Request;
using ERMS.API.Models.Response;
using ERMS.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ERMS.API.Controllers
{
    [ApiController]
    [Route("api/risk")]
    public class RiskController : ControllerBase
    {
        private readonly IRiskService _riskService;
        private readonly IUserPermissionService _permissionService;

        public RiskController(IRiskService riskService, IUserPermissionService permissionService)
        {
            _riskService = riskService;
            _permissionService = permissionService;
        }

        private int GetUserId() => int.TryParse(Request.Headers["X-User-Id"].FirstOrDefault(), out var id) ? id : 0;
        private string GetAdminFlag() => Request.Headers["X-Admin-Flag"].FirstOrDefault() ?? "N";

        private string GetCallerRole(string buId)
        {
            int userId = GetUserId();
            string adminFlag = GetAdminFlag();

            if (adminFlag == "Y") return "ADMIN";

            return _permissionService.GetRoleAsync(userId, buId).Result ?? "N";
        }

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
            var role = GetCallerRole(request.BUId);

            if (role != "O" && role != "ADMIN")
                return StatusCode(403, ApiResponse<int>.Fail(
                    "Access denied. Only Risk Owners can create risks.", 403));

            var userId = GetUserId();
            var result = await _riskService.CreateAsync(request, userId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] RiskRequest request)
        {
            var userId = GetUserId();
            var adminFlag = GetAdminFlag();

            var existing = await _riskService.GetByIdAsync(id);
            if (existing?.Data == null)
                return NotFound(ApiResponse<bool>.NotFound("Risk not found."));

            if (existing.Data.Status != "Draft" && existing.Data.Status != "RevisionRequired")
                return BadRequest(ApiResponse<bool>.Fail(
                    "Risk can only be edited in Draft or RevisionRequired status."));

            if (adminFlag != "Y")
            {
                var role = GetCallerRole(existing.Data.BUId);
                if (role != "O")
                    return StatusCode(403, ApiResponse<bool>.Fail(
                        "Access denied. Only Risk Owners can edit risks."));

                if (existing.Data.CreatedBy != userId)
                    return StatusCode(403, ApiResponse<bool>.Fail(
                        "Access denied. You can only edit risks you created."));
            }

            var result = await _riskService.UpdateAsync(id, request, userId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("submit/{id}")]
        public async Task<IActionResult> Submit(int id)
        {
            var userId = GetUserId();
            var adminFlag = GetAdminFlag();

            var existing = await _riskService.GetByIdAsync(id);
            if (existing?.Data == null)
                return NotFound(ApiResponse<bool>.NotFound("Risk not found."));

            if (existing.Data.Status != "Draft" && existing.Data.Status != "RevisionRequired")
                return BadRequest(ApiResponse<bool>.Fail(
                    "Only Draft or RevisionRequired risks can be submitted."));

            if (adminFlag != "Y")
            {
                var role = GetCallerRole(existing.Data.BUId);
                if (role != "O")
                    return StatusCode(403, ApiResponse<bool>.Fail(
                        "Access denied. Only Risk Owners can submit risks for review."));

                if (existing.Data.CreatedBy != userId)
                    return StatusCode(403, ApiResponse<bool>.Fail(
                        "Access denied. You can only submit risks you created."));
            }

            var result = await _riskService.SubmitAsync(id, userId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("review/{id}")]
        public async Task<IActionResult> Review(int id, [FromBody] RiskReviewRequest request)
        {
            var userId = GetUserId();
            var adminFlag = GetAdminFlag();

            var existing = await _riskService.GetByIdAsync(id);
            if (existing?.Data == null)
                return NotFound(ApiResponse<bool>.NotFound("Risk not found."));

            if (existing.Data.Status != "Submitted" && existing.Data.Status != "UnderReview")
                return BadRequest(ApiResponse<bool>.Fail(
                    "Only Submitted risks can be reviewed."));

            if (adminFlag != "Y")
            {
                var role = GetCallerRole(existing.Data.BUId);
                if (role != "C")
                    return StatusCode(403, ApiResponse<bool>.Fail(
                        "Access denied. Only Risk Champions can review risks."));
            }

            if ((request.Action == "Reject" || request.Action == "SendBack")
                && string.IsNullOrWhiteSpace(request.Remarks))
                return BadRequest(ApiResponse<bool>.Fail(
                    "Remarks are required when rejecting or sending back a risk."));

            var result = await _riskService.ReviewAsync(id, request, userId);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("close/{id}")]
        public async Task<IActionResult> Close(int id)
        {
            var userId = GetUserId();
            var adminFlag = GetAdminFlag();

            var existing = await _riskService.GetByIdAsync(id);
            if (existing?.Data == null)
                return NotFound(ApiResponse<bool>.NotFound("Risk not found."));

            if (adminFlag != "Y")
            {
                var role = GetCallerRole(existing.Data.BUId);
                if (role != "C")
                    return StatusCode(403, ApiResponse<bool>.Fail(
                        "Access denied. Only Risk Champions can close risks."));
            }

            var result = await _riskService.CloseAsync(id, userId);
            return StatusCode(result.StatusCode, result);
        }
    }
}
