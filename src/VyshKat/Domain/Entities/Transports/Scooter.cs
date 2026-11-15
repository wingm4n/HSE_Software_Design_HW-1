using VyshKat.Domain.Interfaces;
using VyshKat.Domain.Validation;

namespace VyshKat.Domain.Entities.Transports;

/// <summary>
/// A kick scooter (kind name "Scooter").
/// Motorised, so it implements <see cref="IEnergyConsumer"/> and takes part in the fleet energy total.
/// </summary>
public sealed class Scooter : Transport, IEnergyConsumer
{
    /// <summary>Daily energy consumption in kWh (finite and non-negative).</summary>
    public double DailyEnergyConsumptionKWh { get; }

    /// <summary>
    /// Creates a scooter with the given inventory number, beginner rating and energy consumption.
    /// </summary>
    /// <param name="inventoryNumber">Inventory number, must not be blank.</param>
    /// <param name="simplicityForBeginner">Beginner rating in the range 1-10.</param>
    /// <param name="dailyEnergyConsumptionKWh">Daily energy consumption in kWh, finite and not negative.</param>
    /// <exception cref="ArgumentException">Some argument is invalid.</exception>
    public Scooter(string inventoryNumber, int simplicityForBeginner, double dailyEnergyConsumptionKWh)
        : base(inventoryNumber, "Scooter", simplicityForBeginner)
    {
        DailyEnergyConsumptionKWh = Guard.EnergyConsumption(
            dailyEnergyConsumptionKWh, nameof(dailyEnergyConsumptionKWh));
    }

    /// <summary>Base description followed by the daily energy consumption.</summary>
    public override string ToString() =>
        $"{base.ToString()} (energy {DailyEnergyConsumptionKWh} kWh)";
}
