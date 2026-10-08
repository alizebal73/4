using GameNet.Domain.Stations;

namespace GameNet.Domain.Tests;

public sealed class StationTests
{
    [Fact]
    public void Disabled_Station_Cannot_Bind_Agent()
    {
        var station = Station.Create(
            1,
            "PC 01",
            StationType.Pc,
            DateTimeOffset.UtcNow);

        station.Disable();

        Assert.Throws<InvalidOperationException>(() =>
            station.BindAgent(Guid.NewGuid()));
    }

    [Fact]
    public void Bound_Station_Cannot_Be_Disabled()
    {
        var station = Station.Create(
            1,
            "PC 01",
            StationType.Pc,
            DateTimeOffset.UtcNow);

        station.BindAgent(Guid.NewGuid());

        Assert.Throws<InvalidOperationException>(station.Disable);
    }
}
