using GameNet.Application.Abstractions;

namespace GameNet.Infrastructure.Persistence;

public sealed class PersistenceReadiness(IDbConnectionFactory connectionFactory) : IPersistenceReadiness
{
    public async ValueTask<bool> IsReadyAsync(CancellationToken cancellationToken = default)
    {
        await using var connection = await connectionFactory.OpenConnectionAsync(cancellationToken);
        return connection.State == System.Data.ConnectionState.Open;
    }
}
