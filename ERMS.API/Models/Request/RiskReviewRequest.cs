namespace ERMS.API.Models.Request
{
    public class RiskReviewRequest
    {
        public string Action { get; set; } = string.Empty; // Approve, Reject, SendBack
        public string? Remarks { get; set; }
    }
}
