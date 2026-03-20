using Dapper;
using MySqlConnector;
using System.Data;
using ERMS.API.Models.Request;
using ERMS.API.Models.Response;
using ERMS.API.Repositories.Interfaces;

namespace ERMS.API.Repositories.Implementations
{
    public class RiskCategoryRepository : IRiskCategoryRepository
    {
        private readonly string _connectionString;

        public RiskCategoryRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")!;
        }

        private MySqlConnection CreateConnection() => new MySqlConnection(_connectionString);

        public async Task<IEnumerable<RiskCategoryResponse>> SearchAsync(string? search, string? status)
        {
            using var conn = CreateConnection();
            return await conn.QueryAsync<RiskCategoryResponse>(
                "sp_RiskCategory_Search",
                new { p_Search = search, p_Status = status },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<RiskCategoryResponse?> GetByIdAsync(int riskCatId)
        {
            using var conn = CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<RiskCategoryResponse>(
                "sp_RiskCategory_GetById",
                new { p_RiskCatId = riskCatId },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<int> InsertAsync(RiskCategoryRequest request, int createdBy)
        {
            using var conn = CreateConnection();
            var result = await conn.QueryFirstOrDefaultAsync<dynamic>(
                "sp_RiskCategory_Insert",
                new
                {
                    p_RiskCatName = request.RiskCatName,
                    p_RiskCatAlias = request.RiskCatAlias,
                    p_Status = request.Status,
                    p_CreatedBy = createdBy
                },
                commandType: CommandType.StoredProcedure);
            return (int)(result?.NewId ?? 0);
        }

        public async Task UpdateAsync(int riskCatId, RiskCategoryRequest request, int updatedBy)
        {
            using var conn = CreateConnection();
            await conn.ExecuteAsync(
                "sp_RiskCategory_Update",
                new
                {
                    p_RiskCatId = riskCatId,
                    p_RiskCatName = request.RiskCatName,
                    p_RiskCatAlias = request.RiskCatAlias,
                    p_Status = request.Status,
                    p_UpdatedBy = updatedBy
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<int> CheckDuplicateAsync(string riskCatName, int? riskCatId)
        {
            using var conn = CreateConnection();
            var result = await conn.QueryFirstOrDefaultAsync<dynamic>(
                "sp_RiskCategory_CheckDuplicate",
                new { p_RiskCatName = riskCatName, p_RiskCatId = riskCatId },
                commandType: CommandType.StoredProcedure);
            return (int)(result?.Cnt ?? 0);
        }

        public async Task<IEnumerable<RiskCategoryResponse>> GetDropdownAsync(string mode)
        {
            using var conn = CreateConnection();
            return await conn.QueryAsync<RiskCategoryResponse>(
                "sp_RiskCategory_Dropdown",
                new { p_Mode = mode },
                commandType: CommandType.StoredProcedure);
        }

        public async Task InsertAuditAsync(int riskCatId, string actionType, int changedBy, string changeSummary)
        {
            using var conn = CreateConnection();
            await conn.ExecuteAsync(
                "sp_RiskCategory_Audit_Insert",
                new
                {
                    p_RiskCatId = riskCatId,
                    p_ActionType = actionType,
                    p_ChangedBy = changedBy,
                    p_ChangeSummary = changeSummary
                },
                commandType: CommandType.StoredProcedure);
        }
    }
}
