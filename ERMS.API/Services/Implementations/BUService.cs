using ERMS.API.Models.Request;
using ERMS.API.Models.Response;
using ERMS.API.Repositories.Interfaces;
using ERMS.API.Services.Interfaces;

namespace ERMS.API.Services.Implementations
{
    public class BUService : IBUService
    {
        private readonly IBURepository _buRepo;

        public BUService(IBURepository buRepo)
        {
            _buRepo = buRepo;
        }

        public async Task<ApiResponse<List<BUResponse>>> SearchAsync(string? search, string? status)
        {
            var result = await _buRepo.SearchAsync(search, status);
            return ApiResponse<List<BUResponse>>.Ok(result.ToList());
        }

        public async Task<ApiResponse<BUResponse>> GetByIdAsync(string buId)
        {
            var bu = await _buRepo.GetByIdAsync(buId);
            if (bu == null) return ApiResponse<BUResponse>.NotFound("Business Unit not found.");
            return ApiResponse<BUResponse>.Ok(bu);
        }

        public async Task<ApiResponse<string>> CreateAsync(BURequest request, int createdBy)
        {
            if (string.IsNullOrWhiteSpace(request.BUName))
                return ApiResponse<string>.Fail("Business Unit name is required.");

            var cnt = await _buRepo.CheckDuplicateAsync(request.BUName, null);
            if (cnt > 0) return ApiResponse<string>.Fail("Business Unit name already exists.");

            var newId = await _buRepo.InsertAsync(request, createdBy);
            await _buRepo.InsertAuditAsync(newId, "INSERT", createdBy, $"Created BU: {request.BUName}");
            return ApiResponse<string>.Ok(newId, "Business Unit created successfully.");
        }

        public async Task<ApiResponse<bool>> UpdateAsync(string buId, BURequest request, int updatedBy)
        {
            if (string.IsNullOrWhiteSpace(request.BUName))
                return ApiResponse<bool>.Fail("Business Unit name is required.");

            var cnt = await _buRepo.CheckDuplicateAsync(request.BUName, buId);
            if (cnt > 0) return ApiResponse<bool>.Fail("Business Unit name already exists.");

            await _buRepo.UpdateAsync(buId, request, updatedBy);
            await _buRepo.InsertAuditAsync(buId, "UPDATE", updatedBy, $"Updated BU: {request.BUName}");
            return ApiResponse<bool>.Ok(true, "Business Unit updated successfully.");
        }

        public async Task<ApiResponse<List<DropdownItem>>> GetDropdownAsync()
        {
            var items = await _buRepo.GetDropdownAsync();
            return ApiResponse<List<DropdownItem>>.Ok(items.ToList());
        }
    }
}
