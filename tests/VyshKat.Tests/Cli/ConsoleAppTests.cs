using VyshKat.Application.Services;
using VyshKat.Cli;
using VyshKat.Domain.Entities.Things;
using VyshKat.Domain.Entities.Transports;
using VyshKat.Domain.Interfaces;


namespace VyshKat.Tests.Cli;


/// <summary>
/// Drives the whole interactive menu with scripted input.
/// The inspection is replaced by a fake (or by the real <see cref="ServiceCenter"/>) through the constructor.
/// </summary>
public class ConsoleAppTests
{
    private sealed class Session
    {
        public required ConsoleHarness Console { get; init; }
        public required FleetService Fleet { get; init; }
        public required IServiceCenter Inspection { get; init; }
        public string Text => Console.Text;
    }


    /// <summary>Builds the app by hand (same wiring as the DI container) and runs it until the script ends.</summary>
    private static Session Run(IServiceCenter inspection, params string[] script)
    {
        var console = ConsoleHarness.WithInput(script);
        var fleet = new FleetService(inspection);
        var printer = new FleetReportPrinter(console.Io);
        var demo = new DemoScenario(inspection, console.Io, printer);


        new ConsoleApp(fleet, console.Io, printer, demo).Run();


        return new Session { Console = console, Fleet = fleet, Inspection = inspection };
    }


    // ---------- construction ----------


    [Fact]
    public void Constructor_RejectsNulls()
    {
        var h = ConsoleHarness.WithInput();
        var fleet = new FleetService(new FakeServiceCenter(true));
        var printer = new FleetReportPrinter(h.Io);
        var demo = new DemoScenario(new FakeServiceCenter(true), h.Io, printer);


        Assert.Throws<ArgumentNullException>(() => new ConsoleApp(null!, h.Io, printer, demo));
        Assert.Throws<ArgumentNullException>(() => new ConsoleApp(fleet, null!, printer, demo));
        Assert.Throws<ArgumentNullException>(() => new ConsoleApp(fleet, h.Io, null!, demo));
        Assert.Throws<ArgumentNullException>(() => new ConsoleApp(fleet, h.Io, printer, null!));
    }


    // ---------- menu loop ----------


    [Fact]
    public void Run_ShowsMenu_AndExitsOnZero()
    {
        var s = Run(new FakeServiceCenter(true), "0");


        Assert.Contains("Main menu:", s.Text);
        Assert.Contains("0. Exit", s.Text);
        Assert.Contains("Goodbye!", s.Text);
        Assert.DoesNotContain("Input closed.", s.Text);
    }


    [Fact]
    public void Run_ExitsCleanly_WhenInputIsClosed()
    {
        var s = Run(new FakeServiceCenter(true) /* no lines at all */);


        Assert.Contains("Input closed.", s.Text);
        Assert.Contains("Goodbye!", s.Text);
    }


    [Theory]
    [InlineData("abc")]
    [InlineData("7")]
    [InlineData("-1")]
    [InlineData("cancel")] // "cancel" has no meaning in the main menu
    public void Run_AsksAgain_OnInvalidMenuChoice(string bad)
    {
        var s = Run(new FakeServiceCenter(true), bad, "0");


        Assert.Contains("Enter an integer from 0 to 6.", s.Text);
        Assert.Contains("Goodbye!", s.Text);
    }


    // ---------- 1. accept transport ----------


    [Fact]
    public void AcceptTransport_AddsScooter_WhenInspectionPasses()
    {
        var inspection = new FakeServiceCenter(decision: true);


        var s = Run(inspection, "1", "1", "S-001", "7", "0.9", "0");


        var scooter = Assert.IsType<Scooter>(Assert.Single(s.Fleet.AllTransports));
        Assert.Equal("S-001", scooter.InventoryNumber);
        Assert.Equal(7, scooter.SimplicityForBeginner);
        Assert.Equal(0.9, scooter.DailyEnergyConsumptionKWh);
        Assert.Same(scooter, Assert.Single(inspection.Inspected)); // the fake was really consulted
        Assert.Contains("Inspection passed", s.Text);
    }


