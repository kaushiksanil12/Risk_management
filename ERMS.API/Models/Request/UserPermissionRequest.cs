namespace ERMS.API.Models.Request
{
    public class UserPermissionRequest
    {
        public int UserId { get; set; }
        public string BUId { get; set; } = string.Empty;
        public string Role { get; set; } = "N";
    }
}
