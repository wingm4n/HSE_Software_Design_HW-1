using Moq;
using VyshKat.Application.Services;
using VyshKat.Domain.Entities.Things;
using VyshKat.Domain.Entities.Transports;
using VyshKat.Domain.Interfaces;
using Xunit;

namespace VyshKat.Tests.Application;

public class FleetServiceTests
{

    [Fact]
    public void AddTransport_AddsVehicle_WhenInspectionApproves()
    {
        var serviceCenter = new Mock<IServiceCenter>();
        serviceCenter.Setup(sc => sc.Inspect(It.IsAny<Transport>())).Returns(true);

        var fleet = new FleetService(serviceCenter.Object);
        var scooter = new Scooter("S-001", simplicityForBeginner: 7, dailyEnergyConsumptionKWh: 0.9);

        var added = fleet.AddTransport(scooter);

        Assert.True(added);
        Assert.Single(fleet.AllTransports);
        serviceCenter.Verify(sc => sc.Inspect(scooter), Times.Once);
    }

    [Fact]
    public void AddTransport_RejectsVehicle_WhenInspectionDeclines()
    {
        var serviceCenter = new Mock<IServiceCenter>();
        serviceCenter.Setup(sc => sc.Inspect(It.IsAny<Transport>())).Returns(false);

        var fleet = new FleetService(serviceCenter.Object);
        var scooter = new Scooter("S-002", simplicityForBeginner: 8, dailyEnergyConsumptionKWh: 0.5);

        var added = fleet.AddTransport(scooter);

        Assert.False(added);
        Assert.Empty(fleet.AllTransports);
    }

    [Fact]
    public void AllTransports_AllThings_ReturnsCorrect_WhenMixedQuery()
    {
        var serviceCenter = new Mock<IServiceCenter>();
        serviceCenter.Setup(sc => sc.Inspect(It.IsAny<Transport>())).Returns(true);

        var fleet = new FleetService(serviceCenter.Object);
        List<IInventoriable> expectedTransports = new List<IInventoriable>();

        var scooter = new Scooter("S-002", simplicityForBeginner: 8, dailyEnergyConsumptionKWh: 0.5);
        expectedTransports.Add(scooter);
        var added = fleet.AddTransport(scooter);
        Assert.True(added);

        var charger = new Charger("C-001", dailyEnergyConsumptionKWh: 0.5);
        fleet.AddThing(charger);

        var bicycle = new Bicycle("B-001", simplicityForBeginner: 8);
        expectedTransports.Add(bicycle);
        added = fleet.AddTransport(bicycle);
        Assert.True(added);

        Assert.Single(fleet.AllThings);
        Assert.Equal(expectedTransports.AsReadOnly(), fleet.AllTransports);

    }

    // ---------- Energy aggregation ----------

    [Fact]
    public void TotalDailyEnergy_Sums_AllEnergyConsumers_And_SkipsBicycles()
    {
        var serviceCenter = new Mock<IServiceCenter>();
        serviceCenter.Setup(sc => sc.Inspect(It.IsAny<Transport>())).Returns(true);

        var fleet = new FleetService(serviceCenter.Object);

        fleet.AddTransport(new Scooter("S-001", 7, dailyEnergyConsumptionKWh: 1.0));
        fleet.AddTransport(new EBike("E-001", 5, dailyEnergyConsumptionKWh: 2.0));
        fleet.AddTransport(new Bicycle("B-001", 9)); // no IEnergyConsumer
        fleet.AddThing(new Charger("C-001", dailyEnergyConsumptionKWh: 0.5));

        Assert.Equal(3.5, fleet.TotalDailyEnergyKWh, precision: 6);
    }

    // ---------- Beginner rule ----------

    [Theory]
    [InlineData(5, false)]
    [InlineData(6, true)]
    [InlineData(7, true)]
    public void BeginnerFriendlyItems_Use_Threshold_Six(int simplicity, bool expected)
    {
        var serviceCenter = new Mock<IServiceCenter>();
        serviceCenter.Setup(sc => sc.Inspect(It.IsAny<Transport>())).Returns(true);

        var fleet = new FleetService(serviceCenter.Object);
        fleet.AddTransport(new Scooter("S-001", simplicity, dailyEnergyConsumptionKWh: 0.5));

        var result = fleet.BeginnerFriendlyItems;

        Assert.Equal(expected, result.Any());
    }


