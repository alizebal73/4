namespace GameNet.Contracts.Foundation;

public sealed record HealthResponse(
    string Status,
    string Database,
    string Version,
    DateTimeOffset UtcTime);
