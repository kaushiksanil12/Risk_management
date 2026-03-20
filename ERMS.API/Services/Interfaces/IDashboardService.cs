using ERMS.API.Models.Response;

namespace ERMS.API.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<ApiResponse<DashboardResponse>> GetSummaryAsync(int userId, string adminFlag);
    }
}
