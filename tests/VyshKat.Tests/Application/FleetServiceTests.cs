using Moq;
using VyshKat.Application.Services;
using VyshKat.Domain.Entities.Things;
using VyshKat.Domain.Entities.Transport;
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
    public void BeginnerFriendlyTransports_Use_Threshold_Six(int simplicity, bool expected)
    {
        var serviceCenter = new Mock<IServiceCenter>();
        serviceCenter.Setup(sc => sc.Inspect(It.IsAny<Transport>())).Returns(true);

        var fleet = new FleetService(serviceCenter.Object);
        fleet.AddTransport(new Scooter("S-001", simplicity, dailyEnergyConsumptionKWh: 0.5));

        var result = fleet.BeginnerFriendlyTransports;

        Assert.Equal(expected, result.Any());
    }
}