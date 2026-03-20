using ERMS.API.Models.Request;
using ERMS.API.Models.Response;

namespace ERMS.API.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse<UserResponse>> LoginAsync(LoginRequest request, string ipAddress);
    }
}
