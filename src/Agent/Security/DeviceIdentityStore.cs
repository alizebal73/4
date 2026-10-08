namespace GameNet.Agent.Security;

/// <summary>
/// Stable machine identity used by the Agent. It is deliberately separate from
/// IP address and SignalR ConnectionId; those values can change on reconnect.
/// </summary>
public sealed class DeviceIdentityStore
{
    private readonly string _path;

    public DeviceIdentityStore()
    {
        var root = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "GameNet Manager",
            "Agent");

        Directory.CreateDirectory(root);
        _path = Path.Combine(root, "device-id.txt");
    }

    public string GetOrCreate()
    {
        try
        {
            var existing = File.Exists(_path)
                ? File.ReadAllText(_path).Trim()
                : string.Empty;

            if (!string.IsNullOrWhiteSpace(existing))
            {
                return existing;
            }

            var created = $"agent-{Guid.NewGuid():N}";

            using var stream = new FileStream(
                _path,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.Read);

            using var writer = new StreamWriter(stream);
            writer.Write(created);
            writer.Flush();

            return created;
        }
        catch (IOException)
        {
            var existing = File.Exists(_path)
                ? File.ReadAllText(_path).Trim()
                : string.Empty;

            if (!string.IsNullOrWhiteSpace(existing))
            {
                return existing;
            }

            throw;
        }
    }
}
