using ERMS.API.Models.Request;
using ERMS.API.Models.Response;

namespace ERMS.API.Repositories.Interfaces
{
    public interface IRiskSubCategoryRepository
    {
        Task<IEnumerable<RiskSubCategoryResponse>> SearchAsync(string? search, string? status, int? riskCatId);
        Task<RiskSubCategoryResponse?> GetByIdAsync(int id);
        Task<int> InsertAsync(RiskSubCategoryRequest request, int createdBy);
        Task UpdateAsync(int id, RiskSubCategoryRequest request, int updatedBy);
        Task<int> CheckDuplicateAsync(int riskCatId, string name, int? id);
        Task<IEnumerable<RiskSubCategoryResponse>> GetDropdownAsync(int riskCatId, string mode);
        Task InsertAuditAsync(int riskSubCatId, string actionType, int changedBy, string changeSummary);
    }
}
