using VyshKat.Application.Services;
using VyshKat.Domain.Entities.Things;
using VyshKat.Domain.Entities.Transports;
using VyshKat.Domain.Interfaces;

namespace VyshKat.Cli;

/// <summary>
/// Scripted walk-through of every scenario of the application.
/// Works on its own isolated <see cref="FleetService"/>, so the user's fleet is never touched.
/// </summary>
public sealed class DemoScenario
{
    private readonly IServiceCenter _serviceCenter;
    private readonly ConsoleIO _io;
    private readonly FleetReportPrinter _printer;

    public DemoScenario(IServiceCenter serviceCenter, ConsoleIO io, FleetReportPrinter printer)
    {
        _serviceCenter = serviceCenter ?? throw new ArgumentNullException(nameof(serviceCenter));
        _io = io ?? throw new ArgumentNullException(nameof(io));
        _printer = printer ?? throw new ArgumentNullException(nameof(printer));
    }

    public void Run()
    {
        _io.WriteLine("##### DEMO (isolated demo fleet, your data is not affected) #####");
        var fleet = new FleetService(_serviceCenter);

        _io.WriteLine();
        _io.WriteLine("Step 1. Accepting transport: every vehicle goes through the service center first.");
        Accept(fleet, new Scooter("S-001", simplicityForBeginner: 7, dailyEnergyConsumptionKWh: 0.9));
        Accept(fleet, new Scooter("S-002", simplicityForBeginner: 4, dailyEnergyConsumptionKWh: 1.1));
        Accept(fleet, new EBike("E-001", simplicityForBeginner: 6, dailyEnergyConsumptionKWh: 1.5));
        Accept(fleet, new Bicycle("B-001", simplicityForBeginner: 9));
        Accept(fleet, new Bicycle("B-002", simplicityForBeginner: 5));

        _io.WriteLine();
        _io.WriteLine("Step 2. Putting things on balance (no inspection for things).");
        var things = new Thing[]
        {
            new Helmet("H-001", "M"),
            new KneePads("K-001", "L"),
            new ElbowPads("EL-001", "S"),
            new Charger("C-001", dailyEnergyConsumptionKWh: 1.2),
            new DockStation("D-001", capacity: 6, dailyEnergyConsumptionKWh: 0.4),
        };
        foreach (var thing in things)
        {
            fleet.AddThing(thing);
            _io.WriteLine($"  Added: {thing}");
        }

        _io.WriteLine();
        _io.WriteLine("Step 3. Safety things can be repaired (polymorphic behaviour):");
        foreach (var safety in things.OfType<SafetyThing>())
            _io.WriteLine($"  {safety.InventoryNumber}: {safety.Repair()}");

        _io.WriteLine();
        _io.WriteLine("Step 4. Invalid data is rejected by the domain model:");
        TryCreate("Scooter with an empty inventory number", () => new Scooter("  ", 5, 1.0));
        TryCreate("Bicycle with simplicity 11", () => new Bicycle("B-099", 11));
        TryCreate("E-Bike with negative energy", () => new EBike("E-099", 5, -2.0));

        _io.WriteLine();
        _io.WriteLine("Step 5. A strict service center declines a vehicle (demo stub plugged in via IServiceCenter):");
        var strictFleet = new FleetService(new RejectAllServiceCenter());
        Accept(strictFleet, new Scooter("S-777", simplicityForBeginner: 8, dailyEnergyConsumptionKWh: 0.5));
        _io.WriteLine($"  Units in the strict fleet: {strictFleet.UnitCount}");

        _io.WriteLine();
        _printer.PrintSummary(fleet);
        _io.WriteLine();
        _printer.PrintBeginnerFriendly(fleet);
        _io.WriteLine();
        _printer.PrintInventory(fleet);

        _io.WriteLine();
        _io.WriteLine("##### DEMO finished #####");
    }

    private void Accept(FleetService fleet, Transport transport)
    {
        var accepted = fleet.AddTransport(transport);
        _io.WriteLine(accepted
            ? $"  ACCEPTED: {transport}"
            : $"  REJECTED by service center: {transport}");
    }

    private void TryCreate(string description, Func<object> factory)
    {
        try
        {
            factory();
            _io.WriteLine($"  {description}: created (unexpected)");
        }
        catch (ArgumentException ex)
        {
            _io.WriteLine($"  {description}: {ex.Message.Split('\n')[0]}");
        }
    }

    private sealed class RejectAllServiceCenter : IServiceCenter
    {
        public bool Inspect(Transport transport) => false;
    }
}
