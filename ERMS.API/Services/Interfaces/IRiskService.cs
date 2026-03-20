using ERMS.API.Models.Request;
using ERMS.API.Models.Response;

namespace ERMS.API.Services.Interfaces
{
    public interface IRiskService
    {
        Task<ApiResponse<List<RiskResponse>>> SearchAsync(int userId, string adminFlag, string? buId, string? fyId, int? riskCatId, string? status, string? search);
        Task<ApiResponse<RiskResponse>> GetByIdAsync(int riskId);
        Task<ApiResponse<int>> CreateAsync(RiskRequest request, int createdBy);
        Task<ApiResponse<bool>> UpdateAsync(int riskId, RiskRequest request, int updatedBy);
        Task<ApiResponse<bool>> SubmitAsync(int riskId, int userId);
        Task<ApiResponse<bool>> ReviewAsync(int riskId, RiskReviewRequest request, int userId);
        Task<ApiResponse<bool>> CloseAsync(int riskId, int userId);
    }
}
