using ERMS.API.Models.Request;
using ERMS.API.Models.Response;

namespace ERMS.API.Repositories.Interfaces
{
    public interface IBURepository
    {
        Task<IEnumerable<BUResponse>> SearchAsync(string? search, string? status);
        Task<BUResponse?> GetByIdAsync(string buId);
        Task<string> InsertAsync(BURequest request, int createdBy);
        Task UpdateAsync(string buId, BURequest request, int updatedBy);
        Task<int> CheckDuplicateAsync(string buName, string? buId);
        Task<IEnumerable<DropdownItem>> GetDropdownAsync();
        Task InsertAuditAsync(string buId, string actionType, int changedBy, string changeSummary);
    }
}
