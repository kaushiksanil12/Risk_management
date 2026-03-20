namespace ERMS.API.Models.Request
{
    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string LoginType { get; set; } = "Custom";
    }
}
