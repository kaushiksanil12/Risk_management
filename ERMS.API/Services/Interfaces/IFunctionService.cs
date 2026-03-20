using ERMS.API.Models.Request;
using ERMS.API.Models.Response;

namespace ERMS.API.Services.Interfaces
{
    public interface IFunctionService
    {
        Task<ApiResponse<List<FunctionResponse>>> SearchAsync(string? search, string? status);
        Task<ApiResponse<FunctionResponse>> GetByIdAsync(int functionId);
        Task<ApiResponse<int>> CreateAsync(FunctionRequest request, int createdBy);
        Task<ApiResponse<bool>> UpdateAsync(int functionId, FunctionRequest request, int updatedBy);
        Task<ApiResponse<List<DropdownItem>>> GetDropdownAsync();
    }
}
