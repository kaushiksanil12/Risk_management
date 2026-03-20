using ERMS.API.Models.Request;
using ERMS.API.Models.Response;

namespace ERMS.API.Repositories.Interfaces
{
    public interface IFunctionRepository
    {
        Task<IEnumerable<FunctionResponse>> SearchAsync(string? search, string? status);
        Task<FunctionResponse?> GetByIdAsync(int functionId);
        Task<int> InsertAsync(FunctionRequest request, int createdBy);
        Task UpdateAsync(int functionId, FunctionRequest request, int updatedBy);
        Task<int> CheckDuplicateAsync(string functionName, int? functionId);
        Task<IEnumerable<DropdownItem>> GetDropdownAsync();
        Task InsertAuditAsync(int functionId, string actionType, int changedBy, string changeSummary);
    }
}
