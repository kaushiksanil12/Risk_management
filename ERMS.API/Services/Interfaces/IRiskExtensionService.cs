using ERMS.API.Models.Request;
using ERMS.API.Models.Response;

namespace ERMS.API.Services.Interfaces
{
    public interface IRiskExtensionService
    {
        Task<ApiResponse<bool>>                               SavePeopleAsync(RiskPeopleRequest request, int userId);
        Task<ApiResponse<RiskPeopleResponse?>>                GetPeopleAsync(int riskId);

        Task<ApiResponse<bool>>                               SaveAssessmentAsync(RiskAssessmentRequest request, int userId);
        Task<ApiResponse<RiskAssessmentResponse?>>            GetAssessmentAsync(int riskId);

        Task<ApiResponse<bool>>                               SaveQuarterRatingAsync(RiskQuarterRatingRequest request, int userId);
        Task<ApiResponse<IEnumerable<RiskQuarterRatingResponse>>> GetRatingsAsync(int riskId);

        Task<ApiResponse<int>>                                UploadAttachmentAsync(IFormFile file, int riskId, int userId);
        Task<ApiResponse<IEnumerable<RiskAttachmentResponse>>> GetAttachmentsAsync(int riskId);
        Task<ApiResponse<bool>>                               DeleteAttachmentAsync(int attachmentId, int riskId);

        Task<ApiResponse<RiskFullResponse?>>                  GetFullRiskAsync(int riskId);
    }
}
