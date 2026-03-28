using ERMS.API.Models.Request;
using ERMS.API.Models.Response;

namespace ERMS.API.Repositories.Interfaces
{
    public interface IRiskExtensionRepository
    {
        Task UpsertPeopleAsync(RiskPeopleRequest request, int userId);
        Task<RiskPeopleResponse?> GetPeopleByRiskAsync(int riskId);

        Task UpsertAssessmentAsync(RiskAssessmentRequest request, int userId);
        Task<RiskAssessmentResponse?> GetAssessmentByRiskAsync(int riskId);

        Task UpsertQuarterRatingAsync(RiskQuarterRatingRequest request, int userId);
        Task<IEnumerable<RiskQuarterRatingResponse>> GetRatingsByRiskAsync(int riskId);

        Task<int> InsertAttachmentAsync(RiskAttachmentRequest request, int userId);
        Task<IEnumerable<RiskAttachmentResponse>> GetAttachmentsByRiskAsync(int riskId);
        Task DeleteAttachmentAsync(int attachmentId, int riskId);

        Task<RiskFullResponse?> GetFullByRiskAsync(int riskId);
    }
}
