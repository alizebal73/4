using GameNet.Application.Foundation;
using GameNet.Application.Identity;
using GameNet.Application.Stations;
using GameNet.Contracts.Stations;
using GameNet.Domain.Identity;
using GameNet.Domain.Stations;
using GameNet.Server.Security;

namespace GameNet.Server.Endpoints;

public static class StationEndpoints
{
    public static IEndpointRouteBuilder MapStationEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/stations", ListAsync);
        app.MapPost("/api/stations", CreateAsync);
        app.MapPut("/api/stations/{stationId:guid}", UpdateAsync);
        app.MapDelete("/api/stations/{stationId:guid}", DeleteAsync);
        return app;
    }

    private static async Task<IResult> ListAsync(
        HttpContext context,
        OperatorAuthService auth,
        StationService stations,
        IGameNetStore store,
        CancellationToken cancellationToken)
    {
        var required = await OperatorAuthorization.RequireAsync(
            context, auth, Permission.StationRead, cancellationToken);

        if (required.Failure is not null)
        {
            return required.Failure;
        }

        var result = await stations.ListAsync(cancellationToken);
        var response = new List<StationDto>(result.Count);

        foreach (var station in result)
        {
            var agent = await store.FindAgentByStationIdAsync(station.Id, cancellationToken);
            response.Add(new StationDto(
                station.Id,
                station.Number,
                station.Name,
                station.Type.ToString(),
                station.Lifecycle.ToString(),
                agent?.State.ToString() ?? "Unassigned",
                agent?.LastSeenAt));
        }

        return Results.Ok(response);
    }

    private static async Task<IResult> CreateAsync(
        HttpContext context,
        OperatorAuthService auth,
        StationService stations,
        CreateStationRequest request,
        CancellationToken cancellationToken)
    {
        var required = await OperatorAuthorization.RequireAsync(
            context, auth, Permission.StationWrite, cancellationToken);

        if (required.Failure is not null)
        {
            return required.Failure;
        }

        if (!Enum.TryParse<StationType>(request.Type, true, out var stationType))
        {
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                ["type"] = ["نوع ایستگاه نامعتبر است."]
            });
        }

        try
        {
            var station = await stations.CreateAsync(
                request.Number, request.Name, stationType, cancellationToken);

            return Results.Created(
                $"/api/stations/{station.Id}",
                new StationDto(
                    station.Id,
                    station.Number,
                    station.Name,
                    station.Type.ToString(),
                    station.Lifecycle.ToString(),
                    "Unassigned",
                    null));
        }
        catch (InvalidOperationException ex)
        {
            return Results.Conflict(new { error = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                ["request"] = [ex.Message]
            });
        }
    }

    private static async Task<IResult> UpdateAsync(
        HttpContext context,
        OperatorAuthService auth,
        StationService stations,
        Guid stationId,
        UpdateStationRequest request,
        CancellationToken cancellationToken)
    {
        var required = await OperatorAuthorization.RequireAsync(
            context, auth, Permission.StationWrite, cancellationToken);

        if (required.Failure is not null)
        {
            return required.Failure;
        }

        if (!Enum.TryParse<StationType>(request.Type, true, out var type) ||
            !Enum.TryParse<StationLifecycle>(request.Lifecycle, true, out var lifecycle))
        {
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                ["request"] = ["نوع یا وضعیت ایستگاه نامعتبر است."]
            });
        }

        try
        {
            var station = await stations.UpdateAsync(
                stationId, request.Name, type, lifecycle, cancellationToken);

            return station is null
                ? Results.NotFound()
                : Results.Ok(new StationDto(
                    station.Id,
                    station.Number,
                    station.Name,
                    station.Type.ToString(),
                    station.Lifecycle.ToString(),
                    "Unknown",
                    null));
        }
        catch (InvalidOperationException ex)
        {
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                ["request"] = [ex.Message]
            });
        }
        catch (ArgumentException ex)
        {
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                ["request"] = [ex.Message]
            });
        }
    }

    private static async Task<IResult> DeleteAsync(
        HttpContext context,
        OperatorAuthService auth,
        StationService stations,
        Guid stationId,
        CancellationToken cancellationToken)
    {
        var required = await OperatorAuthorization.RequireAsync(
            context, auth, Permission.StationWrite, cancellationToken);

        if (required.Failure is not null)
        {
            return required.Failure;
        }

        return await stations.DisableAsync(stationId, cancellationToken)
            ? Results.NoContent()
            : Results.NotFound();
    }
}
