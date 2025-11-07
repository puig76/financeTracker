using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FinanceTracker.ApiService.Data;
using FinanceTracker.ApiService.Entities;
using FinanceTracker.ApiService.Mapping;
using FinanceTracker.ApiService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace FinanceTracker.ApiService.Services;

public class AuthService(FinanceDBContext context, IConfiguration configuration) : IAuthService
{
    public async Task<TokenResponseDTO?> LoginAsync(UserLoginDTO userLogin)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Username == userLogin.Username);
        if (user is null)
        {
            return null;
        }
        if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, userLogin.Password) == PasswordVerificationResult.Failed)
        {
            return null;
        }

        return await CreateTokenResponse(user);
    }
    public async Task<TokenResponseDTO?> RefreshTokenAsync(RefreshTokenRequestDTO request)
    {
        var user = await VerifyRefreshToken(request.UserId, request.RefreshToken);
        if (user is null)
            return null;

        return await CreateTokenResponse(user);
    }
    public async Task<User?> RegisterAsync(UserRegisterDTO request)
    {
        if (await context.Users.AnyAsync(u => u.Username == request.Username))
        {
            return null;
        }

        var user = request.ToEntity();

        context.Users.Add(user);
        await context.SaveChangesAsync();

        return user;
    }   
    public async Task LogoutAsync(string userId)
    {
        var userEntity = await context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == userId);
        if (userEntity is null)
        {
            return;
        }

        // Invalidate the refresh token
        userEntity.RefreshToken = null;
        userEntity.RefreshTokenExpiryTime = null;
        await context.SaveChangesAsync();
    }
    private async Task<TokenResponseDTO> CreateTokenResponse(User user)
    {
        return new TokenResponseDTO(
                    AccessToken: CreateToken(user),
                    RefreshToken: await SetRefreshToken(user)
                );
    }
    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
    private async Task<string> SetRefreshToken(User user)
    {
        var refreshToken = GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await context.SaveChangesAsync();

        return refreshToken;
    }
    private string CreateToken(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
            configuration.GetSection("AppSettings:Secret").Value!));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        var token = new JwtSecurityToken(
            issuer: configuration.GetSection("AppSettings:Issuer").Value!,
            audience: configuration.GetSection("AppSettings:Audience").Value!,
            claims: claims,
            expires: DateTime.Now.AddMinutes(configuration.GetValue<int>("AppSettings:ExpirationInMins")!),
            signingCredentials: creds);

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return jwt;

    }
    private async Task<User?> VerifyRefreshToken(Guid userId, string refreshToken)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return null;
        }

        return user;
    }
    public Task<UserDTO?> GetUserByIdAsync(string userId)
    {
        var userGuid = Guid.Parse(userId);
        return context.Users
            .Where(u => u.Id == userGuid)
            .Select(u => u.ToDTO())
            .FirstOrDefaultAsync();
    }
    public async Task<bool> ChangePasswordAsync(ChangePasswordDTO changePasswordDTO, string userId)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == Guid.Parse(userId));
        if (user is null)
            return false;

        var verificationResult = new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, changePasswordDTO.CurrentPassword);
        if (verificationResult == PasswordVerificationResult.Failed)
            return false;

        user.PasswordHash = new PasswordHasher<User>().HashPassword(user, changePasswordDTO.NewPassword);
        await context.SaveChangesAsync();
        return true;
    }
    public async Task<bool> UpdateUserProfileAsync(UpdateUserDTO updateUserDTO, string userId)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == Guid.Parse(userId));
        if (user is null)
            return false;

        user.Email = updateUserDTO.Email;
        user.Username = updateUserDTO.UserName;

        await context.SaveChangesAsync();
        return true;
    }

}

