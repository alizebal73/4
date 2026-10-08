using GameNet.Application.Identity;
using GameNet.Contracts.Identity;
using GameNet.Domain.Identity;
using GameNet.Server.Security;

namespace GameNet.Server.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/login", LoginAsync);
        app.MapPost("/api/auth/logout", LogoutAsync);
        app.MapGet("/api/auth/me", MeAsync);
        return app;
    }

    private static async Task<IResult> LoginAsync(
        LoginRequest request,
        OperatorAuthService auth,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.UserName) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                ["request"] = ["نام کاربری و رمز عبور الزامی است."]
            });
        }

        var result = await auth.AuthenticateAsync(
            request.UserName,
            request.Password,
            cancellationToken);

        if (result is null)
        {
            return Results.Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                title: "Invalid credentials.");
        }

        var (operatorAccount, token, expiresAt) = result.Value;

        return Results.Ok(new LoginResponse(
            token,
            expiresAt,
            new OperatorDto(
                operatorAccount.Id,
                operatorAccount.UserName,
                operatorAccount.DisplayName,
                operatorAccount.Role.ToString())));
    }

    private static async Task<IResult> LogoutAsync(
        HttpContext context,
        OperatorAuthService auth,
        CancellationToken cancellationToken)
    {
        var header = context.Request.Headers.Authorization.ToString();

        if (!header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return Results.NoContent();
        }

        var token = header["Bearer ".Length..].Trim();
        await auth.RevokeAsync(token, cancellationToken);

        return Results.NoContent();
    }

    private static async Task<IResult> MeAsync(
        HttpContext context,
        OperatorAuthService auth,
        CancellationToken cancellationToken)
    {
        var account = await OperatorAuthorization.GetOperatorAsync(
            context,
            auth,
            cancellationToken);

        return account is null
            ? Results.Problem(statusCode: StatusCodes.Status401Unauthorized)
            : Results.Ok(new OperatorDto(
                account.Id,
                account.UserName,
                account.DisplayName,
                account.Role.ToString()));
    }
}
