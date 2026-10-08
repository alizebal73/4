namespace GameNet.Application.Abstractions;

public interface IClock { DateTimeOffset UtcNow { get; } }
