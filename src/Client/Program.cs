using GameNet.Shared.Primitives;

namespace GameNet.Agent;

internal static class Program
{
    public static void Main()
    {
        var deviceId = EntityId.New();
        Console.WriteLine($"GameNet Agent foundation started. Device identity placeholder: {deviceId}");
    }
}
