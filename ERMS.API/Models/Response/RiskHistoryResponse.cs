namespace ERMS.API.Models.Response
{
    public class RiskHistoryResponse
    {
        public int HistoryId { get; set; }
        public string? OldStatus { get; set; }
        public string? NewStatus { get; set; }
        public string? Action { get; set; }
        public string? Remarks { get; set; }
        public DateTime ChangedDate { get; set; }
        public string ChangedByName { get; set; } = string.Empty;
    }
}
