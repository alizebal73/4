namespace GameNet.Shared.Contracts.V1.Api;

public sealed record ErrorResponse(
    string Code,
    string Message,
    string? CorrelationId = null);
