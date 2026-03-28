namespace ERMS.API.Models.Response
{
    public class RiskAssessmentResponse
    {
        public int     Id              { get; set; }
        public int     RiskId          { get; set; }
        public string? ResponseMeasure { get; set; }
        public string? CurrentControls { get; set; }
        public string? LineOfAction    { get; set; }
        public string? ResponsibleTeam { get; set; }
        public string? TargetDate      { get; set; }
        public string? FrequencyId     { get; set; }
        public string? FrequencyName   { get; set; }
    }
}
