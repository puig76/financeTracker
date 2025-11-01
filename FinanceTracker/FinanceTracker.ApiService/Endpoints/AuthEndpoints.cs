
using System.Security.Claims;
using FinanceTracker.ApiService.Models;
using FinanceTracker.ApiService.Services;

namespace FinanceTracker.ApiService.Controllers;

public static class AuthEndpoints
{
    public static RouteGroupBuilder MapAuthEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/auth");

        group.MapPost("/login", static async (UserLoginDTO userLogin, IAuthService authService) =>
        {
            var token = await authService.LoginAsync(userLogin);
            if (token is null)
            {
                return Results.Unauthorized();
            }

            return Results.Ok(token);
        }).WithName("Login");

        group.MapPost("/register", async (UserRegisterDTO request, IAuthService authService) =>
        {
            var user = await authService.RegisterAsync(request);
            if (user is null)
            {
                return Results.BadRequest("Username already exists.");
            }

            return Results.CreatedAtRoute("Login", new {username = user.Username }, user);
        });

        group.MapPost("/refresh-token", async (RefreshTokenRequestDTO request, IAuthService authService) =>
        {
            var result = await authService.RefreshTokenAsync(request);
            if (result is null || result.AccessToken is null || result.RefreshToken is null)
                return Results.Unauthorized();

            return Results.Ok(result);
        });

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
        }).RequireAuthorization();

        group.MapPost("/logout", () => Results.Ok()).RequireAuthorization();

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
        }).RequireAuthorization();

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
        }).RequireAuthorization();
        
        return group;
    }
}

