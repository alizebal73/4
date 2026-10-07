namespace GameNet.Shared.Contracts.V1.Api;

public sealed record ApiError(string Code, string Message, string CorrelationId);
