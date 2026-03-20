using Dapper;
using MySqlConnector;
using System.Data;
using ERMS.API.Models.Request;
using ERMS.API.Models.Response;
using ERMS.API.Repositories.Interfaces;

namespace ERMS.API.Repositories.Implementations
{
    public class RiskSubCategoryRepository : IRiskSubCategoryRepository
    {
        private readonly string _connectionString;

        public RiskSubCategoryRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")!;
        }

        private MySqlConnection CreateConnection() => new MySqlConnection(_connectionString);

        public async Task<IEnumerable<RiskSubCategoryResponse>> SearchAsync(string? search, string? status, int? riskCatId)
        {
            using var conn = CreateConnection();
            return await conn.QueryAsync<RiskSubCategoryResponse>(
                "sp_RiskSubCategory_Search",
                new { p_Search = search, p_Status = status, p_RiskCatId = riskCatId },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<RiskSubCategoryResponse?> GetByIdAsync(int id)
        {
            using var conn = CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<RiskSubCategoryResponse>(
                "sp_RiskSubCategory_GetById",
                new { p_Id = id },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<int> InsertAsync(RiskSubCategoryRequest request, int createdBy)
        {
            using var conn = CreateConnection();
            var result = await conn.QueryFirstOrDefaultAsync<dynamic>(
                "sp_RiskSubCategory_Insert",
                new
                {
                    p_RiskCatId = request.RiskCatId,
                    p_RiskSubCatName = request.RiskSubCatName,
                    p_RiskCatAlias = request.RiskCatAlias,
                    p_Status = request.Status,
                    p_CreatedBy = createdBy
                },
                commandType: CommandType.StoredProcedure);
            return (int)(result?.NewId ?? 0);
        }

        public async Task UpdateAsync(int id, RiskSubCategoryRequest request, int updatedBy)
        {
            using var conn = CreateConnection();
            await conn.ExecuteAsync(
                "sp_RiskSubCategory_Update",
                new
                {
                    p_Id = id,
                    p_RiskCatId = request.RiskCatId,
                    p_RiskSubCatName = request.RiskSubCatName,
                    p_RiskCatAlias = request.RiskCatAlias,
                    p_Status = request.Status,
                    p_UpdatedBy = updatedBy
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<int> CheckDuplicateAsync(int riskCatId, string name, int? id)
        {
            using var conn = CreateConnection();
            var result = await conn.QueryFirstOrDefaultAsync<dynamic>(
                "sp_RiskSubCategory_CheckDuplicate",
                new { p_RiskCatId = riskCatId, p_Name = name, p_Id = id },
                commandType: CommandType.StoredProcedure);
            return (int)(result?.Cnt ?? 0);
        }

        public async Task<IEnumerable<RiskSubCategoryResponse>> GetDropdownAsync(int riskCatId, string mode)
        {
            using var conn = CreateConnection();
            return await conn.QueryAsync<RiskSubCategoryResponse>(
                "sp_RiskSubCategory_Dropdown",
                new { p_RiskCatId = riskCatId, p_Mode = mode },
                commandType: CommandType.StoredProcedure);
        }

        public async Task InsertAuditAsync(int riskSubCatId, string actionType, int changedBy, string changeSummary)
        {
            using var conn = CreateConnection();
            await conn.ExecuteAsync(
                "sp_RiskSubCategory_Audit_Insert",
                new
                {
                    p_RiskSubCatId = riskSubCatId,
                    p_ActionType = actionType,
                    p_ChangedBy = changedBy,
                    p_ChangeSummary = changeSummary
                },
                commandType: CommandType.StoredProcedure);
        }
    }
}
