using Microsoft.Extensions.DependencyInjection;
using VyshKat.Application.Services;
using VyshKat.Cli;
using VyshKat.Domain.Interfaces;


namespace VyshKat.Tests.Cli;


/// <summary>
/// Shows that the inspection can be replaced through the DI container without touching any other class.
/// The registrations mirror Program.cs; the only difference is the replaced <see cref="IServiceCenter"/>.
/// </summary>
public class CompositionTests
{
    private static ServiceProvider Build(IServiceCenter serviceCenter, TextReader input, TextWriter output)
    {
        var services = new ServiceCollection();
        services.AddSingleton(input);
        services.AddSingleton(output);
        services.AddSingleton(serviceCenter); // <- the substitution point (production: ServiceCenter)
        services.AddSingleton<FleetService>();
        services.AddSingleton<ConsoleIO>();
        services.AddSingleton<FleetReportPrinter>();
        services.AddTransient<DemoScenario>();
        services.AddTransient<ConsoleApp>();
        return services.BuildServiceProvider();
    }


    [Fact]
    public void Container_ResolvesWholeGraph_AndUsesSubstitutedInspection()
    {
        var fake = new FakeServiceCenter(decision: false);
        var output = new StringWriter();
        using var provider = Build(fake, new StringReader("1\n3\nB-001\n9\n0\n"), output);


        provider.GetRequiredService<ConsoleApp>().Run();


        Assert.Single(fake.Inspected);
        Assert.Empty(provider.GetRequiredService<FleetService>().AllTransports);
        Assert.Contains("Inspection failed", output.ToString());
    }


    [Fact]
    public void FleetService_IsSingleton_SoMenuAndReportsShareOneFleet()
    {
        using var provider = Build(new FakeServiceCenter(true), new StringReader(""), new StringWriter());


        Assert.Same(provider.GetRequiredService<FleetService>(), provider.GetRequiredService<FleetService>());
    }


    [Fact]
    public void Container_InjectsTheSameInspection_IntoFleetAndDemo()
    {
        var fake = new FakeServiceCenter(true);
        var output = new StringWriter();
        using var provider = Build(fake, new StringReader(""), output);


        provider.GetRequiredService<DemoScenario>().Run();


        Assert.NotEmpty(fake.Inspected);
    }
}
