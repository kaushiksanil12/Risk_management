using ERMS.API.Models.Request;
using ERMS.API.Models.Response;

namespace ERMS.API.Repositories.Interfaces
{
    public interface IUserPermissionRepository
    {
        Task<IEnumerable<UserPermissionResponse>> GetByUserAsync(int userId);
        Task<int> InsertAsync(UserPermissionRequest request, int createdBy);
        Task UpdateRoleAsync(int permissionId, string newRole, int changedBy);
        Task<int> CheckDuplicateAsync(int userId, string buId);
        Task<string?> GetRoleAsync(int permissionId);
        Task<IEnumerable<DropdownItem>> GetFYDropdownAsync();
    }
}
