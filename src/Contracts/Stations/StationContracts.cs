namespace GameNet.Contracts.Stations;

public sealed record CreateStationRequest(
    int Number,
    string Name,
    string Type);

public sealed record UpdateStationRequest(
    string Name,
    string Type,
    string Lifecycle);

public sealed record StationDto(
    Guid Id,
    int Number,
    string Name,
    string Type,
    string Lifecycle,
    string AgentState,
    DateTimeOffset? LastSeenAt);
