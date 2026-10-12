using VyshKat.Application.Services;
using VyshKat.Domain.Interfaces;

namespace VyshKat.Cli;

/// <summary>Formats fleet reports. Shared by the interactive menu and the demo scenario.</summary>
public sealed class FleetReportPrinter
{
    // Output channel; the printer has no state of its own, so one instance serves the menu and the demo.
    private readonly ConsoleIO _io;

    /// <summary>
    /// Creates the printer.
    /// </summary>
    /// <param name="io">Where the reports are written.</param>
    /// <exception cref="ArgumentNullException"><paramref name="io"/> is <c>null</c>.</exception>
    public FleetReportPrinter(ConsoleIO io)
    {
        _io = io ?? throw new ArgumentNullException(nameof(io));
    }

    /// <summary>
    /// Prints the unit counts, the total daily energy and a per-kind table (units and energy).
    /// </summary>
    /// <param name="fleet">The fleet to report on.</param>
    public void PrintSummary(FleetService fleet)
    {
        _io.WriteLine("=== Fleet report ===");
        _io.WriteLine($"Total units on balance : {fleet.UnitCount}");
        _io.WriteLine($"  transport            : {fleet.AllTransports.Count}");
        _io.WriteLine($"  things               : {fleet.AllThings.Count}");
        _io.WriteLine($"Total daily energy     : {fleet.TotalDailyEnergyKWh:F2} kWh");

        if (fleet.UnitCount == 0)
        {
            _io.WriteLine("(the fleet is empty)");
            return;
        }

        _io.WriteLine();
        _io.WriteLine($"{"Type",-18}{"Units",6}{"Energy, kWh",14}");
        foreach (var group in fleet.AllItems.GroupBy(i => i.Name).OrderBy(g => g.Key))
        {
            var energy = group.OfType<IEnergyConsumer>().Sum(c => c.DailyEnergyConsumptionKWh);
            _io.WriteLine($"{group.Key,-18}{group.Count(),6}{energy,14:F2}");
        }
    }

    /// <summary>
    /// Prints the items (transport and things) whose beginner rating reaches <see cref="FleetService.BeginnerSimplicityLimit"/>.
    /// </summary>
    /// <param name="fleet">The fleet to report on.</param>
    public void PrintBeginnerFriendly(FleetService fleet)
    {
        _io.WriteLine($"=== Equipment suitable for beginners (simplicity >= {FleetService.BeginnerSimplicityLimit}) ===");

        var list = fleet.BeginnerFriendlyItems;
        if (list.Count == 0)
        {
            _io.WriteLine("(none)");
            return;
        }

        _io.WriteLine($"{"Name",-18}{"Inventory #",-14}{"Simplicity",10}");
        foreach (var t in list)
            _io.WriteLine($"{t.Name,-18}{t.InventoryNumber,-14}{t.SimplicityForBeginner,10}");
    }

    /// <summary>
    /// Prints the name and inventory number of every transport and thing, in two sections.
    /// </summary>
    /// <param name="fleet">The fleet to report on.</param>
    public void PrintInventory(FleetService fleet)
    {
        _io.WriteLine("=== Inventory list ===");

        if (fleet.UnitCount == 0)
        {
            _io.WriteLine("(the fleet is empty)");
            return;
        }

        PrintSection("Transport", fleet.AllTransports);
        PrintSection("Things", fleet.AllThings);
    }

    /// <summary>
    /// Prints one titled section of the inventory list, or "(none)" when it is empty.
    /// </summary>
    /// <param name="title">Section title.</param>
    /// <param name="items">Items of the section.</param>
    private void PrintSection(string title, IEnumerable<IInventoriable> items)
    {
        _io.WriteLine($"-- {title} --");
        _io.WriteLine($"{"Name",-18}Inventory #");

        var any = false;
        foreach (var item in items)
        {
            any = true;
            _io.WriteLine($"{item.Name,-18}{item.InventoryNumber}");
        }

        if (!any)
            _io.WriteLine("(none)");
    }
}
