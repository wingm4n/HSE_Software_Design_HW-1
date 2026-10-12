using VyshKat.Domain.Interfaces;

namespace VyshKat.Domain.Entities.Transport;

public sealed class Scooter : Transport, IEnergyConsumer
{
    public double DailyEnergyConsumptionKWh { get; }

    public Scooter(string inventoryNumber, int simplicityForBeginner, double dailyEnergyConsumptionKWh)
        : base(inventoryNumber, "Scooter", simplicityForBeginner)
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