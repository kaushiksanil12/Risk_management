namespace ERMS.API.Models.Response
{
    public class DashboardResponse
    {
        public int TotalRisks { get; set; }
        public List<StatusCount> RisksByStatus { get; set; } = new();
        public List<NameCount> RisksByBU { get; set; } = new();
        public List<NameCount> RisksByCategory { get; set; } = new();
        public List<HighAlertItem> HighAlerts { get; set; } = new();
        public List<FYCount> RisksByFY { get; set; } = new();
    }

    public class StatusCount
    {
        public string Status { get; set; } = string.Empty;
        public int Cnt { get; set; }
    }

    public class NameCount
    {
        public string BUName { get; set; } = string.Empty;
        public string RiskCatName { get; set; } = string.Empty;
        public int Cnt { get; set; }
    }

    public class HighAlertItem
    {
        public int RiskId { get; set; }
        public string RiskTitle { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string ImpactLevel { get; set; } = string.Empty;
        public string BUName { get; set; } = string.Empty;
        public string RiskCatName { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
    }

    public class FYCount
    {
        public string FYId { get; set; } = string.Empty;
        public string FYName { get; set; } = string.Empty;
        public int Cnt { get; set; }
    }
}
