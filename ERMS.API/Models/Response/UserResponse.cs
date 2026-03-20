namespace ERMS.API.Models.Response
{
    public class UserResponse
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string LoginType { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Mobile { get; set; }
        public string Status { get; set; } = string.Empty;
        public string AdminFlag { get; set; } = "N";
        public int IsLocked { get; set; }
    }
}
