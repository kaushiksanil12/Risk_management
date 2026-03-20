using ERMS.API.Models.Request;
using ERMS.API.Models.Response;

namespace ERMS.API.Services.Interfaces
{
    public interface IUserPermissionService
    {
        Task<ApiResponse<List<UserPermissionResponse>>> GetByUserAsync(int userId);
        Task<ApiResponse<bool>> AssignAsync(UserPermissionRequest request, int createdBy);
        Task<ApiResponse<bool>> UpdateRoleAsync(int permissionId, string newRole, int changedBy);
        Task<ApiResponse<List<DropdownItem>>> GetFYDropdownAsync();
    }
}
