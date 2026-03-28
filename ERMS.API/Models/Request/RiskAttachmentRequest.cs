namespace ERMS.API.Models.Request
{
    public class RiskAttachmentRequest
    {
        public int    RiskId   { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public int    FileSize { get; set; }
        public string FileType { get; set; } = string.Empty;
    }
}
