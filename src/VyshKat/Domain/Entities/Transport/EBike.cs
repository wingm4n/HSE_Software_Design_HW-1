using VyshKat.Domain.Interfaces;

namespace VyshKat.Domain.Entities.Transport;

public sealed class EBike : Transport, IEnergyConsumer
{
    public double DailyEnergyConsumptionKWh { get; }

    public EBike(string inventoryNumber, int simplicityForBeginner, double dailyEnergyConsumptionKWh)
        : base(inventoryNumber, "E-Bike", simplicityForBeginner)
    {
        if (dailyEnergyConsumptionKWh < 0)
            throw new ArgumentOutOfRangeException(
                nameof(dailyEnergyConsumptionKWh),
                "Daily energy consumption cannot be negative.");

        DailyEnergyConsumptionKWh = dailyEnergyConsumptionKWh;
    }

    public override string ToString() =>
        $"{base.ToString()} (energy {DailyEnergyConsumptionKWh} kWh)";
}