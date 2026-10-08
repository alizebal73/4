using GameNet.Shared.Contracts.V1.Api;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHealthChecks();

var app = builder.Build();

app.MapGet("/api/health", () =>
    Results.Ok(new ApiEnvelope<object>(
        new
        {
            status = "ok",
            service = "GameNet.Server",
            foundation = true
        },
        null)));

app.MapHealthChecks("/health");

app.Run();

public partial class Program
{
}
