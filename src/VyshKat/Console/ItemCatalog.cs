using VyshKat.Domain.Entities.Things;
using VyshKat.Domain.Entities.Transport;

namespace VyshKat.Cli;

/// <summary>
/// One selectable kind of item in the "add" menus.
/// <paramref name="Create"/> receives the inventory number and asks the user for kind-specific fields.
/// </summary>
public sealed record CatalogEntry<T>(string Title, Func<ConsoleIO, string, T> Create);

/// <summary>
/// UI-side catalog of item kinds. Supporting a new kind of transport / thing in the console
/// means adding one entry here (the domain and FleetService stay untouched).
/// </summary>
public static class ItemCatalog
{
    private const string SimplicityPrompt = "Simplicity for beginners (1-10): ";
    private const string EnergyPrompt = "Daily energy consumption, kWh: ";
    private const string SizePrompt = "Size (e.g. S / M / L): ";

    public static IReadOnlyList<CatalogEntry<Transport>> Transports { get; } = new[]
    {
        new CatalogEntry<Transport>("Scooter", (io, inv) =>
            new Scooter(inv, io.ReadInt(SimplicityPrompt, 1, 10), io.ReadNonNegativeDouble(EnergyPrompt))),

        new CatalogEntry<Transport>("E-Bike", (io, inv) =>
            new EBike(inv, io.ReadInt(SimplicityPrompt, 1, 10), io.ReadNonNegativeDouble(EnergyPrompt))),

        new CatalogEntry<Transport>("Bicycle", (io, inv) =>
            new Bicycle(inv, io.ReadInt(SimplicityPrompt, 1, 10))),
    };

    public static IReadOnlyList<CatalogEntry<Thing>> Things { get; } = new[]
    {
        new CatalogEntry<Thing>("Helmet", (io, inv) => new Helmet(inv, io.ReadNonEmpty(SizePrompt))),

        new CatalogEntry<Thing>("Knee pads", (io, inv) => new KneePads(inv, io.ReadNonEmpty(SizePrompt))),

        new CatalogEntry<Thing>("Elbow pads", (io, inv) => new ElbowPads(inv, io.ReadNonEmpty(SizePrompt))),

        new CatalogEntry<Thing>("Charger", (io, inv) =>
            new Charger(inv, io.ReadNonNegativeDouble(EnergyPrompt))),

        new CatalogEntry<Thing>("Dock station", (io, inv) =>
            new DockStation(
                inv,
                io.ReadInt("Capacity (number of slots): ", 1, 1000),
                io.ReadNonNegativeDouble(EnergyPrompt))),
    };
}
