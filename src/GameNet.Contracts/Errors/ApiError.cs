namespace GameNet.Contracts.Errors;

public sealed record ApiError(string Code, string Message, string? CorrelationId = null);
