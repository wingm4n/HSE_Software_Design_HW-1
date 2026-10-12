using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using VyshKat.Application.Services;
using VyshKat.Cli;
using VyshKat.Domain.Interfaces;

// Stable number formatting ("0.9", not "0,9") regardless of the OS locale.
CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

// Composition root: the only place that knows concrete implementations.
var services = new ServiceCollection();

services.AddSingleton<TextReader>(Console.In);
services.AddSingleton<TextWriter>(Console.Out);

services.AddSingleton<IServiceCenter, ServiceCenter>();
services.AddSingleton<FleetService>();

services.AddSingleton<ConsoleIO>();
services.AddSingleton<FleetReportPrinter>();
services.AddTransient<DemoScenario>();
services.AddTransient<ConsoleApp>();

using var provider = services.BuildServiceProvider();

provider.GetRequiredService<ConsoleApp>().Run();
