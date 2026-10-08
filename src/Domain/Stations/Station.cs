namespace GameNet.Domain.Stations;

public sealed class Station
{
    private Station() { }

    private Station(
        Guid id,
        int number,
        string name,
        StationType type,
        DateTimeOffset createdAt)
    {
        Id = id;
        Number = number;
        Name = name;
        Type = type;
        Lifecycle = StationLifecycle.Enabled;
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }
    public int Number { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public StationType Type { get; private set; }
    public StationLifecycle Lifecycle { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public static Station Create(
        int number,
        string name,
        StationType type,
        DateTimeOffset createdAt)
    {
        if (number <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(number));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Station name is required.", nameof(name));
        }

        return new Station(
            Guid.NewGuid(),
            number,
            name.Trim(),
            type,
            createdAt);
    }

    public void EnterMaintenance()
    {
        if (Lifecycle == StationLifecycle.Disabled)
        {
            throw new InvalidOperationException("Disabled stations cannot enter maintenance.");
        }

        Lifecycle = StationLifecycle.Maintenance;
    }

    public void Enable() => Lifecycle = StationLifecycle.Enabled;

    public void Disable() => Lifecycle = StationLifecycle.Disabled;
}
