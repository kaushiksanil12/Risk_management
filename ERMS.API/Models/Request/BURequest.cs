namespace ERMS.API.Models.Request
{
    public class BURequest
    {
        public string BUName { get; set; } = string.Empty;
        public string BUShortName { get; set; } = string.Empty;
        public string Status { get; set; } = "Active";
    }
}
