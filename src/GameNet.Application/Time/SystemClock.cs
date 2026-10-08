using GameNet.Application.Abstractions;

namespace GameNet.Application.Time;

public sealed class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
