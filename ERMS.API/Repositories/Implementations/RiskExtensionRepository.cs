using Dapper;
using MySqlConnector;
using System.Data;
using ERMS.API.Models.Request;
using ERMS.API.Models.Response;
using ERMS.API.Repositories.Interfaces;

namespace ERMS.API.Repositories.Implementations
{
    public class RiskExtensionRepository : IRiskExtensionRepository
    {
        private readonly string _connectionString;

        public RiskExtensionRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")!;
        }

        private MySqlConnection CreateConnection() => new MySqlConnection(_connectionString);

        // ── People ──────────────────────────────────────────────────────────

        public async Task UpsertPeopleAsync(RiskPeopleRequest request, int userId)
        {
            using var conn = CreateConnection();
            await conn.ExecuteAsync("sp_RiskPeople_InsertOrUpdate",
                new { p_RiskId = request.RiskId, p_OwnerId = request.OwnerId,
                      p_ChampionId = request.ChampionId, p_UserId = userId },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<RiskPeopleResponse?> GetPeopleByRiskAsync(int riskId)
        {
            using var conn = CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<RiskPeopleResponse>(
                "sp_RiskPeople_GetByRisk",
                new { p_RiskId = riskId },
                commandType: CommandType.StoredProcedure);
        }

        // ── Assessment ──────────────────────────────────────────────────────

        public async Task UpsertAssessmentAsync(RiskAssessmentRequest request, int userId)
        {
            using var conn = CreateConnection();
            await conn.ExecuteAsync("sp_RiskAssessment_InsertOrUpdate",
                new {
                    p_RiskId          = request.RiskId,
                    p_ResponseMeasure = request.ResponseMeasure,
                    p_CurrentControls = request.CurrentControls,
                    p_LineOfAction    = request.LineOfAction,
                    p_ResponsibleTeam = request.ResponsibleTeam,
                    p_TargetDate      = string.IsNullOrWhiteSpace(request.TargetDate)
                                        ? (object)DBNull.Value : request.TargetDate,
                    p_FrequencyId     = request.FrequencyId,
                    p_UserId          = userId
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<RiskAssessmentResponse?> GetAssessmentByRiskAsync(int riskId)
        {
            using var conn = CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<RiskAssessmentResponse>(
                "sp_RiskAssessment_GetByRisk",
                new { p_RiskId = riskId },
                commandType: CommandType.StoredProcedure);
        }

        // ── Quarter Rating ──────────────────────────────────────────────────

        public async Task UpsertQuarterRatingAsync(RiskQuarterRatingRequest request, int userId)
        {
            using var conn = CreateConnection();
            await conn.ExecuteAsync("sp_RiskQuarterRating_InsertOrUpdate",
                new {
                    p_RiskId             = request.RiskId,
                    p_Quarter            = request.Quarter,
                    p_GrossImpact        = request.GrossImpact,
                    p_GrossLikelihood    = request.GrossLikelihood,
                    p_GrossScore         = request.GrossScore,
                    p_GrossRating        = request.GrossRating,
                    p_ResidualImpact     = request.ResidualImpact,
                    p_ResidualLikelihood = request.ResidualLikelihood,
                    p_ResidualScore      = request.ResidualScore,
                    p_ResidualRating     = request.ResidualRating,
                    p_UserId             = userId
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<RiskQuarterRatingResponse>> GetRatingsByRiskAsync(int riskId)
        {
            using var conn = CreateConnection();
            return await conn.QueryAsync<RiskQuarterRatingResponse>(
                "sp_RiskQuarterRating_GetByRisk",
                new { p_RiskId = riskId },
                commandType: CommandType.StoredProcedure);
        }

        // ── Attachments ─────────────────────────────────────────────────────

        public async Task<int> InsertAttachmentAsync(RiskAttachmentRequest request, int userId)
        {
            using var conn = CreateConnection();
            var result = await conn.QueryFirstOrDefaultAsync<dynamic>(
                "sp_RiskAttachment_Insert",
                new {
                    p_RiskId   = request.RiskId,
                    p_FileName = request.FileName,
                    p_FilePath = request.FilePath,
                    p_FileSize = request.FileSize,
                    p_FileType = request.FileType,
                    p_UserId   = userId
                },
                commandType: CommandType.StoredProcedure);
            return (int)(result?.NewId ?? 0);
        }

        public async Task<IEnumerable<RiskAttachmentResponse>> GetAttachmentsByRiskAsync(int riskId)
        {
            using var conn = CreateConnection();
            return await conn.QueryAsync<RiskAttachmentResponse>(
                "sp_RiskAttachment_GetByRisk",
                new { p_RiskId = riskId },
                commandType: CommandType.StoredProcedure);
        }

        public async Task DeleteAttachmentAsync(int attachmentId, int riskId)
        {
            using var conn = CreateConnection();
            await conn.ExecuteAsync("sp_RiskAttachment_Delete",
                new { p_AttachmentId = attachmentId, p_RiskId = riskId },
                commandType: CommandType.StoredProcedure);
        }

        // ── Full Risk ───────────────────────────────────────────────────────

        public async Task<RiskFullResponse?> GetFullByRiskAsync(int riskId)
        {
            using var conn = CreateConnection();
            using var multi = await conn.QueryMultipleAsync(
                "sp_RiskFull_GetById",
                new { p_RiskId = riskId },
                commandType: CommandType.StoredProcedure);

            var core        = await multi.ReadFirstOrDefaultAsync<RiskResponse>();
            var people      = await multi.ReadFirstOrDefaultAsync<RiskPeopleResponse>();
            var assessment  = await multi.ReadFirstOrDefaultAsync<RiskAssessmentResponse>();
            var ratings     = (await multi.ReadAsync<RiskQuarterRatingResponse>()).ToList();
            var attachments = (await multi.ReadAsync<RiskAttachmentResponse>()).ToList();
            var history     = (await multi.ReadAsync<RiskHistoryResponse>()).ToList();

            if (core == null) return null;

            return new RiskFullResponse
            {
                Core        = core,
                People      = people,
                Assessment  = assessment,
                Ratings     = ratings,
                Attachments = attachments,
                History     = history
            };
        }
    }
}
