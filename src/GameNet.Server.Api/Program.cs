using GameNet.Application.Abstractions;
using GameNet.Application.Time;
using GameNet.Contracts.Health;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IClock, SystemClock>();
var app = builder.Build();

app.MapGet("/health/live", (IClock clock) =>
    Results.Ok(new HealthResponse("ok", "GameNet.Server.Api", app.Environment.ApplicationName, clock.UtcNow)));

app.MapGet("/health/ready", () =>
    Results.Json(new { status = "not-ready", reason = "Persistence adapter has not been configured." }, statusCode: 503));

app.Run();
