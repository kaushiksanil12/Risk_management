namespace ERMS.API.Models.Response
{
    public class RiskEmailOwnerDto
    {
        public string OwnerEmail { get; set; } = string.Empty;
        public string OwnerName  { get; set; } = string.Empty;
        public string RiskTitle  { get; set; } = string.Empty;
        public string BUId       { get; set; } = string.Empty;
        public string BUName     { get; set; } = string.Empty;
        public string Status     { get; set; } = string.Empty;
    }
}
