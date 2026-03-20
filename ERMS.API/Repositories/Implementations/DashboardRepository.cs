using Dapper;
using MySqlConnector;
using System.Data;
using ERMS.API.Models.Response;
using ERMS.API.Repositories.Interfaces;

namespace ERMS.API.Repositories.Implementations
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly string _connectionString;

        public DashboardRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")!;
        }

        private MySqlConnection CreateConnection() => new MySqlConnection(_connectionString);

        public async Task<DashboardResponse> GetSummaryAsync(int userId, string adminFlag)
        {
            using var conn = CreateConnection();
            using var multi = await conn.QueryMultipleAsync(
                "sp_Dashboard_Summary",
                new { p_UserId = userId, p_AdminFlag = adminFlag },
                commandType: CommandType.StoredProcedure);

            var response = new DashboardResponse();

            // Result set 1: Total count
            var totalResult = await multi.ReadFirstOrDefaultAsync<dynamic>();
            response.TotalRisks = (int)(totalResult?.TotalRisks ?? 0);

            // Result set 2: By Status
            response.RisksByStatus = (await multi.ReadAsync<StatusCount>()).ToList();

            // Result set 3: By BU
            response.RisksByBU = (await multi.ReadAsync<NameCount>()).ToList();

            // Result set 4: By Category
            response.RisksByCategory = (await multi.ReadAsync<NameCount>()).ToList();

            // Result set 5: High impact alerts
            response.HighAlerts = (await multi.ReadAsync<HighAlertItem>()).ToList();

            // Result set 6: By FY
            response.RisksByFY = (await multi.ReadAsync<FYCount>()).ToList();

            return response;
        }
    }
}
