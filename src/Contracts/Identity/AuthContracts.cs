namespace GameNet.Contracts.Identity;

public sealed record LoginRequest(string UserName, string Password);

public sealed record OperatorDto(
    Guid Id,
    string UserName,
    string DisplayName,
    string Role);

public sealed record LoginResponse(
    string AccessToken,
    DateTimeOffset ExpiresAt,
    OperatorDto Operator);
