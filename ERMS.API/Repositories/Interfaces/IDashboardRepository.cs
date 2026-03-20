using ERMS.API.Models.Response;

namespace ERMS.API.Repositories.Interfaces
{
    public interface IDashboardRepository
    {
        Task<DashboardResponse> GetSummaryAsync(int userId, string adminFlag);
    }
}
