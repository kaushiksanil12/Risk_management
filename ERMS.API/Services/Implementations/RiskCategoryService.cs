using ERMS.API.Models.Request;
using ERMS.API.Models.Response;
using ERMS.API.Repositories.Interfaces;
using ERMS.API.Services.Interfaces;

namespace ERMS.API.Services.Implementations
{
    public class RiskCategoryService : IRiskCategoryService
    {
        private readonly IRiskCategoryRepository _repo;

        public RiskCategoryService(IRiskCategoryRepository repo)
        {
            _repo = repo;
        }

        public async Task<ApiResponse<List<RiskCategoryResponse>>> SearchAsync(string? search, string? status)
        {
            var result = await _repo.SearchAsync(search, status);
            return ApiResponse<List<RiskCategoryResponse>>.Ok(result.ToList());
        }

        public async Task<ApiResponse<RiskCategoryResponse>> GetByIdAsync(int riskCatId)
        {
            var item = await _repo.GetByIdAsync(riskCatId);
            if (item == null) return ApiResponse<RiskCategoryResponse>.NotFound("Risk Category not found.");
            return ApiResponse<RiskCategoryResponse>.Ok(item);
        }

        public async Task<ApiResponse<int>> CreateAsync(RiskCategoryRequest request, int createdBy)
        {
            if (string.IsNullOrWhiteSpace(request.RiskCatName))
                return ApiResponse<int>.Fail("Category name is required.");

            var cnt = await _repo.CheckDuplicateAsync(request.RiskCatName, null);
            if (cnt > 0) return ApiResponse<int>.Fail("Category name already exists.");

            var newId = await _repo.InsertAsync(request, createdBy);
            await _repo.InsertAuditAsync(newId, "INSERT", createdBy, $"Created: {request.RiskCatName}");
            return ApiResponse<int>.Ok(newId, "Risk Category created successfully.");
        }

        public async Task<ApiResponse<bool>> UpdateAsync(int riskCatId, RiskCategoryRequest request, int updatedBy)
        {
            if (string.IsNullOrWhiteSpace(request.RiskCatName))
                return ApiResponse<bool>.Fail("Category name is required.");

            var cnt = await _repo.CheckDuplicateAsync(request.RiskCatName, riskCatId);
            if (cnt > 0) return ApiResponse<bool>.Fail("Category name already exists.");

            await _repo.UpdateAsync(riskCatId, request, updatedBy);
            await _repo.InsertAuditAsync(riskCatId, "UPDATE", updatedBy, $"Updated: {request.RiskCatName}");
            return ApiResponse<bool>.Ok(true, "Risk Category updated successfully.");
        }

        public async Task<ApiResponse<List<RiskCategoryResponse>>> GetDropdownAsync(string mode)
        {
            var items = await _repo.GetDropdownAsync(mode);
            return ApiResponse<List<RiskCategoryResponse>>.Ok(items.ToList());
        }
    }
}
