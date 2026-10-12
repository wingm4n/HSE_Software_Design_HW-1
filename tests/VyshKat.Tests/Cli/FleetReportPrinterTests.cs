using VyshKat.Application.Services;
using VyshKat.Cli;
using VyshKat.Domain.Entities.Things;
using VyshKat.Domain.Entities.Transports;


namespace VyshKat.Tests.Cli;


public class FleetReportPrinterTests
{
    private static FleetService NewFleet() => new(new FakeServiceCenter(true));


    private static FleetService SampleFleet()
    {
        var fleet = NewFleet();
        fleet.AddTransport(new Scooter("S-001", 7, 0.9));
        fleet.AddTransport(new Scooter("S-002", 4, 1.1));
        fleet.AddTransport(new Bicycle("B-001", 9));
        fleet.AddThing(new Helmet("H-001", "M"));
        fleet.AddThing(new Charger("CH-001", 1.0));
        fleet.AddThing(new ChargingCabinet("CC-001", 6, 0.5));
        return fleet;
    }


    [Fact]
    public void Constructor_RejectsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new FleetReportPrinter(null!));
    }


    // ---------- summary ----------


    [Fact]
    public void PrintSummary_OnEmptyFleet_ShowsZeros()
    {
        var h = ConsoleHarness.WithInput();


        new FleetReportPrinter(h.Io).PrintSummary(NewFleet());


        Assert.Contains("Total units on balance : 0", h.Text);
        Assert.Contains("Total daily energy     : 0.00 kWh", h.Text);
        Assert.Contains("(the fleet is empty)", h.Text);
    }


    [Fact]
    public void PrintSummary_ShowsCountsAndTotalEnergy()
    {
        var h = ConsoleHarness.WithInput();


        new FleetReportPrinter(h.Io).PrintSummary(SampleFleet());


        Assert.Contains("Total units on balance : 6", h.Text);
        Assert.Contains("transport            : 3", h.Text);
        Assert.Contains("things               : 3", h.Text);
        Assert.Contains("Total daily energy     : 3.50 kWh", h.Text); // 0.9 + 1.1 + 1.0 + 0.5
    }


    [Fact]
    public void PrintSummary_GroupsByTypeWithPerTypeEnergy()
    {
        var h = ConsoleHarness.WithInput();


        new FleetReportPrinter(h.Io).PrintSummary(SampleFleet());


        var scooterRow = h.Text.Split('\n').Single(l => l.StartsWith("Scooter"));
        Assert.Contains("2", scooterRow);
        Assert.Contains("2.00", scooterRow);


        var bicycleRow = h.Text.Split('\n').Single(l => l.StartsWith("Bicycle"));
        Assert.Contains("0.00", bicycleRow); // bicycles consume nothing
    }


    // ---------- beginner list ----------


    [Fact]
    public void PrintBeginnerFriendly_ShowsThresholdAndOnlySuitableItems()
    {
        var h = ConsoleHarness.WithInput();


        new FleetReportPrinter(h.Io).PrintBeginnerFriendly(SampleFleet());


        Assert.Contains($"simplicity >= {FleetService.BeginnerSimplicityLimit}", h.Text);
        Assert.Contains("S-001", h.Text);   // 7
        Assert.Contains("B-001", h.Text);   // 9
        Assert.Contains("CC-001", h.Text);  // 6 - the boundary value is included
        Assert.DoesNotContain("S-002", h.Text); // 4
        Assert.DoesNotContain("H-001", h.Text); // helmets have no rating
        Assert.DoesNotContain("CH-001", h.Text); // chargers have no rating
    }


    [Fact]
    public void PrintBeginnerFriendly_WhenNobodyQualifies_PrintsNone()
    {
        var fleet = NewFleet();
        fleet.AddTransport(new Scooter("S-002", 5, 1.0));
        var h = ConsoleHarness.WithInput();


        new FleetReportPrinter(h.Io).PrintBeginnerFriendly(fleet);


        Assert.Contains("(none)", h.Text);
        Assert.DoesNotContain("S-002", h.Text);
    }


    // ---------- inventory ----------


    [Fact]
    public void PrintInventory_ListsEveryTransportAndThing_InTwoSections()
    {
        var h = ConsoleHarness.WithInput();


        new FleetReportPrinter(h.Io).PrintInventory(SampleFleet());


        var transportAt = h.Text.IndexOf("-- Transport --", StringComparison.Ordinal);
        var thingsAt = h.Text.IndexOf("-- Things --", StringComparison.Ordinal);
        Assert.True(transportAt >= 0 && thingsAt > transportAt);


        foreach (var number in new[] { "S-001", "S-002", "B-001", "H-001", "CH-001", "CC-001" })
            Assert.Contains(number, h.Text);


        Assert.True(h.Text.IndexOf("B-001", StringComparison.Ordinal) < thingsAt);  // transport before the "Things" header
        Assert.True(h.Text.IndexOf("H-001", StringComparison.Ordinal) > thingsAt);  // things after it
    }


    [Fact]
    public void PrintInventory_OnEmptyFleet_SaysSo()
    {
        var h = ConsoleHarness.WithInput();


        new FleetReportPrinter(h.Io).PrintInventory(NewFleet());


        Assert.Contains("(the fleet is empty)", h.Text);
        Assert.DoesNotContain("-- Transport --", h.Text);
    }


    [Fact]
    public void PrintInventory_WhenOneSectionIsEmpty_MarksItAsNone()
    {
        var fleet = NewFleet();
        fleet.AddThing(new Helmet("H-001", "M"));
        var h = ConsoleHarness.WithInput();


        new FleetReportPrinter(h.Io).PrintInventory(fleet);


        var transportPart = h.Text[h.Text.IndexOf("-- Transport --", StringComparison.Ordinal)..h.Text.IndexOf("-- Things --", StringComparison.Ordinal)];
        Assert.Contains("(none)", transportPart);
        Assert.Contains("H-001", h.Text);
    }
}
