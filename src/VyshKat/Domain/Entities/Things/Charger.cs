using VyshKat.Domain.Interfaces;
using VyshKat.Domain.Validation;

namespace VyshKat.Domain.Entities.Things;

public sealed class Charger : Thing, IEnergyConsumer
{
    public double DailyEnergyConsumptionKWh { get; }

    public Charger(string inventoryNumber, double dailyEnergyConsumptionKWh)
        : base(inventoryNumber, "Charger")
    {
        DailyEnergyConsumptionKWh = Guard.EnergyConsumption(
            dailyEnergyConsumptionKWh, nameof(dailyEnergyConsumptionKWh));
    }

    public override string ToString() =>
        $"{base.ToString()} (energy {DailyEnergyConsumptionKWh} kWh)";
}
