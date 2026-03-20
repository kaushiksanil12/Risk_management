using Dapper;
using MySqlConnector;
using System.Data;
using ERMS.API.Models.Request;
using ERMS.API.Models.Response;
using ERMS.API.Repositories.Interfaces;

namespace ERMS.API.Repositories.Implementations
{
    public class RiskRepository : IRiskRepository
    {
        private readonly string _connectionString;

        public RiskRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")!;
        }

        private MySqlConnection CreateConnection() => new MySqlConnection(_connectionString);

        public async Task<int> InsertAsync(RiskRequest request, int createdBy)
        {
            using var conn = CreateConnection();
            var result = await conn.QueryFirstOrDefaultAsync<dynamic>(
                "sp_Risk_Insert",
                new
                {
                    p_BUId = request.BUId,
                    p_FYId = request.FYId,
                    p_RiskCatId = request.RiskCatId,
                    p_RiskSubCatId = request.RiskSubCatId,
                    p_FunctionId = request.FunctionId,
                    p_RiskTitle = request.RiskTitle,
                    p_Description = request.Description,
                    p_ImpactLevel = request.ImpactLevel,
                    p_Likelihood = request.Likelihood,
                    p_CreatedBy = createdBy
                },
                commandType: CommandType.StoredProcedure);
            return (int)(result?.NewRiskId ?? 0);
        }

        public async Task<RiskResponse?> GetByIdAsync(int riskId)
        {
            using var conn = CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<RiskResponse>(
                "sp_Risk_GetById",
                new { p_RiskId = riskId },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<RiskResponse>> SearchAsync(int userId, string adminFlag, string? buId, string? fyId, int? riskCatId, string? status, string? search)
        {
            using var conn = CreateConnection();
            return await conn.QueryAsync<RiskResponse>(
                "sp_Risk_Search",
                new
                {
                    p_UserId = userId,
                    p_AdminFlag = adminFlag,
                    p_BUId = buId,
                    p_FYId = fyId,
                    p_RiskCatId = riskCatId,
                    p_Status = status,
                    p_Search = search
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task UpdateAsync(int riskId, RiskRequest request, int updatedBy)
        {
            using var conn = CreateConnection();
            await conn.ExecuteAsync(
                "sp_Risk_Update",
                new
                {
                    p_RiskId = riskId,
                    p_BUId = request.BUId,
                    p_FYId = request.FYId,
                    p_RiskCatId = request.RiskCatId,
                    p_RiskSubCatId = request.RiskSubCatId,
                    p_FunctionId = request.FunctionId,
                    p_RiskTitle = request.RiskTitle,
                    p_Description = request.Description,
                    p_ImpactLevel = request.ImpactLevel,
                    p_Likelihood = request.Likelihood,
                    p_UpdatedBy = updatedBy
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task UpdateStatusAsync(int riskId, string newStatus, string? remarks, int changedBy)
        {
            using var conn = CreateConnection();
            await conn.ExecuteAsync(
                "sp_Risk_UpdateStatus",
                new
                {
                    p_RiskId = riskId,
                    p_NewStatus = newStatus,
                    p_Remarks = remarks,
                    p_ChangedBy = changedBy
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task InsertHistoryAsync(int riskId, string? oldStatus, string newStatus, string action, string? remarks, int changedBy)
        {
            using var conn = CreateConnection();
            await conn.ExecuteAsync(
                "sp_Risk_History_Insert",
                new
                {
                    p_RiskId = riskId,
                    p_OldStatus = oldStatus,
                    p_NewStatus = newStatus,
                    p_Action = action,
                    p_Remarks = remarks,
                    p_ChangedBy = changedBy
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<RiskHistoryResponse>> GetHistoryByRiskAsync(int riskId)
        {
            using var conn = CreateConnection();
            return await conn.QueryAsync<RiskHistoryResponse>(
                "sp_Risk_History_GetByRisk",
                new { p_RiskId = riskId },
                commandType: CommandType.StoredProcedure);
        }
        public async Task<(RiskEmailOwnerDto? Owner, IEnumerable<RiskEmailChampionDto> Champions)>
            GetEmailRecipientsAsync(int riskId)
        {
            using var conn = CreateConnection();
            using var multi = await conn.QueryMultipleAsync(
                "sp_Risk_GetEmailRecipients",
                new { p_RiskId = riskId },
                commandType: CommandType.StoredProcedure
            );
            var owner = await multi.ReadFirstOrDefaultAsync<RiskEmailOwnerDto>();
            var champions = await multi.ReadAsync<RiskEmailChampionDto>();
            return (owner, champions);
        }
    }
}
