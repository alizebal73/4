using GameNet.Application.Agents;
using GameNet.Application.Identity;
using GameNet.Contracts.Agents;
using GameNet.Domain.Identity;
using GameNet.Server.Security;

namespace GameNet.Server.Endpoints;

public static class AgentEndpoints
{
    public static IEndpointRouteBuilder MapAgentEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/agents", ListAsync);
        app.MapPost("/api/agents/pairing-code", CreatePairingCodeAsync);
        app.MapPost("/api/agents/pair", PairAsync);
        app.MapPost("/api/agents/{agentId:guid}/bind", BindAsync);
        return app;
    }

    private static async Task<IResult> ListAsync(
        HttpContext context,
        OperatorAuthService auth,
        AgentService agents,
        CancellationToken cancellationToken)
    {
        var required = await OperatorAuthorization.RequireAsync(
            context,
            auth,
            Permission.AgentRead,
            cancellationToken);

        if (required.Failure is not null)
        {
            return required.Failure;
        }

        var result = await agents.ListAsync(cancellationToken);

        return Results.Ok(result.Select(agent => new AgentDto(
            agent.Id,
            agent.DeviceId,
            agent.DisplayName,
            agent.State.ToString(),
            agent.StationId,
            agent.LeaseVersion,
            agent.LastSeenAt)));
    }

    private static async Task<IResult> CreatePairingCodeAsync(
        HttpContext context,
        OperatorAuthService auth,
        AgentService agents,
        CreatePairingCodeRequest request,
        CancellationToken cancellationToken)
    {
        var required = await OperatorAuthorization.RequireAsync(
            context,
            auth,
            Permission.AgentManage,
            cancellationToken);

        if (required.Failure is not null)
        {
            return required.Failure;
        }

        try
        {
            var pairing = await agents.CreatePairingCodeAsync(
                request.DeviceId,
                cancellationToken);

            return Results.Ok(new CreatePairingCodeResponse(
                request.DeviceId.Trim(),
                pairing.PairingCode,
                pairing.ExpiresAt));
        }
        catch (ArgumentException ex)
        {
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                ["deviceId"] = [ex.Message]
            });
        }
    }

    private static async Task<IResult> PairAsync(
        AgentService agents,
        PairAgentRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var token = await agents.PairAsync(
                request.DeviceId,
                request.PairingCode,
                request.DisplayName,
                cancellationToken);

            return token is null
                ? Results.Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Pairing failed or code expired.")
                : Results.Ok(new PairAgentResponse(request.DeviceId.Trim(), token));
        }
        catch (ArgumentException ex)
        {
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                ["deviceId"] = [ex.Message]
            });
        }
    }

    private static async Task<IResult> BindAsync(
        HttpContext context,
        OperatorAuthService auth,
        AgentService agents,
        Guid agentId,
        BindAgentRequest request,
        CancellationToken cancellationToken)
    {
        var required = await OperatorAuthorization.RequireAsync(
            context,
            auth,
            Permission.AgentManage,
            cancellationToken);

        if (required.Failure is not null)
        {
            return required.Failure;
        }

        var bound = await agents.BindByOperatorAsync(
            agentId,
            request.StationId,
            cancellationToken);

        return bound
            ? Results.NoContent()
            : Results.Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Agent or station binding conflict.");
    }
}
