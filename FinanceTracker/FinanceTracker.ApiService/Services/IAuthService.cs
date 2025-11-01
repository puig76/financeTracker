using System;
using FinanceTracker.ApiService.Entities;
using FinanceTracker.ApiService.Models;

namespace FinanceTracker.ApiService.Services;

public interface IAuthService
{
    Task<User?> RegisterAsync(UserRegisterDTO userLoginDTO);
    Task<TokenResponseDTO?> LoginAsync(UserLoginDTO userLoginDTO);
    Task<TokenResponseDTO?> RefreshTokenAsync(RefreshTokenRequestDTO request);
    Task<UserDTO?> GetUserByIdAsync(string userId);
    Task<bool> ChangePasswordAsync(ChangePasswordDTO changePasswordDTO, string userId);
    Task<bool> UpdateUserProfileAsync(UpdateUserDTO updateUserDTO, string userId);
}
