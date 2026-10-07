using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GameNet.Agent;

public sealed class Worker(ILogger<Worker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation(
            "GameNet Agent foundation started at {Time}",
            DateTimeOffset.UtcNow);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}
