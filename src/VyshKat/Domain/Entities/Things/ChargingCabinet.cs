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
    /// <summary>How easy the cabinet is for a beginner, from 1 to 10 (validated in the constructor).</summary>
    public int SimplicityForBeginner { get; }
    /// <summary>Daily energy consumption in kWh (finite and non-negative).</summary>
    public double DailyEnergyConsumptionKWh { get; }

    /// <summary>
    /// Creates a charging cabinet.
    /// </summary>
    /// <param name="inventoryNumber">Inventory number, must not be blank.</param>
    /// <param name="simplicityForBeginner">Beginner rating in the range 1-10.</param>
    /// <param name="dailyEnergyConsumptionKWh">Daily energy consumption in kWh, finite and not negative.</param>
    /// <exception cref="ArgumentException">Some argument is invalid.</exception>
    public ChargingCabinet(string inventoryNumber, int simplicityForBeginner, double dailyEnergyConsumptionKWh)
        : base(inventoryNumber, "Charging cabinet")
    {
        SimplicityForBeginner = Guard.Simplicity(simplicityForBeginner, nameof(simplicityForBeginner));
        DailyEnergyConsumptionKWh = Guard.EnergyConsumption(
            dailyEnergyConsumptionKWh, nameof(dailyEnergyConsumptionKWh));
    }

    /// <summary>Base description followed by the daily energy consumption.</summary>
    public override string ToString() =>
        $"{base.ToString()} (energy {DailyEnergyConsumptionKWh} kWh)";
}
