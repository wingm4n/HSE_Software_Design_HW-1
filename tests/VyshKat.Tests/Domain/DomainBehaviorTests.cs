using VyshKat.Application.Services;
using VyshKat.Domain.Entities.Things;
using VyshKat.Domain.Entities.Transports;
using VyshKat.Domain.Interfaces;


namespace VyshKat.Tests.Domain;


public class DomainBehaviorTests
{
    /// <summary>A brand-new kind of transport defined outside the production code (open/closed check).</summary>
    private sealed class Skateboard : Transport
    {
        public Skateboard(string inventoryNumber, int simplicity) : base(inventoryNumber, "Skateboard", simplicity) { }
    }


    // ---------- polymorphism of things ----------


    [Fact]
    public void Repair_IsPolymorphic_ForSafetyThings()
    {
        SafetyThing[] things =
        {
            new Helmet("H-1", "M"),
            new KneePads("K-1", "L"),
            new ElbowPads("E-1", "S"),
        };


        Assert.Equal(
            new[]
            {
                "Repairing a damaged Helmet",
                "Repairing a pair of damaged Knee pads",
                "Repairing a pair of damaged Elbow pads",
            },
            things.Select(t => t.Repair()));
    }


    [Theory]
    [InlineData(typeof(Helmet), "Head")]
    [InlineData(typeof(KneePads), "Knees")]
    [InlineData(typeof(ElbowPads), "Elbows")]
    public void SafetyThings_KnowWhichBodyPartTheySave(Type type, string part)
    {
        var thing = (SafetyThing)Activator.CreateInstance(type, "X-1", "M")!;


        Assert.Equal(part, thing.SavedPartOfTheBody);
        Assert.Equal("M", thing.Size);
        Assert.Contains(part, thing.ToString());
    }


    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void SafetyThing_RejectsBlankSize(string size)
    {
        Assert.Throws<ArgumentException>(() => new Helmet("H-1", size));
    }


    [Theory]
    [InlineData(0)]
    [InlineData(-3)]
    public void DockStation_RejectsNonPositiveCapacity(int capacity)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new DockStation("D-1", capacity, 0.4));
    }


    [Fact]
    public void DockStation_KeepsCapacityAndEnergy_AndShowsThemInToString()
    {
        var dock = new DockStation("D-1", 6, 0.4);


        Assert.Equal(6, dock.Capacity);
        Assert.Equal(0.4, dock.DailyEnergyConsumptionKWh);
        Assert.Contains("capacity 6", dock.ToString());
        Assert.IsAssignableFrom<IEnergyConsumer>(dock);
        Assert.IsNotAssignableFrom<IBeginnerRated>(dock);
    }


    // ---------- transport ----------


    [Theory]
    [InlineData(0)]
    [InlineData(11)]
    [InlineData(-1)]
    public void Transport_RejectsSimplicityOutsideOneToTen(int simplicity)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Bicycle("B-1", simplicity));
        Assert.Throws<ArgumentOutOfRangeException>(() => new Scooter("S-1", simplicity, 1.0));
        Assert.Throws<ArgumentOutOfRangeException>(() => new EBike("E-1", simplicity, 1.0));
    }


    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    public void Transport_AcceptsSimplicityBoundaries(int simplicity)
    {
        Assert.Equal(simplicity, new Bicycle("B-1", simplicity).SimplicityForBeginner);
    }


    [Fact]
    public void OnlyMotorisedTransport_ConsumesEnergy()
    {
        Assert.IsAssignableFrom<IEnergyConsumer>(new Scooter("S-1", 5, 1.0));
        Assert.IsAssignableFrom<IEnergyConsumer>(new EBike("E-1", 5, 1.0));
        Assert.IsNotAssignableFrom<IEnergyConsumer>(new Bicycle("B-1", 5));
    }


    [Fact]
    public void ToString_ContainsNameAndInventoryNumber_ForEveryKind()
    {
        IInventoriable[] items =
        {
            new Scooter("S-1", 5, 1.0), new EBike("E-1", 5, 1.0), new Bicycle("B-1", 5),
            new Helmet("H-1", "M"), new Charger("C-1", 1.0), new ChargingCabinet("CC-1", 5, 1.0),
            new DockStation("D-1", 2, 1.0),
        };


        foreach (var item in items)
        {
            Assert.Contains(item.Name, item.ToString());
            Assert.Contains(item.InventoryNumber, item.ToString());
        }
    }


    [Fact]
    public void Beginner_And_Energy_Contracts_AreExposedThroughInterfaces()
    {
        IBeginnerRated rated = new ChargingCabinet("CC-1", 6, 0.8);
        IEnergyConsumer consumer = (IEnergyConsumer)rated;


        Assert.Equal(6, rated.SimplicityForBeginner);
        Assert.Equal(0.8, consumer.DailyEnergyConsumptionKWh);
        Assert.Equal("CC-1", rated.InventoryNumber);
    }


    // ---------- extensibility ----------


    [Fact]
    public void NewTransportKind_WorksWithFleetAndInspection_WithoutChangingExistingCode()
    {
        var fleet = new FleetService(new ServiceCenter());


        Assert.True(fleet.AddTransport(new Skateboard("SK-1", 8)));
        Assert.True(fleet.AddTransport(new Skateboard("SK-2", 2)));


        Assert.Equal(2, fleet.AllTransports.Count);
        Assert.Equal(0.0, fleet.TotalDailyEnergyKWh);                       // no energy interface -> contributes nothing
        Assert.Equal("SK-1", Assert.Single(fleet.BeginnerFriendlyItems).InventoryNumber);
    }
}
