using ERMS.API.Models.Request;
using ERMS.API.Models.Response;
using ERMS.API.Repositories.Interfaces;
using ERMS.API.Services.Interfaces;

namespace ERMS.API.Services.Implementations
{
    public class UserPermissionService : IUserPermissionService
    {
        private readonly IUserPermissionRepository _repo;

        public UserPermissionService(IUserPermissionRepository repo)
        {
            _repo = repo;
        }

        public async Task<ApiResponse<List<UserPermissionResponse>>> GetByUserAsync(int userId)
        {
            var result = await _repo.GetByUserAsync(userId);
            return ApiResponse<List<UserPermissionResponse>>.Ok(result.ToList());
        }

        public async Task<ApiResponse<bool>> AssignAsync(UserPermissionRequest request, int createdBy)
        {
            if (string.IsNullOrWhiteSpace(request.BUId))
                return ApiResponse<bool>.Fail("Business Unit is required.");

            var cnt = await _repo.CheckDuplicateAsync(request.UserId, request.BUId);
            if (cnt == 0)
            {
                // Insert new
                await _repo.InsertAsync(request, createdBy);
            }
            else
            {
                // Update existing — find permissionId from user permissions
                var perms = await _repo.GetByUserAsync(request.UserId);
                var existing = perms.FirstOrDefault(p => p.BUId == request.BUId);
                if (existing?.PermissionId != null)
                {
                    await _repo.UpdateRoleAsync(existing.PermissionId.Value, request.Role, createdBy);
                }
            }

            return ApiResponse<bool>.Ok(true, "Permission updated successfully.");
        }

        public async Task<ApiResponse<bool>> UpdateRoleAsync(int permissionId, string newRole, int changedBy)
        {
            var oldRole = await _repo.GetRoleAsync(permissionId);
            await _repo.UpdateRoleAsync(permissionId, newRole, changedBy);
            return ApiResponse<bool>.Ok(true, "Role updated successfully.");
        }

        public async Task<string?> GetRoleAsync(int userId, string buId)
        {
            var perms = await _repo.GetByUserAsync(userId);
            return perms.FirstOrDefault(p =>
                string.Equals(p.BUId, buId, StringComparison.OrdinalIgnoreCase))?.Role;
        }

        public async Task<ApiResponse<List<DropdownItem>>> GetFYDropdownAsync()
        {
            var items = await _repo.GetFYDropdownAsync();
            return ApiResponse<List<DropdownItem>>.Ok(items.ToList());
        }
    }
}
