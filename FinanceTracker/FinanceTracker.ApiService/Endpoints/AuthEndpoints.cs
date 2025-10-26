
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

        group.MapPost("/logout", () => Results.Ok()).RequireAuthorization();

        return group;
    }
}

