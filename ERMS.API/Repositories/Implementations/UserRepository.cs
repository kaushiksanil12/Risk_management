using Dapper;
using MySqlConnector;
using System.Data;
using ERMS.API.Models.Request;
using ERMS.API.Models.Response;
using ERMS.API.Repositories.Interfaces;

namespace ERMS.API.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")!;
        }

        private MySqlConnection CreateConnection() => new MySqlConnection(_connectionString);

        public async Task<IEnumerable<UserResponse>> SearchAsync(string? search, string? status)
        {
            using var conn = CreateConnection();
            return await conn.QueryAsync<UserResponse>(
                "sp_User_Search",
                new { p_Search = search, p_Status = status },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<UserResponse?> GetByIdAsync(int userId)
        {
            using var conn = CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<UserResponse>(
                "sp_User_GetById",
                new { p_UserId = userId },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<int> InsertAsync(UserRequest request, int createdBy)
        {
            using var conn = CreateConnection();
            var result = await conn.QueryFirstOrDefaultAsync<dynamic>(
                "sp_User_Insert",
                new
                {
                    p_Username = request.Username,
                    p_LoginType = request.LoginType,
                    p_Password = request.Password,
                    p_FullName = request.FullName,
                    p_Email = request.Email,
                    p_Mobile = request.Mobile,
                    p_Status = request.Status,
                    p_AdminFlag = request.AdminFlag,
                    p_CreatedBy = createdBy
                },
                commandType: CommandType.StoredProcedure);
            return (int)(result?.NewId ?? 0);
        }

        public async Task UpdateAsync(int userId, UserRequest request, int updatedBy)
        {
            using var conn = CreateConnection();
            await conn.ExecuteAsync(
                "sp_User_Update",
                new
                {
                    p_UserId = userId,
                    p_LoginType = request.LoginType,
                    p_Password = request.Password,
                    p_FullName = request.FullName,
                    p_Email = request.Email,
                    p_Mobile = request.Mobile,
                    p_Status = request.Status,
                    p_AdminFlag = request.AdminFlag,
                    p_UpdatedBy = updatedBy
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<int> CheckUsernameAsync(string username, int? userId)
        {
            using var conn = CreateConnection();
            var result = await conn.QueryFirstOrDefaultAsync<dynamic>(
                "sp_User_CheckUsername",
                new { p_Username = username, p_UserId = userId },
                commandType: CommandType.StoredProcedure);
            return (int)(result?.Cnt ?? 0);
        }

        public async Task UnlockAsync(int userId, int updatedBy)
        {
            using var conn = CreateConnection();
            await conn.ExecuteAsync(
                "sp_User_Unlock",
                new { p_UserId = userId, p_UpdatedBy = updatedBy },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<DropdownItem>> GetDropdownAsync()
        {
            using var conn = CreateConnection();
            var items = await conn.QueryAsync<dynamic>(
                "sp_User_Dropdown",
                commandType: CommandType.StoredProcedure);
            return items.Select(x => new DropdownItem
            {
                Value = x.UserId.ToString(),
                Text = (string)x.DisplayName
            });
        }

        public async Task InsertAuditAsync(int userId, string actionType, int changedBy, string? oldData, string? newData)
        {
            using var conn = CreateConnection();
            await conn.ExecuteAsync(
                "sp_User_Audit_Insert",
                new
                {
                    p_UserId = userId,
                    p_ActionType = actionType,
                    p_ChangedBy = changedBy,
                    p_OldData = oldData,
                    p_NewData = newData
                },
                commandType: CommandType.StoredProcedure);
        }
    }
}
