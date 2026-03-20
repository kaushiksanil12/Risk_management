using System.Security.Cryptography;
using System.Text;

namespace ERMS.API.Helpers
{
    public static class PasswordHelper
    {
        /// <summary>
        /// Produces identical output to MariaDB SHA2('password', 256)
        /// </summary>
        public static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }
}
