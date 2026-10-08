using Xunit;

using GameNet.Domain.Stations;

namespace GameNet.Domain.Tests;

public sealed class StationTests
{
    [Fact]
    public void Disabled_Station_Cannot_Enter_Maintenance()
    {
        var station = Station.Create(
            1,
            "PC 01",
            StationType.Pc,
            DateTimeOffset.UtcNow);

        station.Disable();

        Assert.Throws<InvalidOperationException>(station.EnterMaintenance);
    }

    [Fact]
    public void Enabled_Station_Can_Enter_And_Exit_Maintenance()
    {
        var station = Station.Create(
            1,
            "PC 01",
            StationType.Pc,
            DateTimeOffset.UtcNow);

        station.EnterMaintenance();

        Assert.Equal(StationLifecycle.Maintenance, station.Lifecycle);

        station.Enable();

        Assert.Equal(StationLifecycle.Enabled, station.Lifecycle);
    }
}
