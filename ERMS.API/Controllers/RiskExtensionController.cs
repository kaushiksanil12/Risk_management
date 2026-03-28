using ERMS.API.Models.Request;
using ERMS.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ERMS.API.Controllers
{
    [ApiController]
    [Route("api/risk")]
    public class RiskExtensionController : ControllerBase
    {
        private readonly IRiskExtensionService _service;

        public RiskExtensionController(IRiskExtensionService service)
        {
            _service = service;
        }

        private int GetUserId() =>
            int.TryParse(Request.Headers["X-User-Id"].FirstOrDefault(), out var id) ? id : 0;

        // ── People Overview ─────────────────────────────────────────────────

        /// <summary>Save or update People Overview for a risk (Step 2)</summary>
        [HttpPost("{riskId}/people")]
        public async Task<IActionResult> SavePeople(int riskId,
            [FromBody] RiskPeopleRequest request)
        {
            request.RiskId = riskId;
            var result = await _service.SavePeopleAsync(request, GetUserId());
            return Ok(result);
        }

        /// <summary>Get People Overview for a risk</summary>
        [HttpGet("{riskId}/people")]
        public async Task<IActionResult> GetPeople(int riskId)
        {
            var result = await _service.GetPeopleAsync(riskId);
            return Ok(result);
        }

        // ── Risk Assessment ─────────────────────────────────────────────────

        /// <summary>Save or update Risk Assessment (Step 3)</summary>
        [HttpPost("{riskId}/assessment")]
        public async Task<IActionResult> SaveAssessment(int riskId,
            [FromBody] RiskAssessmentRequest request)
        {
            request.RiskId = riskId;
            var result = await _service.SaveAssessmentAsync(request, GetUserId());
            return Ok(result);
        }

        /// <summary>Get Risk Assessment for a risk</summary>
        [HttpGet("{riskId}/assessment")]
        public async Task<IActionResult> GetAssessment(int riskId)
        {
            var result = await _service.GetAssessmentAsync(riskId);
            return Ok(result);
        }

        // ── Quarter Rating ──────────────────────────────────────────────────

        /// <summary>Save or update a Quarter Rating row (Step 4)</summary>
        [HttpPost("{riskId}/quarterrating")]
        public async Task<IActionResult> SaveQuarterRating(int riskId,
            [FromBody] RiskQuarterRatingRequest request)
        {
            request.RiskId = riskId;
            var result = await _service.SaveQuarterRatingAsync(request, GetUserId());
            return Ok(result);
        }

        /// <summary>Get all Quarter Ratings for a risk</summary>
        [HttpGet("{riskId}/quarterrating")]
        public async Task<IActionResult> GetQuarterRatings(int riskId)
        {
            var result = await _service.GetRatingsAsync(riskId);
            return Ok(result);
        }

        // ── Attachments ─────────────────────────────────────────────────────

        /// <summary>Upload a file attachment for a risk (Step 5)</summary>
        [HttpPost("{riskId}/attachments")]
        public async Task<IActionResult> UploadAttachment(int riskId, IFormFile file)
        {
            var result = await _service.UploadAttachmentAsync(file, riskId, GetUserId());
            return Ok(result);
        }

        /// <summary>Get all attachments for a risk</summary>
        [HttpGet("{riskId}/attachments")]
        public async Task<IActionResult> GetAttachments(int riskId)
        {
            var result = await _service.GetAttachmentsAsync(riskId);
            return Ok(result);
        }

        /// <summary>Delete a specific attachment</summary>
        [HttpDelete("{riskId}/attachments/{attachmentId}")]
        public async Task<IActionResult> DeleteAttachment(int riskId, int attachmentId)
        {
            var result = await _service.DeleteAttachmentAsync(attachmentId, riskId);
            return Ok(result);
        }

        // ── Full Risk ───────────────────────────────────────────────────────

        /// <summary>Get the full risk record with all wizard sections in one call</summary>
        [HttpGet("{riskId}/full")]
        public async Task<IActionResult> GetFull(int riskId)
        {
            var result = await _service.GetFullRiskAsync(riskId);
            return Ok(result);
        }
    }
}
