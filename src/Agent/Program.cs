var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "GameNet Manager Agent";
});

builder.Services.AddHostedService<GameNet.Agent.Worker>();

var host = builder.Build();
await host.RunAsync();
