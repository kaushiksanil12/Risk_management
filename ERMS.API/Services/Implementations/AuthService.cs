using ERMS.API.Helpers;
using ERMS.API.Models.Request;
using ERMS.API.Models.Response;
using ERMS.API.Repositories.Interfaces;
using ERMS.API.Services.Interfaces;

namespace ERMS.API.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepo;
        private readonly IConfiguration _config;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IAuthRepository authRepo, IConfiguration config, ILogger<AuthService> logger)
        {
            _authRepo = authRepo;
            _config = config;
            _logger = logger;
        }

        public async Task<ApiResponse<UserResponse>> LoginAsync(LoginRequest request, string ipAddress)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                return ApiResponse<UserResponse>.Fail("Username and password are required.");

            var user = await _authRepo.GetUserForLoginAsync(request.Username);
            if (user == null)
            {
                await _authRepo.InsertLoginAuditAsync(null, request.Username, request.LoginType, "FAILED", ipAddress, "User not found");
                return ApiResponse<UserResponse>.Fail("Invalid username or password.", 401);
            }

            int userId = Convert.ToInt32(user.UserId);
            int isLocked = user.IsLocked is bool ? ((bool)user.IsLocked ? 1 : 0) : Convert.ToInt32(user.IsLocked ?? 0);
            string status = user.Status?.ToString() ?? "";
            int failedAttempts = Convert.ToInt32(user.FailedAttempts ?? 0);
            string dbPassword = user.Password?.ToString() ?? "";
            string loginType = user.LoginType?.ToString() ?? "Custom";

            if (isLocked == 1)
            {
                await _authRepo.InsertLoginAuditAsync(userId, request.Username, loginType, "LOCKED", ipAddress, "Account is locked");
                return ApiResponse<UserResponse>.Fail("Account is locked. Contact administrator.", 423);
            }

            if (status == "Inactive")
            {
                await _authRepo.InsertLoginAuditAsync(userId, request.Username, loginType, "FAILED", ipAddress, "Account inactive");
                return ApiResponse<UserResponse>.Fail("Account is inactive.", 403);
            }

            bool authenticated = false;

            if (loginType == "Custom")
            {
                var hashedInput = PasswordHelper.HashPassword(request.Password);
                authenticated = hashedInput == dbPassword;
            }
            else
            {
                // LDAP placeholder
                bool ldapEnabled = _config.GetValue<bool>("Ldap:Enabled");
                if (!ldapEnabled)
                {
                    await _authRepo.InsertLoginAuditAsync(userId, request.Username, loginType, "FAILED", ipAddress, "LDAP not enabled");
                    return ApiResponse<UserResponse>.Fail("LDAP authentication is not enabled.", 400);
                }
                authenticated = false; // LDAP stub
            }

            if (!authenticated)
            {
                failedAttempts++;
                int maxAttempts = _config.GetValue<int>("ApiSettings:MaxFailedLoginAttempts");
                await _authRepo.UpdateFailedAttemptsAsync(userId, failedAttempts);

                if (failedAttempts >= maxAttempts)
                {
                    await _authRepo.LockAccountAsync(userId);
                    await _authRepo.InsertLoginAuditAsync(userId, request.Username, loginType, "LOCKED", ipAddress, $"Locked after {failedAttempts} failed attempts");
                    return ApiResponse<UserResponse>.Fail("Account locked due to too many failed attempts.", 423);
                }

                await _authRepo.InsertLoginAuditAsync(userId, request.Username, loginType, "FAILED", ipAddress, "Invalid password");
                return ApiResponse<UserResponse>.Fail("Invalid username or password.", 401);
            }

            // Success
            await _authRepo.ResetFailedAttemptsAsync(userId);
            await _authRepo.InsertLoginAuditAsync(userId, request.Username, loginType, "SUCCESS", ipAddress, null);

            var userResponse = new UserResponse
            {
                UserId = userId,
                Username = request.Username,
                FullName = "", // Will be populated by the caller
                AdminFlag = "",
                LoginType = loginType
            };

            return ApiResponse<UserResponse>.Ok(userResponse, "Login successful.");
        }
    }
}
