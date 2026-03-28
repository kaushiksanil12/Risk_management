namespace ERMS.API.Models.Request
{
    public class RiskAssessmentRequest
    {
        public int     RiskId          { get; set; }
        public string? ResponseMeasure { get; set; }
        public string? CurrentControls { get; set; }
        public string? LineOfAction    { get; set; }
        public string? ResponsibleTeam { get; set; }
        public string? TargetDate      { get; set; }  // yyyy-MM-dd
        public string? FrequencyId     { get; set; }
    }
}
