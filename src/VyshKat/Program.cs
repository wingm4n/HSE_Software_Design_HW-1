using Microsoft.Extensions.DependencyInjection;
//using VyshKat.Application.Services;
//using VyshKat.Domain.Interfaces;
using VyshKat.Cli;

var services = new ServiceCollection();
services.AddTransient<ConsoleApp>();
var provider = services.BuildServiceProvider();

var app = provider.GetRequiredService<ConsoleApp>();
app.Run();