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
    // Not a FleetService: the demo builds its own fleet around the injected inspection (the same one the real fleet uses).
    private readonly IServiceCenter _serviceCenter;
    private readonly ConsoleIO _io;
    private readonly FleetReportPrinter _printer;

    /// <summary>
    /// Creates the demo.
    /// </summary>
    /// <param name="serviceCenter">Inspection used by the demo fleet.</param>
    /// <param name="io">Text input/output.</param>
    /// <param name="printer">Report formatter.</param>
    /// <exception cref="ArgumentNullException">Any argument is <c>null</c>.</exception>
    public DemoScenario(IServiceCenter serviceCenter, ConsoleIO io, FleetReportPrinter printer)
    {
        _serviceCenter = serviceCenter ?? throw new ArgumentNullException(nameof(serviceCenter));
        _io = io ?? throw new ArgumentNullException(nameof(io));
        _printer = printer ?? throw new ArgumentNullException(nameof(printer));
    }

    /// <summary>Plays all the demo steps and prints the final reports.</summary>
    public void Run()
    {
        _io.WriteLine("##### DEMO (isolated demo fleet, your data is not affected) #####");
        var fleet = new FleetService(_serviceCenter);

        _io.WriteLine();
        _io.WriteLine("Step 1. Accepting transport: every vehicle goes through the service center first.");
        // Valid vehicles pass the inspection.
        Accept(fleet, new Scooter("S-001", simplicityForBeginner: 7, dailyEnergyConsumptionKWh: 0.9));
        Accept(fleet, new Scooter("S-002", simplicityForBeginner: 4, dailyEnergyConsumptionKWh: 1.1));
        Accept(fleet, new EBike("E-001", simplicityForBeginner: 6, dailyEnergyConsumptionKWh: 1.5));
        Accept(fleet, new Bicycle("B-001", simplicityForBeginner: 9));
        Accept(fleet, new Bicycle("B-002", simplicityForBeginner: 5));

        _io.WriteLine();
        _io.WriteLine($"Step 2. The service center declines a faulty vehicle (limit: {ServiceCenter.MaxDailyEnergyKWh} kWh per day).");
        // 12 kWh per day is above the service center limit, so this one is declined.
        Accept(fleet, new EBike("E-666", simplicityForBeginner: 8, dailyEnergyConsumptionKWh: 12.0));

        _io.WriteLine();
        _io.WriteLine("Step 3. Putting things on balance (no inspection for things).");
        var things = new Thing[]
        {
            new Helmet("H-001", "M"),
            new KneePads("K-001", "L"),
            new ElbowPads("EL-001", "S"),
            new Charger("C-001", dailyEnergyConsumptionKWh: 1.2),
            new DockStation("D-001", capacity: 6, dailyEnergyConsumptionKWh: 0.4),
            new ChargingCabinet("CC-001", simplicityForBeginner: 8, dailyEnergyConsumptionKWh: 0.8),
            new ChargingCabinet("CC-002", simplicityForBeginner: 3, dailyEnergyConsumptionKWh: 0.8),
        };
        foreach (var thing in things)
            AddThing(fleet, thing);

        _io.WriteLine();
        _io.WriteLine("Step 4. Inventory numbers are unique across transport and things:");
        // "S-001" is taken by a scooter, "B-001" by a bicycle: duplicates are refused even across transport and things.
        Accept(fleet, new Bicycle("S-001", simplicityForBeginner: 9));
        AddThing(fleet, new Helmet("B-001", "L"));

        _io.WriteLine();
        _io.WriteLine("Step 5. Safety things can be repaired (polymorphic behaviour):");
        foreach (var safety in things.OfType<SafetyThing>())
            _io.WriteLine($"  {safety.InventoryNumber}: {safety.Repair()}");

        _io.WriteLine();
        _io.WriteLine("Step 6. Invalid data is rejected by the domain model:");
        TryCreate("Scooter with an empty inventory number", () => new Scooter("  ", 5, 1.0));
        TryCreate("Bicycle with simplicity 11", () => new Bicycle("B-099", 11));
        TryCreate("E-Bike with negative energy", () => new EBike("E-099", 5, -2.0));
        TryCreate("Charging cabinet with simplicity 0", () => new ChargingCabinet("CC-099", 0, 0.5));

        _io.WriteLine();
        _printer.PrintSummary(fleet);
        _io.WriteLine();
        _printer.PrintBeginnerFriendly(fleet);
        _io.WriteLine();
        _printer.PrintInventory(fleet);

        _io.WriteLine();
        _io.WriteLine("##### DEMO finished #####");
    }

    /// <summary>Tries to accept a vehicle and prints the outcome: accepted, duplicate number or declined by the service center.</summary>
    private void Accept(FleetService fleet, Transport transport)
    {
        // Remember whether the number was already used, to tell a duplicate apart from a failed inspection.
        var wasTaken = fleet.IsInventoryNumberTaken(transport.InventoryNumber);

        if (fleet.AddTransport(transport))
            _io.WriteLine($"  ACCEPTED: {transport}");
        else if (wasTaken)
            _io.WriteLine($"  REJECTED (inventory number already in use): {transport}");
        else
            _io.WriteLine($"  REJECTED by service center: {transport}");
    }

    /// <summary>Tries to put a thing on balance and prints the outcome.</summary>
    private void AddThing(FleetService fleet, Thing thing)
    {
        if (fleet.AddThing(thing))
            _io.WriteLine($"  Added: {thing}");
        else
            _io.WriteLine($"  REJECTED (inventory number already in use): {thing}");
    }

    /// <summary>
    /// Shows that the domain model refuses invalid data: runs <paramref name="factory"/> and prints the validation message.
    /// </summary>
    /// <param name="description">What is being created (for the output).</param>
    /// <param name="factory">Constructor call that is expected to throw <see cref="ArgumentException"/>.</param>
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
}
