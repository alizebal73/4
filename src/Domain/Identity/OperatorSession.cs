namespace GameNet.Domain.Identity;

public sealed class OperatorSession
{
    private OperatorSession() { }

    private OperatorSession(
        Guid id,
        Guid operatorId,
        string tokenHash,
        DateTimeOffset createdAt,
        DateTimeOffset expiresAt)
    {
        Id = id;
        OperatorId = operatorId;
        TokenHash = tokenHash;
        CreatedAt = createdAt;
        ExpiresAt = expiresAt;
    }

    public Guid Id { get; private set; }
    public Guid OperatorId { get; private set; }
    public string TokenHash { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset ExpiresAt { get; private set; }
    public DateTimeOffset? RevokedAt { get; private set; }

    public static OperatorSession Create(
        Guid operatorId,
        string tokenHash,
        DateTimeOffset createdAt,
        TimeSpan lifetime)
    {
        return new OperatorSession(
            Guid.NewGuid(),
            operatorId,
            tokenHash,
            createdAt,
            createdAt.Add(lifetime));
    }

    public bool IsValidAt(DateTimeOffset now) =>
        RevokedAt is null && now < ExpiresAt;

    public void Revoke(DateTimeOffset now) => RevokedAt ??= now;
}
