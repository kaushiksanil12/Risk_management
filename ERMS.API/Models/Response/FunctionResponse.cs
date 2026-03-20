namespace ERMS.API.Models.Response
{
    public class FunctionResponse
    {
        public int FunctionId { get; set; }
        public string FunctionName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
