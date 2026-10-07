using GameNet.Agent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddWindowsService(options => options.ServiceName = "GameNet Manager Agent");
builder.Services.AddHostedService<Worker>();

await builder.Build().RunAsync();
