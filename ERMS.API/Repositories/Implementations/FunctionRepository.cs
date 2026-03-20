using Dapper;
using MySqlConnector;
using System.Data;
using ERMS.API.Models.Request;
using ERMS.API.Models.Response;
using ERMS.API.Repositories.Interfaces;

namespace ERMS.API.Repositories.Implementations
{
    public class FunctionRepository : IFunctionRepository
    {
        private readonly string _connectionString;

        public FunctionRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")!;
        }

        private MySqlConnection CreateConnection() => new MySqlConnection(_connectionString);

        public async Task<IEnumerable<FunctionResponse>> SearchAsync(string? search, string? status)
        {
            using var conn = CreateConnection();
            return await conn.QueryAsync<FunctionResponse>(
                "sp_Function_Search",
                new { p_Search = search, p_Status = status },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<FunctionResponse?> GetByIdAsync(int functionId)
        {
            using var conn = CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<FunctionResponse>(
                "sp_Function_GetById",
                new { p_FunctionId = functionId },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<int> InsertAsync(FunctionRequest request, int createdBy)
        {
            using var conn = CreateConnection();
            var result = await conn.QueryFirstOrDefaultAsync<dynamic>(
                "sp_Function_Insert",
                new
                {
                    p_FunctionName = request.FunctionName,
                    p_Status = request.Status,
                    p_CreatedBy = createdBy
                },
                commandType: CommandType.StoredProcedure);
            return (int)(result?.NewId ?? 0);
        }

        public async Task UpdateAsync(int functionId, FunctionRequest request, int updatedBy)
        {
            using var conn = CreateConnection();
            await conn.ExecuteAsync(
                "sp_Function_Update",
                new
                {
                    p_FunctionId = functionId,
                    p_FunctionName = request.FunctionName,
                    p_Status = request.Status,
                    p_UpdatedBy = updatedBy
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<int> CheckDuplicateAsync(string functionName, int? functionId)
        {
            using var conn = CreateConnection();
            var result = await conn.QueryFirstOrDefaultAsync<dynamic>(
                "sp_Function_CheckDuplicate",
                new { p_FunctionName = functionName, p_FunctionId = functionId },
                commandType: CommandType.StoredProcedure);
            return (int)(result?.Cnt ?? 0);
        }

        public async Task<IEnumerable<DropdownItem>> GetDropdownAsync()
        {
            using var conn = CreateConnection();
            var items = await conn.QueryAsync<dynamic>(
                "sp_Function_Dropdown",
                commandType: CommandType.StoredProcedure);
            return items.Select(x => new DropdownItem
            {
                Value = x.FunctionId.ToString(),
                Text = (string)x.FunctionName
            });
        }

        public async Task InsertAuditAsync(int functionId, string actionType, int changedBy, string changeSummary)
        {
            using var conn = CreateConnection();
            await conn.ExecuteAsync(
                "sp_Function_Audit_Insert",
                new
                {
                    p_FunctionId = functionId,
                    p_ActionType = actionType,
                    p_ChangedBy = changedBy,
                    p_ChangeSummary = changeSummary
                },
                commandType: CommandType.StoredProcedure);
        }
    }
}
