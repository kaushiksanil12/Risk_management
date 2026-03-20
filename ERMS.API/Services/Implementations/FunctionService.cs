using ERMS.API.Models.Request;
using ERMS.API.Models.Response;
using ERMS.API.Repositories.Interfaces;
using ERMS.API.Services.Interfaces;

namespace ERMS.API.Services.Implementations
{
    public class FunctionService : IFunctionService
    {
        private readonly IFunctionRepository _repo;

        public FunctionService(IFunctionRepository repo)
        {
            _repo = repo;
        }

        public async Task<ApiResponse<List<FunctionResponse>>> SearchAsync(string? search, string? status)
        {
            var result = await _repo.SearchAsync(search, status);
            return ApiResponse<List<FunctionResponse>>.Ok(result.ToList());
        }

        public async Task<ApiResponse<FunctionResponse>> GetByIdAsync(int functionId)
        {
            var item = await _repo.GetByIdAsync(functionId);
            if (item == null) return ApiResponse<FunctionResponse>.NotFound("Function not found.");
            return ApiResponse<FunctionResponse>.Ok(item);
        }

        public async Task<ApiResponse<int>> CreateAsync(FunctionRequest request, int createdBy)
        {
            if (string.IsNullOrWhiteSpace(request.FunctionName))
                return ApiResponse<int>.Fail("Function name is required.");

            var cnt = await _repo.CheckDuplicateAsync(request.FunctionName, null);
            if (cnt > 0) return ApiResponse<int>.Fail("Function name already exists.");

            var newId = await _repo.InsertAsync(request, createdBy);
            await _repo.InsertAuditAsync(newId, "INSERT", createdBy, $"Created: {request.FunctionName}");
            return ApiResponse<int>.Ok(newId, "Function created successfully.");
        }

        public async Task<ApiResponse<bool>> UpdateAsync(int functionId, FunctionRequest request, int updatedBy)
        {
            if (string.IsNullOrWhiteSpace(request.FunctionName))
                return ApiResponse<bool>.Fail("Function name is required.");

            var cnt = await _repo.CheckDuplicateAsync(request.FunctionName, functionId);
            if (cnt > 0) return ApiResponse<bool>.Fail("Function name already exists.");

            await _repo.UpdateAsync(functionId, request, updatedBy);
            await _repo.InsertAuditAsync(functionId, "UPDATE", updatedBy, $"Updated: {request.FunctionName}");
            return ApiResponse<bool>.Ok(true, "Function updated successfully.");
        }

        public async Task<ApiResponse<List<DropdownItem>>> GetDropdownAsync()
        {
            var items = await _repo.GetDropdownAsync();
            return ApiResponse<List<DropdownItem>>.Ok(items.ToList());
        }
    }
}
