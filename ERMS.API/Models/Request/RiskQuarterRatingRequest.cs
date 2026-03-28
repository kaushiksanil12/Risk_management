namespace ERMS.API.Models.Request
{
    public class RiskQuarterRatingRequest
    {
        public int    RiskId             { get; set; }
        public string Quarter            { get; set; } = string.Empty;
        public string GrossImpact        { get; set; } = string.Empty;
        public string GrossLikelihood    { get; set; } = string.Empty;
        public int    GrossScore         { get; set; }
        public string GrossRating        { get; set; } = string.Empty;
        public string ResidualImpact     { get; set; } = string.Empty;
        public string ResidualLikelihood { get; set; } = string.Empty;
        public int    ResidualScore      { get; set; }
        public string ResidualRating     { get; set; } = string.Empty;
    }
}
