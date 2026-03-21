namespace ERMS.Web.Models
{
    public class UserPermissionResponse
    {
        public string BUId { get; set; } = string.Empty;
        public string BUName { get; set; } = string.Empty;
        public string Role { get; set; } = "N"; // O | C | V | N
        public string Status { get; set; } = string.Empty;
    }
}
