using GameNet.Agent.Provisioning;
using GameNet.Agent.Security;

if (args.Length > 0 &&
    string.Equals(args[0], "--pair", StringComparison.OrdinalIgnoreCase))
{
    if (args.Length < 5)
    {
        Console.Error.WriteLine(
            "Usage: GameNet.Manager.Agent.exe --pair <serverUrl> <deviceId> <pairingCode> <displayName>");
        return 2;
    }

    var credentialStore = new AgentCredentialStore();
    var provisioner = new AgentProvisioner(credentialStore);

    var paired = await provisioner.PairAsync(
        args[1],
        args[2],
        args[3],
        args[4],
        CancellationToken.None);

    Console.WriteLine(paired
        ? "Agent paired successfully."
        : "Agent pairing failed.");

    return paired ? 0 : 1;
}

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "GameNet Manager Agent";
});

builder.Services.AddSingleton<AgentCredentialStore>();
builder.Services.AddHostedService<GameNet.Agent.Worker>();

var host = builder.Build();
await host.RunAsync();
