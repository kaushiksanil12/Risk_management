using ERMS.API.Models.Request;
using ERMS.API.Models.Response;

namespace ERMS.API.Services.Interfaces
{
    public interface IUserService
    {
        Task<ApiResponse<List<UserResponse>>> SearchAsync(string? search, string? status);
        Task<ApiResponse<UserResponse>> GetByIdAsync(int userId);
        Task<ApiResponse<int>> CreateAsync(UserRequest request, int createdBy);
        Task<ApiResponse<bool>> UpdateAsync(int userId, UserRequest request, int updatedBy);
        Task<ApiResponse<bool>> UnlockAsync(int userId, int updatedBy);
        Task<ApiResponse<List<DropdownItem>>> GetDropdownAsync();
    }
}
