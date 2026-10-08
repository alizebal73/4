using GameNet.Shared.Contracts.V1.Api;

namespace GameNet.Server.Api;

public static class EndpointRouteExtensions
{
    public static IEndpointRouteBuilder MapGameNetEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/health", () =>
            Results.Ok(new ApiEnvelope<object>(
                new
                {
                    status = "ok",
                    service = "GameNet.Server",
                    foundation = true
                },
                null)));

        endpoints.MapHealthChecks("/health");

        return endpoints;
    }
}
