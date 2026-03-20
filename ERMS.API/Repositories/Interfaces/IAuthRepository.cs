using ERMS.API.Models.Response;

namespace ERMS.API.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        Task<dynamic?> GetUserForLoginAsync(string username);
        Task UpdateFailedAttemptsAsync(int userId, int attempts);
        Task ResetFailedAttemptsAsync(int userId);
        Task LockAccountAsync(int userId);
        Task InsertLoginAuditAsync(int? userId, string username, string loginType, string status, string ipAddress, string? failureReason);
    }
}
