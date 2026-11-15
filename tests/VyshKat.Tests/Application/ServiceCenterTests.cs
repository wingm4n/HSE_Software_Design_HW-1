using VyshKat.Application.Services;
using VyshKat.Domain.Entities.Transports;

namespace VyshKat.Tests.Application;

public class ServiceCenterTests
{
    private readonly ServiceCenter _center = new();

    [Theory]
    [InlineData(0.0)]
    [InlineData(1.5)]
    [InlineData(ServiceCenter.MaxDailyEnergyKWh)] // the limit itself is still fine
    public void Inspect_Accepts_WhenEnergyIsWithinLimit(double energy)
    {
        Assert.True(_center.Inspect(new Scooter("S-001", 7, energy)));
        Assert.True(_center.Inspect(new EBike("E-001", 7, energy)));
    }

    [Fact]
    public void Inspect_Declines_WhenEnergyIsAboveLimit()
    {
        Assert.False(_center.Inspect(new Scooter("S-001", 7, ServiceCenter.MaxDailyEnergyKWh + 0.01)));
        Assert.False(_center.Inspect(new EBike("E-001", 7, 100)));
    }

    [Fact]
    public void Inspect_Accepts_BicycleWithoutEnergy()
    {
        Assert.True(_center.Inspect(new Bicycle("B-001", 3)));
    }

    [Fact]
    public void Inspect_Declines_Null()
    {
        Assert.False(_center.Inspect(null!));
    }
}
