using VyshKat.Domain.Entities.Things;
using VyshKat.Domain.Entities.Transport;
namespace VyshKat.Cli;

public class ConsoleApp
{
    public void Run()
    {
        Console.WriteLine("Console app running");
        var helmet = new Helmet("H-001", "M");
        var charger = new Charger("C-001", 1.2);

        Console.WriteLine(helmet);
        Console.WriteLine(charger);

        var dock = new DockStation("D-001", capacity: 6, dailyEnergyConsumptionKWh: 0.4);

        Console.WriteLine(dock); // Dock station [D-001] (capacity 6)

        var scooter = new Scooter("S-001", simplicityForBeginner: 7, dailyEnergyConsumptionKWh: 0.9);
        var bike = new Bicycle("B-001", simplicityForBeginner: 5);

        Console.WriteLine(scooter); // Scooter [S-001] (energy 0.9 kWh)
        Console.WriteLine(bike);    // Bicycle [B-001]
    }
}
