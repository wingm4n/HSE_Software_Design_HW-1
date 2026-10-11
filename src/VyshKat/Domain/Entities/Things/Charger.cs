using VyshKat.Domain.Interfaces;

namespace VyshKat.Domain.Entities.Things;

public sealed class Charger : Thing, IEnergyConsumer
{
    public double DailyEnergyConsumptionKWh { get; }

    public Charger(string inventoryNumber, double dailyEnergyConsumptionKWh)
        : base(inventoryNumber, "Charger")
    {
        if (dailyEnergyConsumptionKWh < 0)
            throw new ArgumentOutOfRangeException(
                nameof(dailyEnergyConsumptionKWh),
                "Daily energy consumption cannot be negative.");

        DailyEnergyConsumptionKWh = dailyEnergyConsumptionKWh;
    }
}