using VyshKat.Application.Services;

namespace VyshKat.Cli;

/// <summary>Interactive text menu on top of <see cref="FleetService"/>.</summary>
public sealed class ConsoleApp
{
    private readonly FleetService _fleet;
    private readonly ConsoleIO _io;
    private readonly FleetReportPrinter _printer;
    private readonly DemoScenario _demo;

    public ConsoleApp(FleetService fleet, ConsoleIO io, FleetReportPrinter printer, DemoScenario demo)
    {
        _fleet = fleet ?? throw new ArgumentNullException(nameof(fleet));
        _io = io ?? throw new ArgumentNullException(nameof(io));
        _printer = printer ?? throw new ArgumentNullException(nameof(printer));
        _demo = demo ?? throw new ArgumentNullException(nameof(demo));
    }

    public void Run()
    {
        _io.WriteLine("==============================================");
        _io.WriteLine("  VyshKat - fleet accounting   (\"Let's ride to the deadline\")");
        _io.WriteLine("==============================================");

        try
        {
            while (true)
            {
                PrintMenu();
                var choice = _io.ReadInt("> ", 0, 6, allowCancel: false);
                _io.WriteLine();

                if (choice == 0)
                    break;

                Execute(choice);
                _io.WriteLine();
            }
        }
        catch (EndOfStreamException)
        {
            _io.WriteLine();
            _io.WriteLine("Input closed.");
        }

        _io.WriteLine("Goodbye!");
    }

    private void PrintMenu()
    {
        _io.WriteLine("Main menu:");
        _io.WriteLine("  1. Accept new transport (with inspection)");
        _io.WriteLine("  2. Put a thing on balance");
        _io.WriteLine("  3. Fleet report (units and daily energy)");
        _io.WriteLine("  4. Equipment suitable for beginners");
        _io.WriteLine("  5. Inventory list (all transport and things)");
        _io.WriteLine("  6. Run demo scenario");
        _io.WriteLine("  0. Exit");
    }

    private void Execute(int choice)
    {
        try
        {
            switch (choice)
            {
                case 1: AddTransport(); break;
                case 2: AddThing(); break;
                case 3: _printer.PrintSummary(_fleet); break;
                case 4: _printer.PrintBeginnerFriendly(_fleet); break;
                case 5: _printer.PrintInventory(_fleet); break;
                case 6: _demo.Run(); break;
            }
        }
        catch (OperationCanceledException)
        {
            _io.WriteLine("Cancelled, nothing was changed.");
        }
        catch (ArgumentException ex)
        {
            // Domain invariants (see Transport / Thing constructors) are the single source of validation rules.
            _io.WriteError($"Invalid data: {ex.Message}");
        }
    }

    private void AddTransport()
    {
        _io.WriteLine($"(type '{ConsoleIO.CancelKeyword}' at any prompt to abort)");
        var entries = ItemCatalog.Transports;
        var entry = entries[_io.ReadChoice("Transport type:", entries.Select(e => e.Title).ToList())];

        var inventoryNumber = ReadFreeInventoryNumber();
        var transport = entry.Create(_io, inventoryNumber);

        _io.WriteLine("Sending to the service center for inspection...");
        // The number was checked for uniqueness above, so "false" here means a failed inspection.
        if (_fleet.AddTransport(transport))
            _io.WriteLine($"Inspection passed. Accepted into the fleet: {transport}");
        else
            _io.WriteLine($"Inspection failed (daily energy above {ServiceCenter.MaxDailyEnergyKWh} kWh is not allowed). " +
                          $"Transport was NOT accepted: {transport}");
    }

    private void AddThing()
    {
        _io.WriteLine($"(type '{ConsoleIO.CancelKeyword}' at any prompt to abort)");
        var entries = ItemCatalog.Things;
        var entry = entries[_io.ReadChoice("Thing type:", entries.Select(e => e.Title).ToList())];

        var inventoryNumber = ReadFreeInventoryNumber();
        var thing = entry.Create(_io, inventoryNumber);

        if (_fleet.AddThing(thing))
            _io.WriteLine($"Put on balance: {thing}");
        else
            _io.WriteLine($"Thing was NOT put on balance: {thing}");
    }

    /// <summary>Asks for an inventory number until it is not used by any transport or thing yet.</summary>
    private string ReadFreeInventoryNumber()
    {
        while (true)
        {
            var number = _io.ReadNonEmpty("Inventory number: ");
            if (!_fleet.IsInventoryNumberTaken(number))
                return number;

            _io.WriteError($"Inventory number '{number}' is already in use. Enter another one.");
        }
    }
}
