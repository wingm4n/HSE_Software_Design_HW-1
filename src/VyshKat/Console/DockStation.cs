using VyshKat.Domain.Interfaces;

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

        if (dailyEnergyConsumptionKWh < 0)
            throw new ArgumentOutOfRangeException(
                nameof(dailyEnergyConsumptionKWh),
                "Daily energy consumption cannot be negative.");

        Capacity = capacity;
        DailyEnergyConsumptionKWh = dailyEnergyConsumptionKWh;
    }

    public override string ToString() =>
        $"{base.ToString()} (capacity {Capacity})";
}