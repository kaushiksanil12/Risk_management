using ERMS.API.Models.Request;
using ERMS.API.Models.Response;
using ERMS.API.Repositories.Interfaces;
using ERMS.API.Services.Interfaces;

namespace ERMS.API.Services.Implementations
{
    public class RiskSubCategoryService : IRiskSubCategoryService
    {
        private readonly IRiskSubCategoryRepository _repo;

        public RiskSubCategoryService(IRiskSubCategoryRepository repo)
        {
            _repo = repo;
        }

        public async Task<ApiResponse<List<RiskSubCategoryResponse>>> SearchAsync(string? search, string? status, int? riskCatId)
        {
            var result = await _repo.SearchAsync(search, status, riskCatId);
            return ApiResponse<List<RiskSubCategoryResponse>>.Ok(result.ToList());
        }

        public async Task<ApiResponse<RiskSubCategoryResponse>> GetByIdAsync(int id)
        {
            var item = await _repo.GetByIdAsync(id);
            if (item == null) return ApiResponse<RiskSubCategoryResponse>.NotFound("Risk Sub-Category not found.");
            return ApiResponse<RiskSubCategoryResponse>.Ok(item);
        }

        public async Task<ApiResponse<int>> CreateAsync(RiskSubCategoryRequest request, int createdBy)
        {
            if (string.IsNullOrWhiteSpace(request.RiskSubCatName))
                return ApiResponse<int>.Fail("Sub-category name is required.");

            var cnt = await _repo.CheckDuplicateAsync(request.RiskCatId, request.RiskSubCatName, null);
            if (cnt > 0) return ApiResponse<int>.Fail("Sub-category name already exists for this category.");

            var newId = await _repo.InsertAsync(request, createdBy);
            await _repo.InsertAuditAsync(newId, "INSERT", createdBy, $"Created: {request.RiskSubCatName}");
            return ApiResponse<int>.Ok(newId, "Risk Sub-Category created successfully.");
        }

        public async Task<ApiResponse<bool>> UpdateAsync(int id, RiskSubCategoryRequest request, int updatedBy)
        {
            if (string.IsNullOrWhiteSpace(request.RiskSubCatName))
                return ApiResponse<bool>.Fail("Sub-category name is required.");

            var cnt = await _repo.CheckDuplicateAsync(request.RiskCatId, request.RiskSubCatName, id);
            if (cnt > 0) return ApiResponse<bool>.Fail("Sub-category name already exists for this category.");

            await _repo.UpdateAsync(id, request, updatedBy);
            await _repo.InsertAuditAsync(id, "UPDATE", updatedBy, $"Updated: {request.RiskSubCatName}");
            return ApiResponse<bool>.Ok(true, "Risk Sub-Category updated successfully.");
        }

        public async Task<ApiResponse<List<RiskSubCategoryResponse>>> GetDropdownAsync(int riskCatId, string mode)
        {
            var items = await _repo.GetDropdownAsync(riskCatId, mode);
            return ApiResponse<List<RiskSubCategoryResponse>>.Ok(items.ToList());
        }
    }
}
