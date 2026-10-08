using GameNet.Application.Foundation;
using GameNet.Domain.Agents;

namespace GameNet.Application.Agents;

public sealed class AgentService(
    IGameNetStore store,
    ITokenGenerator tokenGenerator,
    IClock clock)
{
    private static readonly TimeSpan PairingLifetime = TimeSpan.FromMinutes(10);

    public async Task<(string PairingCode, DateTimeOffset ExpiresAt)> CreatePairingCodeAsync(
        string deviceId,
        CancellationToken cancellationToken)
    {
        var normalizedDeviceId = NormalizeDeviceId(deviceId);
        var agent = await store.FindAgentByDeviceIdAsync(normalizedDeviceId, cancellationToken);

        if (agent is null)
        {
            agent = AgentDevice.Register(normalizedDeviceId, clock.UtcNow);
            await store.AddAgentAsync(agent, cancellationToken);
        }

        var code = CreateNumericCode();
        var expiresAt = clock.UtcNow.Add(PairingLifetime);
        agent.SetPairingCode(tokenGenerator.Hash(code), expiresAt);

        await store.SaveChangesAsync(cancellationToken);
        return (code, expiresAt);
    }

    public async Task<string?> PairAsync(
        string deviceId,
        string pairingCode,
        string displayName,
        CancellationToken cancellationToken)
    {
        var agent = await store.FindAgentByDeviceIdAsync(
            NormalizeDeviceId(deviceId),
            cancellationToken);

        if (agent is null ||
            !agent.CanPair(tokenGenerator.Hash(pairingCode.Trim()), clock.UtcNow))
        {
            return null;
        }

        var accessToken = tokenGenerator.Generate();

        agent.CompletePairing(
            displayName,
            tokenGenerator.Hash(accessToken));

        try
        {
            await store.SaveChangesAsync(cancellationToken);
        }
        catch (ConcurrencyConflictException)
        {
            return null;
        }

        return accessToken;
    }

    public async Task<long?> OpenLeaseAsync(
        string accessToken,
        string connectionId,
        CancellationToken cancellationToken)
    {
        var agent = await GetAuthorizedAgentAsync(accessToken, cancellationToken);

        if (agent is null)
        {
            return null;
        }

        var leaseVersion = agent.OpenLease(connectionId, clock.UtcNow);

        try
        {
            await store.SaveChangesAsync(cancellationToken);
        }
        catch (ConcurrencyConflictException)
        {
            return null;
        }

        return leaseVersion;
    }

    public async Task<bool> HeartbeatAsync(
        string accessToken,
        string connectionId,
        long leaseVersion,
        CancellationToken cancellationToken)
    {
        var agent = await GetAuthorizedAgentAsync(accessToken, cancellationToken);

        if (agent is null)
        {
            return false;
        }

        var accepted = agent.Heartbeat(connectionId, leaseVersion, clock.UtcNow);

        if (!accepted)
        {
            return false;
        }

        try
        {
            await store.SaveChangesAsync(cancellationToken);
        }
        catch (ConcurrencyConflictException)
        {
            return false;
        }

        return true;
    }

    public async Task DisconnectAsync(
        string accessToken,
        string connectionId,
        CancellationToken cancellationToken)
    {
        var agent = await GetAuthorizedAgentAsync(accessToken, cancellationToken);

        if (agent is null)
        {
            return;
        }

        agent.Disconnect(connectionId, clock.UtcNow);

        try
        {
            await store.SaveChangesAsync(cancellationToken);
        }
        catch (ConcurrencyConflictException)
        {
            // A newer connection or presence update already owns the Agent row.
        }
    }

    public async Task<bool> BindAsync(
        string accessToken,
        Guid stationId,
        CancellationToken cancellationToken)
    {
        var agent = await GetAuthorizedAgentAsync(accessToken, cancellationToken);

        if (agent is null)
        {
            return false;
        }

        return await store.BindAgentToStationAsync(
            agent.Id,
            stationId,
            cancellationToken);
    }

    public Task<IReadOnlyList<AgentDevice>> ListAsync(
        CancellationToken cancellationToken) =>
        store.ListAgentsAsync(cancellationToken);

    public Task<bool> BindByOperatorAsync(
        Guid agentId,
        Guid stationId,
        CancellationToken cancellationToken) =>
        store.BindAgentToStationAsync(agentId, stationId, cancellationToken);

    private async Task<AgentDevice?> GetAuthorizedAgentAsync(
        string accessToken,
        CancellationToken cancellationToken) =>
        string.IsNullOrWhiteSpace(accessToken)
            ? null
            : await store.FindAgentByCredentialHashAsync(
                tokenGenerator.Hash(accessToken),
                cancellationToken);

    private static string NormalizeDeviceId(string deviceId)
    {
        if (string.IsNullOrWhiteSpace(deviceId))
        {
            throw new ArgumentException("DeviceId is required.", nameof(deviceId));
        }

        return deviceId.Trim();
    }

    private static string CreateNumericCode()
    {
        Span<byte> bytes = stackalloc byte[4];
        System.Security.Cryptography.RandomNumberGenerator.Fill(bytes);
        var value = BitConverter.ToUInt32(bytes) % 1_000_000;
        return value.ToString("D6");
    }
}
