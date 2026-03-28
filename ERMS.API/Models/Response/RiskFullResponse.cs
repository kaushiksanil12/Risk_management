namespace ERMS.API.Models.Response
{
    public class RiskFullResponse
    {
        public RiskResponse?                          Core        { get; set; }
        public RiskPeopleResponse?                    People      { get; set; }
        public RiskAssessmentResponse?                Assessment  { get; set; }
        public List<RiskQuarterRatingResponse>        Ratings     { get; set; } = new();
        public List<RiskAttachmentResponse>           Attachments { get; set; } = new();
        public List<RiskHistoryResponse>              History     { get; set; } = new();
    }
}
