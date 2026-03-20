using ERMS.API.Models.Request;
using ERMS.API.Models.Response;

namespace ERMS.API.Repositories.Interfaces
{
    public interface IRiskRepository
    {
        Task<int> InsertAsync(RiskRequest request, int createdBy);
        Task<RiskResponse?> GetByIdAsync(int riskId);
        Task<IEnumerable<RiskResponse>> SearchAsync(int userId, string adminFlag, string? buId, string? fyId, int? riskCatId, string? status, string? search);
        Task UpdateAsync(int riskId, RiskRequest request, int updatedBy);
        Task UpdateStatusAsync(int riskId, string newStatus, string? remarks, int changedBy);
        Task InsertHistoryAsync(int riskId, string? oldStatus, string newStatus, string action, string? remarks, int changedBy);
        Task<IEnumerable<RiskHistoryResponse>> GetHistoryByRiskAsync(int riskId);
    }
}
