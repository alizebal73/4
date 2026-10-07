using GameNet.Application.Foundation;

namespace GameNet.Infrastructure.Foundation;

public sealed class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
