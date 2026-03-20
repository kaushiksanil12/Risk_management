using Dapper;
using MySqlConnector;
using System.Data;
using ERMS.API.Repositories.Interfaces;

namespace ERMS.API.Repositories.Implementations
{
    public class AuthRepository : IAuthRepository
    {
        private readonly string _connectionString;

        public AuthRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")!;
        }

        private MySqlConnection CreateConnection() => new MySqlConnection(_connectionString);

        public async Task<dynamic?> GetUserForLoginAsync(string username)
        {
            using var conn = CreateConnection();
            return await conn.QueryFirstOrDefaultAsync(
                "sp_GetUserForLogin",
                new { p_Username = username },
                commandType: CommandType.StoredProcedure);
        }

        public async Task UpdateFailedAttemptsAsync(int userId, int attempts)
        {
            using var conn = CreateConnection();
            await conn.ExecuteAsync(
                "sp_UpdateFailedAttempts",
                new { p_UserId = userId, p_Attempts = attempts },
                commandType: CommandType.StoredProcedure);
        }

        public async Task ResetFailedAttemptsAsync(int userId)
        {
            using var conn = CreateConnection();
            await conn.ExecuteAsync(
                "sp_ResetFailedAttempts",
                new { p_UserId = userId },
                commandType: CommandType.StoredProcedure);
        }

        public async Task LockAccountAsync(int userId)
        {
            using var conn = CreateConnection();
            await conn.ExecuteAsync(
                "sp_LockAccount",
                new { p_UserId = userId },
                commandType: CommandType.StoredProcedure);
        }

        public async Task InsertLoginAuditAsync(int? userId, string username, string loginType, string status, string ipAddress, string? failureReason)
        {
            using var conn = CreateConnection();
            await conn.ExecuteAsync(
                "sp_InsertLoginAudit",
                new
                {
                    p_UserId = userId,
                    p_Username = username,
                    p_LoginType = loginType,
                    p_Status = status,
                    p_IPAddress = ipAddress,
                    p_FailureReason = failureReason
                },
                commandType: CommandType.StoredProcedure);
        }
    }
}
