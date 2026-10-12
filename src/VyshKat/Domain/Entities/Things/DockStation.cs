using VyshKat.Domain.Interfaces;
using VyshKat.Domain.Validation;

namespace VyshKat.Domain.Entities.Things;

public sealed class DockStation : Thing, IEnergyConsumer
{
    public int Capacity { get; }
    public double DailyEnergyConsumptionKWh { get; }

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

    public override string ToString() =>
        $"{base.ToString()} (capacity {Capacity})";
}
