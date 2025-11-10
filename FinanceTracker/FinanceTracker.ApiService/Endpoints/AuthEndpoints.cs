
using System.Security.Claims;
using FinanceTracker.ApiService.Models;
using FinanceTracker.ApiService.Services;

namespace FinanceTracker.ApiService.Controllers;

public static class AuthEndpoints
{
    public static RouteGroupBuilder MapAuthEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/auth").WithTags("Authentication");

        group.MapPost("/login", static async (UserLoginDTO userLogin, IAuthService authService) =>
        {
            var token = await authService.LoginAsync(userLogin);
            if (token is null)
            {
                return Results.Unauthorized();
            }

            return Results.Ok(token);
        }).WithName("Login")
        .WithSummary("User login")
        .WithDescription("Authenticates a user and returns JWT access and refresh tokens.");

        group.MapPost("/register", async (UserRegisterDTO request, IAuthService authService) =>
        {
            var user = await authService.RegisterAsync(request);
            if (user is null)
            {
                return Results.BadRequest("Username already exists.");
            }

            return Results.CreatedAtRoute("Login", new {username = user.Username }, user);
        }).WithName("Register")
        .WithSummary("User registration")
        .WithDescription("Registers a new user with the provided details.");

        group.MapPost("/refresh-token", async (RefreshTokenRequestDTO request, IAuthService authService) =>
        {
            var result = await authService.RefreshTokenAsync(request);
            if (result is null || result.AccessToken is null || result.RefreshToken is null)
                return Results.Unauthorized();

            return Results.Ok(result);
        })
        .WithName("RefreshToken")
        .WithSummary("Refreshes JWT tokens")
        .WithDescription("Generates new access and refresh tokens using a valid refresh token.");

        group.MapGet("/me", async (ClaimsPrincipal user, IAuthService authService) =>
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim is null)
            {
                return Results.Unauthorized();
            }

            var userId = userIdClaim.Value;
            var userInfo = await authService.GetUserByIdAsync(userId);
            if (userInfo is null)
            {
                return Results.NotFound();
            }

            return Results.Ok(userInfo);
        }).RequireAuthorization()
        .WithName("GetCurrentUser")
        .WithSummary("Gets current user info")
        .WithDescription("Retrieves information about the currently authenticated user.");

        group.MapPost("/logout", async (ClaimsPrincipal user, IAuthService authService) =>
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim is not null)
            {
                var userId = userIdClaim.Value;
                await authService.LogoutAsync(userId);
            }

            return Results.Ok();
        }).RequireAuthorization()
        .WithName("Logout")
        .WithSummary("User logout")
        .WithDescription("Logs out the currently authenticated user and invalidates their refresh token.");

        group.MapPatch("/change-password", async (ClaimsPrincipal user, ChangePasswordDTO changePasswordDTO, IAuthService authService) =>
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim is null)
            {
                return Results.Unauthorized();
            }

            var userId = userIdClaim.Value;
            var result = await authService.ChangePasswordAsync(changePasswordDTO, userId);
            if (!result)
            {
                return Results.BadRequest("Password change failed.");
            }

            return Results.Ok("Password changed successfully.");
        }).RequireAuthorization()
        .WithName("ChangePassword")
        .WithSummary("Change user password")
        .WithDescription("Allows the currently authenticated user to change their password.");

        group.MapPut("/update-profile", async (ClaimsPrincipal user, UpdateUserDTO updateUserDTO, IAuthService authService) =>
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim is null)
            {
                return Results.Unauthorized();
            }

            var userId = userIdClaim.Value;
            var result = await authService.UpdateUserProfileAsync(updateUserDTO, userId);
            if (!result)
            {
                return Results.BadRequest("Profile update failed.");
            }

            return Results.Ok("Profile updated successfully.");
        }).RequireAuthorization()
        .WithName("UpdateUserProfile")
        .WithSummary("Update user profile")
        .WithDescription("Allows the currently authenticated user to update their profile information.");
        
        return group;
    }
}

