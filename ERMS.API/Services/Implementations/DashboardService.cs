using ERMS.API.Models.Response;
using ERMS.API.Repositories.Interfaces;
using ERMS.API.Services.Interfaces;

namespace ERMS.API.Services.Implementations
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _repo;

        public DashboardService(IDashboardRepository repo)
        {
            _repo = repo;
        }

        public async Task<ApiResponse<DashboardResponse>> GetSummaryAsync(int userId, string adminFlag)
        {
            var result = await _repo.GetSummaryAsync(userId, adminFlag);
            return ApiResponse<DashboardResponse>.Ok(result);
        }
    }
}
