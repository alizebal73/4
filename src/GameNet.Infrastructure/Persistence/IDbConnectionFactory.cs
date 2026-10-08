using System.Data.Common;

namespace GameNet.Infrastructure.Persistence;

/// <summary>Database boundary; concrete PostgreSQL provider stays in Infrastructure.</summary>
public interface IDbConnectionFactory
{
    ValueTask<DbConnection> OpenConnectionAsync(CancellationToken cancellationToken = default);
}
