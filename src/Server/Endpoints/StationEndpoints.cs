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
        return app;
    }

    private static async Task<IResult> ListAsync(
        HttpContext context,
        OperatorAuthService auth,
        StationService stations,
        CancellationToken cancellationToken)
    {
        var required = await OperatorAuthorization.RequireAsync(
            context,
            auth,
            Permission.StationRead,
            cancellationToken);

        if (required.Failure is not null)
        {
            return required.Failure;
        }

        var result = await stations.ListAsync(cancellationToken);

        return Results.Ok(result.Select(station => new StationDto(
            station.Id,
            station.Number,
            station.Name,
            station.Type.ToString(),
            station.Lifecycle.ToString(),
            "Offline",
            null)));
    }

    private static async Task<IResult> CreateAsync(
        HttpContext context,
        OperatorAuthService auth,
        StationService stations,
        CreateStationRequest request,
        CancellationToken cancellationToken)
    {
        var required = await OperatorAuthorization.RequireAsync(
            context,
            auth,
            Permission.StationWrite,
            cancellationToken);

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
                request.Number,
                request.Name,
                stationType,
                cancellationToken);

            return Results.Created(
                $"/api/stations/{station.Id}",
                new StationDto(
                    station.Id,
                    station.Number,
                    station.Name,
                    station.Type.ToString(),
                    station.Lifecycle.ToString(),
                    "Offline",
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
}
