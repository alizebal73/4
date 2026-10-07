using GameNet.Application.Foundation;
using GameNet.Contracts.Foundation;
using GameNet.Infrastructure.Foundation;
using GameNet.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("PostgreSQL")
    ?? throw new InvalidOperationException(
        "ConnectionStrings:PostgreSQL is required. Configure it with appsettings or environment variables.");

builder.Services.AddSingleton<IClock, SystemClock>();
builder.Services.AddDbContext<GameNetDbContext>(options =>
    options.UseNpgsql(connectionString, npgsql =>
    {
        npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "gamenet");
    }));

var app = builder.Build();

app.MapGet("/api/health", async (
    GameNetDbContext db,
    IClock clock,
    CancellationToken cancellationToken) =>
{
    var database = "unavailable";

    try
    {
        database = await db.Database.CanConnectAsync(cancellationToken)
            ? "ready"
            : "unavailable";
    }
    catch
    {
        database = "unavailable";
    }

    var status = database == "ready" ? "ok" : "degraded";

    return Results.Ok(new HealthResponse(
        status,
        database,
        typeof(Program).Assembly.GetName().Version?.ToString() ?? "0.1.0",
        clock.UtcNow));
});

app.Run();

public partial class Program;