    [Fact]
    public void AcceptTransport_DoesNotAdd_WhenInspectionDeclines()
    {
        var inspection = new FakeServiceCenter(decision: false);


        var s = Run(inspection, "1", "3", "B-001", "9", "0");


        Assert.Empty(s.Fleet.AllTransports);
        Assert.Single(inspection.Inspected);
        Assert.Contains("Inspection failed", s.Text);
        Assert.Contains("was NOT accepted", s.Text);
    }


    [Fact]
    public void AcceptTransport_WithRealServiceCenter_DeclinesFaultyEBike()
    {
        // 5 kWh is above ServiceCenter.MaxDailyEnergyKWh: the user can trigger a rejection by typing such a value.
        var s = Run(new ServiceCenter(), "1", "2", "E-001", "8", "5", "0");


        Assert.Empty(s.Fleet.AllTransports);
        Assert.Contains("Inspection failed", s.Text);
        Assert.Contains(ServiceCenter.MaxDailyEnergyKWh.ToString(System.Globalization.CultureInfo.InvariantCulture), s.Text);
    }


    [Fact]
    public void AcceptTransport_WithRealServiceCenter_AcceptsNormalEBike()
    {
        var s = Run(new ServiceCenter(), "1", "2", "E-001", "8", "1.5", "0");


        Assert.Single(s.Fleet.AllTransports);
        Assert.Contains("Inspection passed", s.Text);
    }


    [Fact]
    public void AcceptTransport_AsksAgain_WhenInventoryNumberIsTaken()
    {
        var inspection = new FakeServiceCenter(true);


        // Add B-001, then try B-001 again (case-insensitive), then give a free number.
        var s = Run(inspection,
            "1", "3", "B-001", "9",
            "1", "3", "b-001", "B-002", "5",
            "0");


        Assert.Equal(new[] { "B-001", "B-002" }, s.Fleet.AllTransports.Select(t => t.InventoryNumber));
        Assert.Contains("'b-001' is already in use", s.Text);
        Assert.Equal(2, inspection.Inspected.Count); // the duplicate never reached the inspection
    }


    [Fact]
    public void AcceptTransport_AsksAgain_OnInvalidFieldValues()
    {
        // simplicity 11 and negative energy are refused by the prompt, then valid values are accepted
        var s = Run(new FakeServiceCenter(true), "1", "1", "S-001", "11", "7", "-1", "0.5", "0");


        Assert.Single(s.Fleet.AllTransports);
        Assert.Contains("Enter an integer from 1 to 10.", s.Text);
        Assert.Contains("Enter a non-negative number", s.Text);
    }


    [Fact]
    public void AcceptTransport_CanBeCancelled_AndLeavesFleetUntouched()
    {
        var inspection = new FakeServiceCenter(true);


        var s = Run(inspection, "1", "1", "S-001", "cancel", "0");


        Assert.Empty(s.Fleet.AllItems);
        Assert.Empty(inspection.Inspected);
        Assert.Contains("Cancelled, nothing was changed.", s.Text);
        Assert.Contains("Goodbye!", s.Text); // the app keeps running after a cancel
    }


    // ---------- 2. things ----------


    [Fact]
    public void PutThing_AddsHelmet_WithoutInspection()
    {
        var inspection = new FakeServiceCenter(decision: false); // would decline everything - must not matter for things


        var s = Run(inspection, "2", "1", "H-001", "M", "0");


        var helmet = Assert.IsType<Helmet>(Assert.Single(s.Fleet.AllThings));
        Assert.Equal("M", helmet.Size);
        Assert.Empty(inspection.Inspected);
        Assert.Contains("Put on balance", s.Text);
    }


    [Fact]
    public void PutThing_AddsChargingCabinet_WithBeginnerRating()
    {
        var s = Run(new FakeServiceCenter(true), "2", "5", "CC-001", "8", "0.8", "0");


        var cabinet = Assert.IsType<ChargingCabinet>(Assert.Single(s.Fleet.AllThings));
        Assert.Equal(8, cabinet.SimplicityForBeginner);
        Assert.Equal(0.8, cabinet.DailyEnergyConsumptionKWh);
    }


