using VyshKat.Domain.Interfaces;
using VyshKat.Domain.Validation;

namespace VyshKat.Domain.Entities.Things;

/// <summary>
/// A dock station where vehicles are parked. It is an energy consumer, so its consumption
/// is included in the fleet energy total.
/// </summary>
public sealed class DockStation : Thing, IEnergyConsumer
{
    /// <summary>Number of vehicle slots in the station (positive).</summary>
    public int Capacity { get; }
    /// <summary>Daily energy consumption in kWh (finite and non-negative).</summary>
    public double DailyEnergyConsumptionKWh { get; }

    /// <summary>
    /// Creates a dock station.
    /// </summary>
    /// <param name="inventoryNumber">Inventory number, must not be blank.</param>
    /// <param name="capacity">Number of slots, must be positive.</param>
    /// <param name="dailyEnergyConsumptionKWh">Daily energy consumption in kWh, finite and not negative.</param>
    /// <exception cref="ArgumentException">Some argument is invalid (a non-positive capacity throws <see cref="ArgumentOutOfRangeException"/>).</exception>
    public DockStation(
        string inventoryNumber,
        int capacity,
        double dailyEnergyConsumptionKWh)
        : base(inventoryNumber, "Dock station")
    {
        if (capacity <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(capacity), "Capacity must be positive.");

        Capacity = capacity;
        DailyEnergyConsumptionKWh = Guard.EnergyConsumption(
            dailyEnergyConsumptionKWh, nameof(dailyEnergyConsumptionKWh));
    }

    /// <summary>Base description followed by the capacity.</summary>
    public override string ToString() =>
        $"{base.ToString()} (capacity {Capacity})";
}
