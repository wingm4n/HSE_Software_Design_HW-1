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

// Console streams are registered as dependencies, so the UI classes can be tested with in-memory streams.
services.AddSingleton<TextReader>(Console.In);
services.AddSingleton<TextWriter>(Console.Out);

// Inspection rules: replace this registration (or the implementation) to change them; FleetService and DemoScenario follow automatically.
services.AddSingleton<IServiceCenter, ServiceCenter>();
// One fleet for the whole session, shared by the menu and the reports.
services.AddSingleton<FleetService>();

// UI services.
services.AddSingleton<ConsoleIO>();
services.AddSingleton<FleetReportPrinter>();
// Short-lived objects; the demo creates its own isolated fleet on every run.
services.AddTransient<DemoScenario>();
services.AddTransient<ConsoleApp>();

// Build the container once; disposing it at the end of Main releases the registered services.
using var provider = services.BuildServiceProvider();

// Resolve the entry point: the container creates the whole dependency graph.
provider.GetRequiredService<ConsoleApp>().Run();
