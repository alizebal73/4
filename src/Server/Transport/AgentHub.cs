using GameNet.Application.Agents;

namespace GameNet.Server.Transport;

public sealed class AgentHub : Microsoft.AspNetCore.SignalR.Hub
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
        var token = httpContext?.Request.Headers.Authorization
            .ToString();

        if (token?.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) == true)
        {
            return token["Bearer ".Length..].Trim();
        }

        var queryToken = httpContext?.Request.Query["access_token"].ToString();

        return string.IsNullOrWhiteSpace(queryToken) ? null : queryToken;
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        _ = DisconnectAsync();
        return base.OnDisconnectedAsync(exception);
    }

    private async Task DisconnectAsync()
    {
        var httpContext = Context.GetHttpContext();
        var token = GetAccessToken();

        if (httpContext is null || token is null)
        {
            return;
        }

        var agents = httpContext.RequestServices
            .GetRequiredService<AgentService>();

        await agents.DisconnectAsync(
            token,
            Context.ConnectionId,
            CancellationToken.None);
    }
}
