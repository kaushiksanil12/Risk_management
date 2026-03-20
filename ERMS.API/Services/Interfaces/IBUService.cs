using ERMS.API.Models.Request;
using ERMS.API.Models.Response;

namespace ERMS.API.Services.Interfaces
{
    public interface IBUService
    {
        Task<ApiResponse<List<BUResponse>>> SearchAsync(string? search, string? status);
        Task<ApiResponse<BUResponse>> GetByIdAsync(string buId);
        Task<ApiResponse<string>> CreateAsync(BURequest request, int createdBy);
        Task<ApiResponse<bool>> UpdateAsync(string buId, BURequest request, int updatedBy);
        Task<ApiResponse<List<DropdownItem>>> GetDropdownAsync();
    }
}
