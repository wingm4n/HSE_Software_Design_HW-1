using VyshKat.Domain.Entities.Things;
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
    }
}
