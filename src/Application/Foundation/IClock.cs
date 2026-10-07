namespace GameNet.Application.Foundation;

public interface IClock
{
    DateTimeOffset UtcNow { get; }
}
