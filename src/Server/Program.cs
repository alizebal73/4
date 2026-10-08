using GameNet.Application.Agents;
using GameNet.Application.Foundation;
using GameNet.Application.Identity;
using GameNet.Application.Stations;
using GameNet.Contracts.Foundation;
using GameNet.Infrastructure.Foundation;
using GameNet.Infrastructure.Persistence;
using GameNet.Infrastructure.Security;
using GameNet.Server.Bootstrap;
using GameNet.Server.Endpoints;
using GameNet.Server.Transport;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("PostgreSQL")
    ?? throw new InvalidOperationException(
        "ConnectionStrings:PostgreSQL is required. Configure it with appsettings or environment variables.");

builder.Services.AddSingleton<IClock, SystemClock>();
builder.Services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();
builder.Services.AddSingleton<ITokenGenerator, SecureTokenGenerator>();

builder.Services.AddDbContext<GameNetDbContext>(options =>
    options.UseNpgsql(connectionString, npgsql =>
    {
        npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "gamenet");
    }));

builder.Services.AddScoped<IGameNetStore, GameNetStore>();
builder.Services.AddScoped<OperatorAuthService>();
builder.Services.AddScoped<StationService>();
builder.Services.AddScoped<AgentService>();
builder.Services.AddSignalR();
builder.Services.AddHostedService<AgentPresenceMonitor>();

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<GameNetDbContext>();
    await db.Database.MigrateAsync();

    var store = scope.ServiceProvider.GetRequiredService<IGameNetStore>();
    var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

    await DatabaseSeeder.EnsureBootstrapOperatorAsync(
        store,
        hasher,
        app.Configuration,
        CancellationToken.None);
}

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

app.MapAuthEndpoints();
app.MapStationEndpoints();
app.MapAgentEndpoints();
app.MapHub<AgentHub>("/hubs/agent");

app.Run();

public partial class Program;
