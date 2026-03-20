using Dapper;
using MySqlConnector;
using System.Data;
using ERMS.API.Models.Request;
using ERMS.API.Models.Response;
using ERMS.API.Repositories.Interfaces;

namespace ERMS.API.Repositories.Implementations
{
    public class BURepository : IBURepository
    {
        private readonly string _connectionString;

        public BURepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")!;
        }

        private MySqlConnection CreateConnection() => new MySqlConnection(_connectionString);

        public async Task<IEnumerable<BUResponse>> SearchAsync(string? search, string? status)
        {
            using var conn = CreateConnection();
            return await conn.QueryAsync<BUResponse>(
                "sp_BU_Search",
                new { p_Search = search, p_Status = status },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<BUResponse?> GetByIdAsync(string buId)
        {
            using var conn = CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<BUResponse>(
                "sp_BU_GetById",
                new { p_BUId = buId },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<string> InsertAsync(BURequest request, int createdBy)
        {
            using var conn = CreateConnection();
            var result = await conn.QueryFirstOrDefaultAsync<dynamic>(
                "sp_BU_Insert",
                new
                {
                    p_BUName = request.BUName,
                    p_BUShortName = request.BUShortName,
                    p_Status = request.Status,
                    p_CreatedBy = createdBy
                },
                commandType: CommandType.StoredProcedure);
            return (string)(result?.NewBUId ?? "");
        }

        public async Task UpdateAsync(string buId, BURequest request, int updatedBy)
        {
            using var conn = CreateConnection();
            await conn.ExecuteAsync(
                "sp_BU_Update",
                new
                {
                    p_BUId = buId,
                    p_BUName = request.BUName,
                    p_BUShortName = request.BUShortName,
                    p_Status = request.Status,
                    p_UpdatedBy = updatedBy
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<int> CheckDuplicateAsync(string buName, string? buId)
        {
            using var conn = CreateConnection();
            var result = await conn.QueryFirstOrDefaultAsync<dynamic>(
                "sp_BU_CheckDuplicate",
                new { p_BUName = buName, p_BUId = buId },
                commandType: CommandType.StoredProcedure);
            return (int)(result?.Cnt ?? 0);
        }

        public async Task<IEnumerable<DropdownItem>> GetDropdownAsync()
        {
            using var conn = CreateConnection();
            var items = await conn.QueryAsync<dynamic>(
                "sp_BU_Dropdown",
                commandType: CommandType.StoredProcedure);
            return items.Select(x => new DropdownItem
            {
                Value = (string)x.BUId,
                Text = (string)x.BUName
            });
        }

        public async Task InsertAuditAsync(string buId, string actionType, int changedBy, string changeSummary)
        {
            using var conn = CreateConnection();
            await conn.ExecuteAsync(
                "sp_BU_Audit_Insert",
                new
                {
                    p_BUId = buId,
                    p_ActionType = actionType,
                    p_ChangedBy = changedBy,
                    p_ChangeSummary = changeSummary
                },
                commandType: CommandType.StoredProcedure);
        }
    }
}
