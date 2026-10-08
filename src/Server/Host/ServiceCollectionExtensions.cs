using Microsoft.Extensions.DependencyInjection;

namespace GameNet.Server.Host;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGameNetServer(this IServiceCollection services)
    {
        services.AddHealthChecks();
        return services;
    }
}
