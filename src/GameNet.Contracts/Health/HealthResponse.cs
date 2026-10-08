namespace GameNet.Contracts.Health;

public sealed record HealthResponse(string Status, string Service, string Version, DateTimeOffset Utc);
