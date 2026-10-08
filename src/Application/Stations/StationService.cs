using GameNet.Application.Foundation;
using GameNet.Domain.Stations;

namespace GameNet.Application.Stations;

public sealed class StationService(
    IGameNetStore store,
    IClock clock)
{
    public async Task<Station> CreateAsync(
        int number,
        string name,
        StationType type,
        CancellationToken cancellationToken)
    {
        if (await store.StationNumberExistsAsync(number, cancellationToken))
        {
            throw new InvalidOperationException(
                $"Station number {number} already exists.");
        }

        var station = Station.Create(number, name, type, clock.UtcNow);
        await store.AddStationAsync(station, cancellationToken);
        await store.SaveChangesAsync(cancellationToken);
        return station;
    }

    public Task<IReadOnlyList<Station>> ListAsync(
        CancellationToken cancellationToken) =>
        store.ListStationsAsync(cancellationToken);
}
