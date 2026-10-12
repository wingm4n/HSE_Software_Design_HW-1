using VyshKat.Domain.Interfaces;
using VyshKat.Domain.Validation;

namespace VyshKat.Domain.Entities.Things;

/// <summary>
/// A self-service charging cabinet. It is the demo of a <see cref="Thing"/> that also has a
/// "simplicity for a beginner" rating (how easy it is to use for someone who has never done it),
/// so it takes part in the beginner-friendly list together with transport.
/// </summary>
public sealed class ChargingCabinet : Thing, IEnergyConsumer, IBeginnerRated
{
    public int SimplicityForBeginner { get; }
    public double DailyEnergyConsumptionKWh { get; }

    public ChargingCabinet(string inventoryNumber, int simplicityForBeginner, double dailyEnergyConsumptionKWh)
        : base(inventoryNumber, "Charging cabinet")
    {
        SimplicityForBeginner = Guard.Simplicity(simplicityForBeginner, nameof(simplicityForBeginner));
        DailyEnergyConsumptionKWh = Guard.EnergyConsumption(
            dailyEnergyConsumptionKWh, nameof(dailyEnergyConsumptionKWh));
    }

    public override string ToString() =>
        $"{base.ToString()} (energy {DailyEnergyConsumptionKWh} kWh)";
}
