using GameNet.Agent.Security;
using Microsoft.AspNetCore.SignalR.Client;

namespace GameNet.Agent;

public sealed class Worker(
    ILogger<Worker> logger,
    AgentCredentialStore credentialStore) : BackgroundService
{
    private static readonly TimeSpan RetryDelay = TimeSpan.FromSeconds(5);
    private static readonly TimeSpan HeartbeatInterval = TimeSpan.FromSeconds(10);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var token = credentialStore.TryRead() ??
                        Environment.GetEnvironmentVariable("GAMENET_AGENT_TOKEN");

            if (string.IsNullOrWhiteSpace(token))
            {
                logger.LogWarning("Agent is not paired. Run the provisioning command first.");
                await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
                continue;
            }

            try
            {
                await RunConnectionAsync(token, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                return;
            }
            catch (Exception exception)
            {
                logger.LogWarning(
                    exception,
                    "Agent transport failed; retrying in {DelaySeconds}s.",
                    RetryDelay.TotalSeconds);
            }

            await Task.Delay(RetryDelay, stoppingToken);
        }
    }

    private async Task RunConnectionAsync(
        string token,
        CancellationToken cancellationToken)
    {
        var serverUrl = Environment.GetEnvironmentVariable("GAMENET_AGENT_SERVER")
            ?? "http://127.0.0.1:5080";

        await using var connection = new HubConnectionBuilder()
            .WithUrl(
                $"{serverUrl.TrimEnd('/')}/hubs/agent",
                options => options.AccessTokenProvider = () => Task.FromResult<string?>(token))
            .WithAutomaticReconnect()
            .Build();

        await connection.StartAsync(cancellationToken);

        var leaseVersion = await connection.InvokeAsync<long?>(
            "OpenLease",
            cancellationToken);

        if (leaseVersion is null)
        {
            throw new InvalidOperationException(
                "Server rejected the Agent credential.");
        }

        logger.LogInformation(
            "Agent connected. LeaseVersion={LeaseVersion}",
            leaseVersion);

        while (connection.State == HubConnectionState.Connected &&
               !cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(HeartbeatInterval, cancellationToken);

            var accepted = await connection.InvokeAsync<bool>(
                "Heartbeat",
                leaseVersion.Value,
                cancellationToken);

            if (!accepted)
            {
                throw new InvalidOperationException(
                    "Server rejected the current Agent lease.");
            }
        }
    }
}
