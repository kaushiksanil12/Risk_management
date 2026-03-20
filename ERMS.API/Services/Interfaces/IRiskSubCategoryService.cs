using ERMS.API.Models.Request;
using ERMS.API.Models.Response;

namespace ERMS.API.Services.Interfaces
{
    public interface IRiskSubCategoryService
    {
        Task<ApiResponse<List<RiskSubCategoryResponse>>> SearchAsync(string? search, string? status, int? riskCatId);
        Task<ApiResponse<RiskSubCategoryResponse>> GetByIdAsync(int id);
        Task<ApiResponse<int>> CreateAsync(RiskSubCategoryRequest request, int createdBy);
        Task<ApiResponse<bool>> UpdateAsync(int id, RiskSubCategoryRequest request, int updatedBy);
        Task<ApiResponse<List<RiskSubCategoryResponse>>> GetDropdownAsync(int riskCatId, string mode);
    }
}
