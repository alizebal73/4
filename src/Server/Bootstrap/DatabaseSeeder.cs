using GameNet.Application.Foundation;
using GameNet.Application.Identity;
using GameNet.Domain.Identity;

namespace GameNet.Server.Bootstrap;

public static class DatabaseSeeder
{
    public static async Task EnsureBootstrapOperatorAsync(
        IGameNetStore store,
        IPasswordHasher passwordHasher,
        IConfiguration configuration,
        CancellationToken cancellationToken)
    {
        var existing = await store.ListOperatorsAsync(cancellationToken);

        if (existing.Count > 0)
        {
            return;
        }

        var userName = configuration["GAMENET_BOOTSTRAP_USERNAME"];
        var password = configuration["GAMENET_BOOTSTRAP_PASSWORD"];

        if (string.IsNullOrWhiteSpace(userName) ||
            string.IsNullOrWhiteSpace(password))
        {
            return;
        }

        var displayName = configuration["GAMENET_BOOTSTRAP_DISPLAY_NAME"] ??
                          "GameNet Owner";

        var operatorAccount = Operator.Create(
            OperatorAuthService.NormalizeUserName(userName),
            displayName,
            passwordHasher.Hash(password),
            OperatorRole.Owner,
            DateTimeOffset.UtcNow);

        await store.AddOperatorAsync(operatorAccount, cancellationToken);
        await store.SaveChangesAsync(cancellationToken);
    }
}
