namespace GameNet.Domain.Agents;

public sealed class AgentDevice
{
    private AgentDevice() { }

    private AgentDevice(
        Guid id,
        string deviceId,
        DateTimeOffset createdAt)
    {
        Id = id;
        DeviceId = deviceId;
        State = AgentState.Registered;
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }
    public string DeviceId { get; private set; } = string.Empty;
    public string DisplayName { get; private set; } = string.Empty;
    public string? CredentialHash { get; private set; }
    public string? PairingCodeHash { get; private set; }
    public DateTimeOffset? PairingExpiresAt { get; private set; }
    public AgentState State { get; private set; }
    public Guid? StationId { get; private set; }
    public string? ConnectionId { get; private set; }
    public long LeaseVersion { get; private set; }
    public DateTimeOffset? LastSeenAt { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public static AgentDevice Register(string deviceId, DateTimeOffset createdAt)
    {
        if (string.IsNullOrWhiteSpace(deviceId))
        {
            throw new ArgumentException("DeviceId is required.", nameof(deviceId));
        }

        return new AgentDevice(Guid.NewGuid(), deviceId.Trim(), createdAt);
    }

    public void SetPairingCode(string codeHash, DateTimeOffset expiresAt)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(codeHash);

        PairingCodeHash = codeHash;
        PairingExpiresAt = expiresAt;
        State = AgentState.Registered;
    }

    public bool CanPair(string codeHash, DateTimeOffset now) =>
        State is not AgentState.Disabled &&
        PairingExpiresAt is not null &&
        PairingExpiresAt > now &&
        !string.IsNullOrWhiteSpace(PairingCodeHash) &&
        string.Equals(PairingCodeHash, codeHash, StringComparison.Ordinal);

    public void CompletePairing(string displayName, string credentialHash)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(credentialHash);

        DisplayName = string.IsNullOrWhiteSpace(displayName) ? DeviceId : displayName.Trim();
        CredentialHash = credentialHash;
        PairingCodeHash = null;
        PairingExpiresAt = null;
        State = AgentState.Paired;
        ConnectionId = null;
        LeaseVersion = 0;
    }

    public long OpenLease(string connectionId, DateTimeOffset now)
    {
        if (State is AgentState.Disabled)
        {
            throw new InvalidOperationException("Disabled agents cannot open a lease.");
        }

        if (string.IsNullOrWhiteSpace(CredentialHash))
        {
            throw new InvalidOperationException("Agent is not paired.");
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(connectionId);

        LeaseVersion = checked(LeaseVersion + 1);
        ConnectionId = connectionId;
        State = AgentState.Online;
        LastSeenAt = now;
        return LeaseVersion;
    }

    public bool Heartbeat(string connectionId, long leaseVersion, DateTimeOffset now)
    {
        if (State is AgentState.Disabled ||
            !string.Equals(ConnectionId, connectionId, StringComparison.Ordinal) ||
            LeaseVersion != leaseVersion)
        {
            return false;
        }

        LastSeenAt = now;
        State = AgentState.Online;
        return true;
    }

    public bool ShouldBecomeStale(DateTimeOffset now, TimeSpan timeout) =>
        State == AgentState.Online &&
        LastSeenAt is not null &&
        now - LastSeenAt >= timeout;

    public void MarkStale(DateTimeOffset now)
    {
        if (State == AgentState.Online)
        {
            State = AgentState.Stale;
            ConnectionId = null;
            LastSeenAt = now;
        }
    }

    public void Disconnect(string connectionId, DateTimeOffset now)
    {
        if (string.Equals(ConnectionId, connectionId, StringComparison.Ordinal))
        {
            ConnectionId = null;
            State = AgentState.Stale;
            LastSeenAt = now;
        }
    }

    public void BindStation(Guid stationId)
    {
        if (State is AgentState.Disabled)
        {
            throw new InvalidOperationException("Disabled agents cannot bind to a station.");
        }

        StationId = stationId;
    }

    public void UnbindStation(Guid stationId)
    {
        if (StationId == stationId)
        {
            StationId = null;
        }
    }

    public void Disable()
    {
        State = AgentState.Disabled;
        ConnectionId = null;
    }
}
