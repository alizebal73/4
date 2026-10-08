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

    public async Task<Station?> UpdateAsync(
        Guid stationId,
        string name,
        StationType type,
        StationLifecycle lifecycle,
        CancellationToken cancellationToken)
    {
        var station = await store.GetStationAsync(stationId, cancellationToken);

        if (station is null)
        {
            return null;
        }

        station.Rename(name);
        station.ChangeType(type);

        switch (lifecycle)
        {
            case StationLifecycle.Enabled:
                station.Enable();
                break;
            case StationLifecycle.Maintenance:
                station.EnterMaintenance();
                break;
            case StationLifecycle.Disabled:
                station.Disable();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(lifecycle));
        }

        await store.SaveChangesAsync(cancellationToken);
        return station;
    }

    public async Task<bool> DisableAsync(
        Guid stationId,
        CancellationToken cancellationToken)
    {
        var station = await store.GetStationAsync(stationId, cancellationToken);

        if (station is null)
        {
            return false;
        }

        station.Disable();
        await store.SaveChangesAsync(cancellationToken);
        return true;
    }
}
