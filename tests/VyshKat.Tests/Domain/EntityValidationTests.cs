using VyshKat.Domain.Entities.Things;
using VyshKat.Domain.Entities.Transports;
using VyshKat.Domain.Interfaces;

namespace VyshKat.Tests.Domain;

public class EntityValidationTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(11)]
    public void ChargingCabinet_RejectsSimplicityOutOfRange(int simplicity)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new ChargingCabinet("CC-001", simplicity, 0.5));
    }

    [Fact]
    public void ChargingCabinet_IsBeginnerRatedEnergyConsumer()
    {
        var cabinet = new ChargingCabinet("CC-001", 8, 0.5);

        Assert.IsAssignableFrom<IBeginnerRated>(cabinet);
        Assert.IsAssignableFrom<IEnergyConsumer>(cabinet);
        Assert.Equal(8, cabinet.SimplicityForBeginner);
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    public void Energy_RejectsNegativeAndNonFiniteValues(double energy)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Scooter("S-001", 5, energy));
        Assert.Throws<ArgumentOutOfRangeException>(() => new EBike("E-001", 5, energy));
        Assert.Throws<ArgumentOutOfRangeException>(() => new Charger("C-001", energy));
        Assert.Throws<ArgumentOutOfRangeException>(() => new DockStation("D-001", 4, energy));
    }

    [Fact]
    public void BlankInventoryNumber_IsRejected_AndParamNameIsCorrect()
    {
        var thingEx = Assert.Throws<ArgumentException>(() => new Helmet(" ", "M"));
        var transportEx = Assert.Throws<ArgumentException>(() => new Bicycle("", 5));

        Assert.Equal("inventoryNumber", thingEx.ParamName);
        Assert.Equal("inventoryNumber", transportEx.ParamName);
    }
}
