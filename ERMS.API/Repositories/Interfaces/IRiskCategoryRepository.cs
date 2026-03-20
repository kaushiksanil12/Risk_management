using ERMS.API.Models.Request;
using ERMS.API.Models.Response;

namespace ERMS.API.Repositories.Interfaces
{
    public interface IRiskCategoryRepository
    {
        Task<IEnumerable<RiskCategoryResponse>> SearchAsync(string? search, string? status);
        Task<RiskCategoryResponse?> GetByIdAsync(int riskCatId);
        Task<int> InsertAsync(RiskCategoryRequest request, int createdBy);
        Task UpdateAsync(int riskCatId, RiskCategoryRequest request, int updatedBy);
        Task<int> CheckDuplicateAsync(string riskCatName, int? riskCatId);
        Task<IEnumerable<RiskCategoryResponse>> GetDropdownAsync(string mode);
        Task InsertAuditAsync(int riskCatId, string actionType, int changedBy, string changeSummary);
    }
}
