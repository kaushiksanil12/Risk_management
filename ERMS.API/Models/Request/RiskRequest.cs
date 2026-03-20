namespace ERMS.API.Models.Request
{
    public class RiskRequest
    {
        public string BUId { get; set; } = string.Empty;
        public string FYId { get; set; } = string.Empty;
        public int RiskCatId { get; set; }
        public int RiskSubCatId { get; set; }
        public int FunctionId { get; set; }
        public string RiskTitle { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImpactLevel { get; set; } = string.Empty;
        public string Likelihood { get; set; } = string.Empty;
    }
}
