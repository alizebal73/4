using GameNet.Server.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();
builder.Services.AddGameNetInfrastructure();

var app = builder.Build();

app.UseExceptionHandler();

app.MapGet("/api/v1/health/live", () => Results.Ok(new { status = "ok" }));
app.MapHealthChecks("/api/v1/health/ready");

app.Run();

public partial class Program;
