namespace ERMS.API.Models.Email
{
    /// <summary>
    /// Internal model to carry email data
    /// </summary>
    public class EmailMessage
    {
        public string ToEmail  { get; set; } = string.Empty;
        public string ToName   { get; set; } = string.Empty;
        public string Subject  { get; set; } = string.Empty;
        public string HtmlBody { get; set; } = string.Empty;
    }
}
