using GameNet.Application.Identity;
using GameNet.Domain.Identity;

namespace GameNet.Server.Security;

internal static class OperatorAuthorization
{
    public static async Task<Operator?> GetOperatorAsync(
        HttpContext context,
        OperatorAuthService auth,
        CancellationToken cancellationToken)
    {
        var header = context.Request.Headers.Authorization.ToString();

        if (!header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var token = header["Bearer ".Length..].Trim();
        return await auth.ValidateAsync(token, cancellationToken);
    }

    public static async Task<(Operator? Operator, IResult? Failure)> RequireAsync(
        HttpContext context,
        OperatorAuthService auth,
        Permission permission,
        CancellationToken cancellationToken)
    {
        var account = await GetOperatorAsync(context, auth, cancellationToken);

        if (account is null)
        {
            return (null, Results.Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                title: "Authentication required."));
        }

        if (!account.HasPermission(permission))
        {
            return (null, Results.Problem(
                statusCode: StatusCodes.Status403Forbidden,
                title: "Permission denied."));
        }

        return (account, null);
    }
}