    [Fact]
    public void BeginnerFriendlyItems_IncludesThingsWithRating_AndSkipsOthers()
    {
        var serviceCenter = new Mock<IServiceCenter>();
        serviceCenter.Setup(sc => sc.Inspect(It.IsAny<Transport>())).Returns(true);

        var fleet = new FleetService(serviceCenter.Object);
        fleet.AddTransport(new Bicycle("B-001", simplicityForBeginner: 9));
        fleet.AddTransport(new Bicycle("B-002", simplicityForBeginner: 5));
        fleet.AddThing(new ChargingCabinet("CC-001", simplicityForBeginner: 6, dailyEnergyConsumptionKWh: 0.5));
        fleet.AddThing(new ChargingCabinet("CC-002", simplicityForBeginner: 2, dailyEnergyConsumptionKWh: 0.5));
        fleet.AddThing(new Helmet("H-001", "M")); // no beginner rating at all

        var ids = fleet.BeginnerFriendlyItems.Select(i => i.InventoryNumber).ToList();

        Assert.Equal(new[] { "B-001", "CC-001" }, ids);
    }

    // ---------- Inventory number uniqueness ----------

    [Fact]
    public void AddTransport_RejectsDuplicateInventoryNumber_WithoutCallingInspection()
    {
        var serviceCenter = new Mock<IServiceCenter>();
        serviceCenter.Setup(sc => sc.Inspect(It.IsAny<Transport>())).Returns(true);

        var fleet = new FleetService(serviceCenter.Object);
        fleet.AddTransport(new Scooter("S-001", 7, 0.9));

        var added = fleet.AddTransport(new Bicycle("S-001", 9));

        Assert.False(added);
        Assert.Equal(1, fleet.UnitCount);
        serviceCenter.Verify(sc => sc.Inspect(It.IsAny<Transport>()), Times.Once);
    }

    [Fact]
    public void AddThing_RejectsDuplicateInventoryNumber_AcrossTransportAndThings()
    {
        var serviceCenter = new Mock<IServiceCenter>();
        serviceCenter.Setup(sc => sc.Inspect(It.IsAny<Transport>())).Returns(true);

        var fleet = new FleetService(serviceCenter.Object);
        fleet.AddTransport(new Bicycle("X-001", 9));

        Assert.False(fleet.AddThing(new Helmet("X-001", "M")));
        Assert.True(fleet.AddThing(new Helmet("X-002", "M")));
        Assert.False(fleet.AddThing(new Charger("X-002", 0.5)));
        Assert.Equal(2, fleet.UnitCount);
    }

    [Theory]
    [InlineData("S-001")]
    [InlineData("s-001")]
    [InlineData("  S-001  ")]
    public void IsInventoryNumberTaken_IgnoresCaseAndOuterSpaces(string candidate)
    {
        var fleet = new FleetService(new ServiceCenter());
        fleet.AddTransport(new Scooter("S-001", 7, 0.9));

        Assert.True(fleet.IsInventoryNumberTaken(candidate));
        Assert.False(fleet.IsInventoryNumberTaken("S-002"));
    }

    [Fact]
    public void AddThing_ReturnsTrue_WhenNumberIsFree()
    {
        var fleet = new FleetService(new Mock<IServiceCenter>().Object);

        Assert.True(fleet.AddThing(new Helmet("H-001", "M")));
        Assert.Single(fleet.AllThings);
    }

    // ---------- Real inspection plugged into the fleet ----------

    [Fact]
    public void AddTransport_WithRealServiceCenter_DeclinesFaultyVehicle()
    {
        var fleet = new FleetService(new ServiceCenter());

        var accepted = fleet.AddTransport(new Scooter("S-001", 7, ServiceCenter.MaxDailyEnergyKWh + 0.1));

        Assert.False(accepted);
        Assert.Equal(0, fleet.UnitCount);
    }
}
