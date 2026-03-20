namespace ERMS.API.Models.Response
{
    public class UserPermissionResponse
    {
        public string BUId { get; set; } = string.Empty;
        public string BUName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int? PermissionId { get; set; }
        public string Role { get; set; } = "N";
    }
}
