using GameNet.Server.Api;
using GameNet.Server.Host;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGameNetServer();

var app = builder.Build();

app.MapGameNetEndpoints();

app.Run();

public partial class Program
{
}
