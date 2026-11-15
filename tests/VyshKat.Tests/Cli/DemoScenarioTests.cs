using VyshKat.Application.Services;
using VyshKat.Cli;
using VyshKat.Domain.Interfaces;


namespace VyshKat.Tests.Cli;


public class DemoScenarioTests
{
    private static string RunDemo(IServiceCenter? inspection = null)
    {
        var h = ConsoleHarness.WithInput();
        var demo = new DemoScenario(inspection ?? new ServiceCenter(), h.Io, new FleetReportPrinter(h.Io));


        demo.Run();


        return h.Text;
    }


    [Fact]
    public void Constructor_RejectsNulls()
    {
        var h = ConsoleHarness.WithInput();
        var printer = new FleetReportPrinter(h.Io);
        var center = new FakeServiceCenter(true);


        Assert.Throws<ArgumentNullException>(() => new DemoScenario(null!, h.Io, printer));
        Assert.Throws<ArgumentNullException>(() => new DemoScenario(center, null!, printer));
        Assert.Throws<ArgumentNullException>(() => new DemoScenario(center, h.Io, null!));
    }


    [Fact]
    public void Run_WithRealServiceCenter_ShowsEveryScenario()
    {
        var text = RunDemo();


        Assert.Contains("ACCEPTED: Scooter [S-001]", text);                    // accepted by inspection
        Assert.Contains("REJECTED by service center: E-Bike [E-666]", text);   // rejected by inspection
        Assert.Contains("Added: Helmet [H-001]", text);                        // things on balance
        Assert.Contains("REJECTED (inventory number already in use)", text);   // uniqueness
        Assert.Contains("Repairing a damaged Helmet", text);                   // polymorphism
        Assert.Contains("Inventory number cannot be empty", text);             // validation
        Assert.Contains("=== Fleet report ===", text);                         // reports
        Assert.Contains("=== Equipment suitable for beginners", text);
        Assert.Contains("=== Inventory list ===", text);
        Assert.EndsWith("##### DEMO finished #####" + Environment.NewLine, text);
    }


    [Fact]
    public void Run_WithApprovingFake_AcceptsEvenTheFaultyVehicle()
    {
        var inspection = new FakeServiceCenter(decision: true);


        var text = RunDemo(inspection: inspection);


        Assert.Contains("ACCEPTED: E-Bike [E-666]", text);
        Assert.DoesNotContain("REJECTED by service center", text);
        Assert.Equal(6, inspection.Inspected.Count); // 5 vehicles + E-666; the duplicate is refused before the inspection
    }


    [Fact]
    public void Run_WithDecliningFake_AcceptsNoTransport_ButStillPutsThingsOnBalance()
    {
        var inspection = new FakeServiceCenter(decision: false);


        var text = RunDemo(inspection: inspection);


        Assert.DoesNotContain("ACCEPTED:", text);
        Assert.Contains("REJECTED by service center: Scooter [S-001]", text);
        Assert.Contains("Added: Helmet [H-001]", text);
    }


    [Fact]
    public void Run_UsesTheInjectedInspection_NotAHardcodedOne()
    {
        var inspection = new FakeServiceCenter(decision: true);


        RunDemo(inspection: inspection);


        Assert.NotEmpty(inspection.Inspected);
    }
}
