namespace GameNet.Application.Abstractions;

public interface IPersistenceReadiness
{
    ValueTask<bool> IsReadyAsync(CancellationToken cancellationToken = default);
}
