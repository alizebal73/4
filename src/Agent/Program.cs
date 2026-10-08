using GameNet.Agent.Provisioning;
using GameNet.Agent.Security;

var identityStore = new DeviceIdentityStore();
var deviceId = identityStore.GetOrCreate();

if (args.Length > 0 &&
    string.Equals(args[0], "--device-id", StringComparison.OrdinalIgnoreCase))
{
    Console.WriteLine(deviceId);
    return;
}

if (args.Length > 0 &&
    string.Equals(args[0], "--pair", StringComparison.OrdinalIgnoreCase))
{
    if (args.Length < 4)
    {
        Console.Error.WriteLine(
            "Usage: GameNet.Manager.Agent.exe --pair <serverUrl> <pairingCode> <displayName>");
        Environment.ExitCode = 2;
        return;
    }

    var credentialStore = new AgentCredentialStore();
    var provisioner = new AgentProvisioner(credentialStore);

    var paired = await provisioner.PairAsync(
        args[1],
        deviceId,
        args[2],
        args[3],
        CancellationToken.None);

    Console.WriteLine(paired
        ? $"Agent {deviceId} paired successfully."
        : $"Agent {deviceId} pairing failed.");

    Environment.ExitCode = paired ? 0 : 1;
    return;
}

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "GameNet Manager Agent";
});

builder.Services.AddSingleton(identityStore);
builder.Services.AddSingleton<AgentCredentialStore>();
builder.Services.AddHostedService<GameNet.Agent.Worker>();

var host = builder.Build();
await host.RunAsync();
