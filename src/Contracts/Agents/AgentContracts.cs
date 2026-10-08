namespace GameNet.Contracts.Agents;

public sealed record CreatePairingCodeRequest(string DeviceId);

public sealed record CreatePairingCodeResponse(
    string DeviceId,
    string PairingCode,
    DateTimeOffset ExpiresAt);

public sealed record PairAgentRequest(
    string DeviceId,
    string PairingCode,
    string DisplayName);

public sealed record PairAgentResponse(
    string DeviceId,
    string AccessToken);

public sealed record BindAgentRequest(Guid StationId);

public sealed record AgentHeartbeatResponse(
    bool Accepted,
    long LeaseVersion,
    DateTimeOffset ServerTime);
