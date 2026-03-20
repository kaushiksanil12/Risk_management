using ERMS.API.Helpers;
using ERMS.API.Models.Request;
using ERMS.API.Models.Response;
using ERMS.API.Repositories.Interfaces;
using ERMS.API.Services.Interfaces;
using System.Text.Json;

namespace ERMS.API.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;

        public UserService(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<ApiResponse<List<UserResponse>>> SearchAsync(string? search, string? status)
        {
            var result = await _userRepo.SearchAsync(search, status);
            return ApiResponse<List<UserResponse>>.Ok(result.ToList());
        }

        public async Task<ApiResponse<UserResponse>> GetByIdAsync(int userId)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null) return ApiResponse<UserResponse>.NotFound("User not found.");
            return ApiResponse<UserResponse>.Ok(user);
        }

        public async Task<ApiResponse<int>> CreateAsync(UserRequest request, int createdBy)
        {
            if (string.IsNullOrWhiteSpace(request.Username))
                return ApiResponse<int>.Fail("Username is required.");
            if (string.IsNullOrWhiteSpace(request.FullName))
                return ApiResponse<int>.Fail("Full name is required.");
            if (string.IsNullOrWhiteSpace(request.Email))
                return ApiResponse<int>.Fail("Email is required.");

            var cnt = await _userRepo.CheckUsernameAsync(request.Username, null);
            if (cnt > 0) return ApiResponse<int>.Fail("Username already exists.");

            if (request.LoginType == "Custom")
            {
                if (string.IsNullOrWhiteSpace(request.Password))
                    return ApiResponse<int>.Fail("Password is required for Custom login.");
                request.Password = PasswordHelper.HashPassword(request.Password);
            }

            var newId = await _userRepo.InsertAsync(request, createdBy);
            await _userRepo.InsertAuditAsync(newId, "INSERT", createdBy, null, JsonSerializer.Serialize(request));
            return ApiResponse<int>.Ok(newId, "User created successfully.");
        }

        public async Task<ApiResponse<bool>> UpdateAsync(int userId, UserRequest request, int updatedBy)
        {
            if (string.IsNullOrWhiteSpace(request.FullName))
                return ApiResponse<bool>.Fail("Full name is required.");
            if (string.IsNullOrWhiteSpace(request.Email))
                return ApiResponse<bool>.Fail("Email is required.");

            var existing = await _userRepo.GetByIdAsync(userId);
            if (existing == null) return ApiResponse<bool>.NotFound("User not found.");

            var cnt = await _userRepo.CheckUsernameAsync(existing.Username, userId);
            if (cnt > 0) return ApiResponse<bool>.Fail("Username already exists.");

            if (request.LoginType == "Custom" && !string.IsNullOrWhiteSpace(request.Password))
                request.Password = PasswordHelper.HashPassword(request.Password);
            else
                request.Password = null;

            var oldData = JsonSerializer.Serialize(existing);
            await _userRepo.UpdateAsync(userId, request, updatedBy);
            await _userRepo.InsertAuditAsync(userId, "UPDATE", updatedBy, oldData, JsonSerializer.Serialize(request));
            return ApiResponse<bool>.Ok(true, "User updated successfully.");
        }

        public async Task<ApiResponse<bool>> UnlockAsync(int userId, int updatedBy)
        {
            await _userRepo.UnlockAsync(userId, updatedBy);
            return ApiResponse<bool>.Ok(true, "User unlocked successfully.");
        }

        public async Task<ApiResponse<List<DropdownItem>>> GetDropdownAsync()
        {
            var items = await _userRepo.GetDropdownAsync();
            return ApiResponse<List<DropdownItem>>.Ok(items.ToList());
        }
    }
}
