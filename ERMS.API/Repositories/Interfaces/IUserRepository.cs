using ERMS.API.Models.Request;
using ERMS.API.Models.Response;

namespace ERMS.API.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<UserResponse>> SearchAsync(string? search, string? status);
        Task<UserResponse?> GetByIdAsync(int userId);
        Task<int> InsertAsync(UserRequest request, int createdBy);
        Task UpdateAsync(int userId, UserRequest request, int updatedBy);
        Task<int> CheckUsernameAsync(string username, int? userId);
        Task UnlockAsync(int userId, int updatedBy);
        Task<IEnumerable<DropdownItem>> GetDropdownAsync();
        Task InsertAuditAsync(int userId, string actionType, int changedBy, string? oldData, string? newData);
    }
}
