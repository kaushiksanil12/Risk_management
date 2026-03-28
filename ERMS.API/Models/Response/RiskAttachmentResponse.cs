namespace ERMS.API.Models.Response
{
    public class RiskAttachmentResponse
    {
        public int    Id             { get; set; }
        public int    RiskId         { get; set; }
        public string FileName       { get; set; } = string.Empty;
        public string FilePath       { get; set; } = string.Empty;
        public int    FileSize       { get; set; }
        public string FileType       { get; set; } = string.Empty;
        public string UploadedByName { get; set; } = string.Empty;
        public string UploadedDate   { get; set; } = string.Empty;
    }
}
