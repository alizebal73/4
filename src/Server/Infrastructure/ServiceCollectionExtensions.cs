using Microsoft.Extensions.Configuration;
using Npgsql;

namespace GameNet.Server.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGameNetInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSignalR();

        var connectionString =
            configuration.GetConnectionString("GameNet")
            ?? configuration["GAMENET_DATABASE"];

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            services.AddHealthChecks().AddCheck(
                "postgresql",
                () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Unhealthy(
                    "No PostgreSQL connection string is configured."),
                tags: new[] { "ready", "database" });

            return services;
        }

        var dataSource = NpgsqlDataSource.Create(connectionString);
        services.AddSingleton(dataSource);
        services.AddHealthChecks().AddCheck<PostgreSqlHealthCheck>(
            "postgresql",
            tags: new[] { "ready", "database" });

        return services;
    }
}
