using GameNet.Application.Foundation;

namespace GameNet.Server.Background;

public sealed class AgentPresenceMonitor(
    IGameNetStore store,
    IClock clock,
    ILogger<AgentPresenceMonitor> logger) : BackgroundService
{
    private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(15);
    private static readonly TimeSpan StaleAfter = TimeSpan.FromSeconds(30);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await MarkStaleAgentsAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                return;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Agent presence monitor failed.");
            }

            try
            {
                await Task.Delay(PollInterval, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                return;
            }
        }
    }

    private async Task MarkStaleAgentsAsync(CancellationToken cancellationToken)
    {
        var agents = await store.ListAgentsAsync(cancellationToken);
        var now = clock.UtcNow;
        var changed = false;

        foreach (var agent in agents)
        {
            if (!agent.ShouldBecomeStale(now, StaleAfter))
            {
                continue;
            }

            agent.MarkStale(now);
            changed = true;
        }

        if (changed)
        {
            await store.SaveChangesAsync(cancellationToken);
        }
    }
}
