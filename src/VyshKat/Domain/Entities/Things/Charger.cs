using VyshKat.Domain.Interfaces;
using VyshKat.Domain.Validation;

namespace VyshKat.Domain.Entities.Things;

/// <summary>A charger. It is an energy consumer, so its consumption is included in the fleet energy total.</summary>
public sealed class Charger : Thing, IEnergyConsumer
{
    /// <summary>Daily energy consumption in kWh (finite and non-negative).</summary>
    public double DailyEnergyConsumptionKWh { get; }

    /// <summary>
    /// Creates a charger.
    /// </summary>
    /// <param name="inventoryNumber">Inventory number, must not be blank.</param>
    /// <param name="dailyEnergyConsumptionKWh">Daily energy consumption in kWh, finite and not negative.</param>
    /// <exception cref="ArgumentException">Some argument is invalid.</exception>
    public Charger(string inventoryNumber, double dailyEnergyConsumptionKWh)
        : base(inventoryNumber, "Charger")
    {
        DailyEnergyConsumptionKWh = Guard.EnergyConsumption(
            dailyEnergyConsumptionKWh, nameof(dailyEnergyConsumptionKWh));
    }

    /// <summary>Base description followed by the daily energy consumption.</summary>
    public override string ToString() =>
        $"{base.ToString()} (energy {DailyEnergyConsumptionKWh} kWh)";
}
