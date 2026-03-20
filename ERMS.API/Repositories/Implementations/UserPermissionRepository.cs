using Dapper;
using MySqlConnector;
using System.Data;
using ERMS.API.Models.Request;
using ERMS.API.Models.Response;
using ERMS.API.Repositories.Interfaces;

namespace ERMS.API.Repositories.Implementations
{
    public class UserPermissionRepository : IUserPermissionRepository
    {
        private readonly string _connectionString;

        public UserPermissionRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")!;
        }

        private MySqlConnection CreateConnection() => new MySqlConnection(_connectionString);

        public async Task<IEnumerable<UserPermissionResponse>> GetByUserAsync(int userId)
        {
            using var conn = CreateConnection();
            return await conn.QueryAsync<UserPermissionResponse>(
                "sp_UserPermission_ByUser",
                new { p_UserId = userId },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<int> InsertAsync(UserPermissionRequest request, int createdBy)
        {
            using var conn = CreateConnection();
            var result = await conn.QueryFirstOrDefaultAsync<dynamic>(
                "sp_UserPermission_Insert",
                new
                {
                    p_UserId = request.UserId,
                    p_BUId = request.BUId,
                    p_Role = request.Role,
                    p_CreatedBy = createdBy
                },
                commandType: CommandType.StoredProcedure);
            return (int)(result?.NewId ?? 0);
        }

        public async Task UpdateRoleAsync(int permissionId, string newRole, int changedBy)
        {
            using var conn = CreateConnection();
            await conn.ExecuteAsync(
                "sp_UserPermission_UpdateRole",
                new
                {
                    p_PermissionId = permissionId,
                    p_NewRole = newRole,
                    p_ChangedBy = changedBy
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<int> CheckDuplicateAsync(int userId, string buId)
        {
            using var conn = CreateConnection();
            var result = await conn.QueryFirstOrDefaultAsync<dynamic>(
                "sp_UserPermission_CheckDuplicate",
                new { p_UserId = userId, p_BUId = buId },
                commandType: CommandType.StoredProcedure);
            return (int)(result?.Cnt ?? 0);
        }

        public async Task<string?> GetRoleAsync(int permissionId)
        {
            using var conn = CreateConnection();
            var result = await conn.QueryFirstOrDefaultAsync<dynamic>(
                "sp_UserPermission_GetRole",
                new { p_PermissionId = permissionId },
                commandType: CommandType.StoredProcedure);
            return (string?)(result?.Role);
        }

        public async Task<IEnumerable<DropdownItem>> GetFYDropdownAsync()
        {
            using var conn = CreateConnection();
            var items = await conn.QueryAsync<dynamic>(
                "sp_FY_Dropdown",
                commandType: CommandType.StoredProcedure);
            return items.Select(x => new DropdownItem
            {
                Value = (string)x.FYId,
                Text = (string)x.FYName
            });
        }
    }
}
