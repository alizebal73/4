using GameNet.Application.Agents;
using Microsoft.AspNetCore.SignalR;

namespace GameNet.Server.Transport;

public sealed class AgentHub : Hub
{
    public async Task<long?> OpenLease(
        AgentService agents,
        CancellationToken cancellationToken)
    {
        var token = GetAccessToken();

        return token is null
            ? null
            : await agents.OpenLeaseAsync(
                token,
                Context.ConnectionId,
                cancellationToken);
    }

    public Task<bool> Heartbeat(
        AgentService agents,
        long leaseVersion,
        CancellationToken cancellationToken)
    {
        var token = GetAccessToken();

        return token is null
            ? Task.FromResult(false)
            : agents.HeartbeatAsync(
                token,
                Context.ConnectionId,
                leaseVersion,
                cancellationToken);
    }

    private string? GetAccessToken()
    {
        var httpContext = Context.GetHttpContext();
        var token = httpContext?.Request.Headers.Authorization.ToString();

        if (token?.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) == true)
        {
            return token["Bearer ".Length..].Trim();
        }

        var queryToken = httpContext?.Request.Query["access_token"].ToString();
        return string.IsNullOrWhiteSpace(queryToken) ? null : queryToken;
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var token = GetAccessToken();

        if (token is not null)
        {
            await DisconnectAsync(token);
        }

        await base.OnDisconnectedAsync(exception);
    }

    private async Task DisconnectAsync(string token)
    {
        var agents = Context.GetHttpContext()?
            .RequestServices
            .GetRequiredService<AgentService>();

        if (agents is null)
        {
            return;
        }

        await agents.DisconnectAsync(
            token,
            Context.ConnectionId,
            CancellationToken.None);
    }
}
