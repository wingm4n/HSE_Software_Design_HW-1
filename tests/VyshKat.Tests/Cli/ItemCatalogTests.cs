using VyshKat.Cli;
using VyshKat.Domain.Entities.Things;
using VyshKat.Domain.Entities.Transports;
using VyshKat.Domain.Interfaces;


namespace VyshKat.Tests.Cli;


public class ItemCatalogTests
{
    [Fact]
    public void Titles_AreUnique_WithinEachCatalog()
    {
        Assert.Equal(ItemCatalog.Transports.Count, ItemCatalog.Transports.Select(e => e.Title).Distinct().Count());
        Assert.Equal(ItemCatalog.Things.Count, ItemCatalog.Things.Select(e => e.Title).Distinct().Count());
    }


    [Theory]
    [InlineData("Scooter", typeof(Scooter), "7", "0.9")]
    [InlineData("E-Bike", typeof(EBike), "6", "1.5")]
    [InlineData("Bicycle", typeof(Bicycle), "9")]
    public void TransportEntry_AsksForItsFields_AndCreatesTheRightType(string title, Type expected, params string[] answers)
    {
        var h = ConsoleHarness.WithInput(answers);
        var entry = ItemCatalog.Transports.Single(e => e.Title == title);


        var transport = entry.Create(h.Io, "T-1");


        Assert.IsType(expected, transport);
        Assert.Equal("T-1", transport.InventoryNumber);
        Assert.Equal(int.Parse(answers[0]), transport.SimplicityForBeginner);
    }


    [Theory]
    [InlineData("Helmet", typeof(Helmet), "M")]
    [InlineData("Knee pads", typeof(KneePads), "L")]
    [InlineData("Elbow pads", typeof(ElbowPads), "S")]
    [InlineData("Charger", typeof(Charger), "1.2")]
    [InlineData("Charging cabinet", typeof(ChargingCabinet), "8", "0.8")]
    [InlineData("Dock station", typeof(DockStation), "6", "0.4")]
    public void ThingEntry_AsksForItsFields_AndCreatesTheRightType(string title, Type expected, params string[] answers)
    {
        var h = ConsoleHarness.WithInput(answers);
        var entry = ItemCatalog.Things.Single(e => e.Title == title);


        var thing = entry.Create(h.Io, "X-1");


        Assert.IsType(expected, thing);
        Assert.Equal("X-1", thing.InventoryNumber);
    }


    [Fact]
    public void EnergyConsumingEntries_ProduceEnergyConsumersWithTypedValue()
    {
        var h = ConsoleHarness.WithInput("5", "2,5");


        var scooter = ItemCatalog.Transports.Single(e => e.Title == "Scooter").Create(h.Io, "S-1");


        Assert.Equal(2.5, Assert.IsAssignableFrom<IEnergyConsumer>(scooter).DailyEnergyConsumptionKWh);
    }


    [Fact]
    public void CatalogEntry_RepromptsOnInvalidAnswer()
    {
        var h = ConsoleHarness.WithInput("99", "-3", "4", "1");


        var scooter = ItemCatalog.Transports.Single(e => e.Title == "Scooter").Create(h.Io, "S-1");


        // "99" is not a valid simplicity, "-3" is not a valid energy: the entry keeps asking.
        Assert.Equal(4, scooter.SimplicityForBeginner);
        Assert.Contains("Enter an integer", h.Text);
    }
}
