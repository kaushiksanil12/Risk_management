using ERMS.API.Models.Request;
using ERMS.API.Models.Response;

namespace ERMS.API.Services.Interfaces
{
    public interface IRiskCategoryService
    {
        Task<ApiResponse<List<RiskCategoryResponse>>> SearchAsync(string? search, string? status);
        Task<ApiResponse<RiskCategoryResponse>> GetByIdAsync(int riskCatId);
        Task<ApiResponse<int>> CreateAsync(RiskCategoryRequest request, int createdBy);
        Task<ApiResponse<bool>> UpdateAsync(int riskCatId, RiskCategoryRequest request, int updatedBy);
        Task<ApiResponse<List<RiskCategoryResponse>>> GetDropdownAsync(string mode);
    }
}
