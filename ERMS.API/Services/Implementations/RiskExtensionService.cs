using ERMS.API.Models.Request;
using ERMS.API.Models.Response;
using ERMS.API.Repositories.Interfaces;
using ERMS.API.Services.Interfaces;

namespace ERMS.API.Services.Implementations
{
    public class RiskExtensionService : IRiskExtensionService
    {
        private readonly IRiskExtensionRepository _repo;
        private readonly ILogger<RiskExtensionService> _logger;

        public RiskExtensionService(IRiskExtensionRepository repo,
            ILogger<RiskExtensionService> logger)
        {
            _repo   = repo;
            _logger = logger;
        }

        // ── People ──────────────────────────────────────────────────────────

        public async Task<ApiResponse<bool>> SavePeopleAsync(RiskPeopleRequest request, int userId)
        {
            if (request.OwnerId <= 0)
                return ApiResponse<bool>.Fail("Responsibility (Owner) is required.");
            if (request.ChampionId <= 0)
                return ApiResponse<bool>.Fail("Risk Champion is required.");

            await _repo.UpsertPeopleAsync(request, userId);
            return ApiResponse<bool>.Ok(true, "People overview saved.");
        }

        public async Task<ApiResponse<RiskPeopleResponse?>> GetPeopleAsync(int riskId)
        {
            var data = await _repo.GetPeopleByRiskAsync(riskId);
            return ApiResponse<RiskPeopleResponse?>.Ok(data);
        }

        // ── Assessment ──────────────────────────────────────────────────────

        public async Task<ApiResponse<bool>> SaveAssessmentAsync(
            RiskAssessmentRequest request, int userId)
        {
            await _repo.UpsertAssessmentAsync(request, userId);
            return ApiResponse<bool>.Ok(true, "Risk assessment saved.");
        }

        public async Task<ApiResponse<RiskAssessmentResponse?>> GetAssessmentAsync(int riskId)
        {
            var data = await _repo.GetAssessmentByRiskAsync(riskId);
            return ApiResponse<RiskAssessmentResponse?>.Ok(data);
        }

        // ── Quarter Rating ──────────────────────────────────────────────────

        public async Task<ApiResponse<bool>> SaveQuarterRatingAsync(
            RiskQuarterRatingRequest request, int userId)
        {
            if (string.IsNullOrWhiteSpace(request.Quarter))
                return ApiResponse<bool>.Fail("Quarter is required.");
            if (!new[] { "Q1", "Q2", "Q3", "Q4" }.Contains(request.Quarter))
                return ApiResponse<bool>.Fail("Quarter must be Q1, Q2, Q3, or Q4.");

            await _repo.UpsertQuarterRatingAsync(request, userId);
            return ApiResponse<bool>.Ok(true, "Quarter rating saved.");
        }

        public async Task<ApiResponse<IEnumerable<RiskQuarterRatingResponse>>> GetRatingsAsync(int riskId)
        {
            var data = await _repo.GetRatingsByRiskAsync(riskId);
            return ApiResponse<IEnumerable<RiskQuarterRatingResponse>>.Ok(data);
        }

        // ── Attachments ─────────────────────────────────────────────────────

        public async Task<ApiResponse<int>> UploadAttachmentAsync(
            IFormFile file, int riskId, int userId)
        {
            if (file == null || file.Length == 0)
                return ApiResponse<int>.Fail("No file provided.");

            if (file.Length > 10 * 1024 * 1024)
                return ApiResponse<int>.Fail("File size must not exceed 10 MB.");

            var allowed = new[] { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".png", ".jpg", ".jpeg" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowed.Contains(ext))
                return ApiResponse<int>.Fail($"File type '{ext}' is not allowed.");

            var uploadDir = Path.Combine("wwwroot", "uploads", "risks", riskId.ToString());
            Directory.CreateDirectory(uploadDir);

            var uniqueName = $"{Guid.NewGuid()}{ext}";
            var fullPath   = Path.Combine(uploadDir, uniqueName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
                await file.CopyToAsync(stream);

            var request = new RiskAttachmentRequest
            {
                RiskId   = riskId,
                FileName = file.FileName,
                FilePath = $"/uploads/risks/{riskId}/{uniqueName}",
                FileSize = (int)file.Length,
                FileType = file.ContentType
            };

            var newId = await _repo.InsertAttachmentAsync(request, userId);
            return ApiResponse<int>.Ok(newId, "File uploaded successfully.");
        }

        public async Task<ApiResponse<IEnumerable<RiskAttachmentResponse>>> GetAttachmentsAsync(int riskId)
        {
            var data = await _repo.GetAttachmentsByRiskAsync(riskId);
            return ApiResponse<IEnumerable<RiskAttachmentResponse>>.Ok(data);
        }

        public async Task<ApiResponse<bool>> DeleteAttachmentAsync(int attachmentId, int riskId)
        {
            await _repo.DeleteAttachmentAsync(attachmentId, riskId);
            return ApiResponse<bool>.Ok(true, "Attachment deleted.");
        }

        // ── Full Risk ───────────────────────────────────────────────────────

        public async Task<ApiResponse<RiskFullResponse?>> GetFullRiskAsync(int riskId)
        {
            var data = await _repo.GetFullByRiskAsync(riskId);
            if (data == null)
                return ApiResponse<RiskFullResponse?>.NotFound("Risk not found.");
            return ApiResponse<RiskFullResponse?>.Ok(data);
        }
    }
}
