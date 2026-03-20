namespace ERMS.API.Models.Request
{
    public class UserRequest
    {
        public string Username { get; set; } = string.Empty;
        public string LoginType { get; set; } = "Custom";
        public string? Password { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Mobile { get; set; }
        public string Status { get; set; } = "Active";
        public string AdminFlag { get; set; } = "N";
    }
}
