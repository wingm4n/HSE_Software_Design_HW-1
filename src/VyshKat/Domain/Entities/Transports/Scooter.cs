using VyshKat.Domain.Interfaces;
using VyshKat.Domain.Validation;

namespace VyshKat.Domain.Entities.Transports;

public sealed class Scooter : Transport, IEnergyConsumer
{
    public double DailyEnergyConsumptionKWh { get; }

    public Scooter(string inventoryNumber, int simplicityForBeginner, double dailyEnergyConsumptionKWh)
        : base(inventoryNumber, "Scooter", simplicityForBeginner)
    {
        DailyEnergyConsumptionKWh = Guard.EnergyConsumption(
            dailyEnergyConsumptionKWh, nameof(dailyEnergyConsumptionKWh));
    }

    public override string ToString() =>
        $"{base.ToString()} (energy {DailyEnergyConsumptionKWh} kWh)";
}
