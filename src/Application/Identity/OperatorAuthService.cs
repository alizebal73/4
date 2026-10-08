using System.Security.Cryptography;
using System.Text;
using GameNet.Application.Foundation;
using GameNet.Domain.Identity;

namespace GameNet.Application.Identity;

public sealed class OperatorAuthService(
    IGameNetStore store,
    IPasswordHasher passwordHasher,
    ITokenGenerator tokenGenerator,
    IClock clock)
{
    private static readonly TimeSpan SessionLifetime = TimeSpan.FromHours(12);

    public async Task<(Operator Operator, string AccessToken, DateTimeOffset ExpiresAt)?> AuthenticateAsync(
        string userName,
        string password,
        CancellationToken cancellationToken)
    {
        var account = await store.FindOperatorByUserNameAsync(userName, cancellationToken);

        if (account is null ||
            !account.IsActive ||
            !passwordHasher.Verify(password, account.PasswordHash))
        {
            return null;
        }

        var now = clock.UtcNow;
        var token = tokenGenerator.Generate();
        var session = OperatorSession.Create(
            account.Id,
            tokenGenerator.Hash(token),
            now,
            SessionLifetime);

        await store.AddOperatorSessionAsync(session, cancellationToken);
        await store.SaveChangesAsync(cancellationToken);

        return (account, token, session.ExpiresAt);
    }

    public async Task<Operator?> ValidateAsync(
        string accessToken,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            return null;
        }

        var session = await store.FindOperatorSessionByTokenHashAsync(
            tokenGenerator.Hash(accessToken),
            cancellationToken);

        if (session is null || !session.IsValidAt(clock.UtcNow))
        {
            return null;
        }

        var account = await store.GetOperatorAsync(session.OperatorId, cancellationToken);
        return account is { IsActive: true } ? account : null;
    }

    public static string NormalizeUserName(string userName) =>
        userName.Trim().ToUpperInvariant();
}
