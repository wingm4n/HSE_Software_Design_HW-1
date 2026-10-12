using VyshKat.Domain.Interfaces;
using VyshKat.Domain.Validation;

namespace VyshKat.Domain.Entities.Transports;

public sealed class EBike : Transport, IEnergyConsumer
{
    public double DailyEnergyConsumptionKWh { get; }

    public EBike(string inventoryNumber, int simplicityForBeginner, double dailyEnergyConsumptionKWh)
        : base(inventoryNumber, "E-Bike", simplicityForBeginner)
    {
        DailyEnergyConsumptionKWh = Guard.EnergyConsumption(
            dailyEnergyConsumptionKWh, nameof(dailyEnergyConsumptionKWh));
    }

    public override string ToString() =>
        $"{base.ToString()} (energy {DailyEnergyConsumptionKWh} kWh)";
}