    [Fact]
    public void PutThing_AsksAgain_WhenNumberIsUsedByTransport()
    {
        var s = Run(new FakeServiceCenter(true),
            "1", "3", "X-001", "9",
            "2", "1", "X-001", "H-001", "L",
            "0");


        Assert.Single(s.Fleet.AllTransports);
        Assert.Equal("H-001", Assert.Single(s.Fleet.AllThings).InventoryNumber);
        Assert.Contains("'X-001' is already in use", s.Text);
    }


    [Fact]
    public void PutThing_CanBeCancelled()
    {
        var s = Run(new FakeServiceCenter(true), "2", "1", "cancel", "0");


        Assert.Empty(s.Fleet.AllItems);
        Assert.Contains("Cancelled, nothing was changed.", s.Text);
    }


    // ---------- 3-5. reports ----------


    [Fact]
    public void Reports_ShowDataEnteredInTheSession()
    {
        var s = Run(new ServiceCenter(),
            "1", "1", "S-001", "7", "0.9",   // scooter, beginner friendly
            "1", "3", "B-001", "4",          // bicycle, not beginner friendly
            "2", "4", "C-001", "1.2",        // charger
            "3", "4", "5", "0");


        Assert.Contains("=== Fleet report ===", s.Text);
        Assert.Contains("Total units on balance : 3", s.Text);
        Assert.Contains("Total daily energy     : 2.10 kWh", s.Text);


        Assert.Contains("=== Equipment suitable for beginners", s.Text);
        var beginnerPart = s.Text[s.Text.IndexOf("=== Equipment suitable", StringComparison.Ordinal)..];
        beginnerPart = beginnerPart[..beginnerPart.IndexOf("=== Inventory list", StringComparison.Ordinal)];
        Assert.Contains("S-001", beginnerPart);
        Assert.DoesNotContain("B-001", beginnerPart);


        var inventoryPart = s.Text[s.Text.IndexOf("=== Inventory list", StringComparison.Ordinal)..];
        Assert.Contains("S-001", inventoryPart);
        Assert.Contains("B-001", inventoryPart);
        Assert.Contains("C-001", inventoryPart);
    }


    [Fact]
    public void Reports_OnEmptyFleet_SayThatItIsEmpty()
    {
        var s = Run(new FakeServiceCenter(true), "3", "4", "5", "0");


        Assert.Contains("(the fleet is empty)", s.Text);
        Assert.Contains("(none)", s.Text);
    }


    // ---------- 6. demo ----------


    [Fact]
    public void Demo_RunsOnIsolatedFleet_AndDoesNotTouchUserData()
    {
        var s = Run(new ServiceCenter(),
            "1", "3", "MY-1", "9", // user's own bicycle
            "6",                   // demo uses its own FleetService
            "0");


        Assert.Contains("##### DEMO finished #####", s.Text);
        var mine = Assert.Single(s.Fleet.AllItems);
        Assert.Equal("MY-1", mine.InventoryNumber);
    }


    [Fact]
    public void Demo_CanBeRunTwice_BecauseItDoesNotKeepState()
    {
        var s = Run(new ServiceCenter(), "6", "6", "0");


        var first = s.Text.IndexOf("##### DEMO finished #####", StringComparison.Ordinal);
        var second = s.Text.IndexOf("##### DEMO finished #####", first + 1, StringComparison.Ordinal);
        Assert.True(first >= 0 && second > first);
        // Duplicate numbers in the demo are refused on purpose (step 4), but the first step must succeed both times.
        Assert.Equal(2, CountOccurrences(s.Text, "ACCEPTED: Scooter [S-001]"));
    }


    private static int CountOccurrences(string text, string value)
    {
        var count = 0;
        for (var i = text.IndexOf(value, StringComparison.Ordinal); i >= 0; i = text.IndexOf(value, i + 1, StringComparison.Ordinal))
            count++;
        return count;
    }
}
