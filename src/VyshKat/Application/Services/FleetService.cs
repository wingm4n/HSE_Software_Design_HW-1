using VyshKat.Domain.Entities.Things;
using VyshKat.Domain.Entities.Transports;
using VyshKat.Domain.Interfaces;

namespace VyshKat.Application.Services;

public sealed class FleetService
{
    /// <summary>An item with simplicity for a beginner at or above this value may be given to beginners.</summary>
    public const int BeginnerSimplicityLimit = 6;

    private readonly IServiceCenter _serviceCenter;
    private readonly List<IInventoriable> _items = new();

    public FleetService(IServiceCenter serviceCenter)
    {
        _serviceCenter = serviceCenter ?? throw new ArgumentNullException(nameof(serviceCenter));
    }

    /// <summary>
    /// Tries to accept a vehicle. Returns false (and changes nothing) if the inventory number is
    /// already in use or the service center declines the vehicle.
    /// </summary>
    public bool AddTransport(Transport transport)
    {
        if (transport is null)
            throw new ArgumentNullException(nameof(transport));

        if (IsInventoryNumberTaken(transport.InventoryNumber))
            return false;

        if (!_serviceCenter.Inspect(transport))
            return false;

        _items.Add(transport);
        return true;
    }

    /// <summary>
    /// Puts a thing on balance (things are not inspected). Returns false (and changes nothing)
    /// if the inventory number is already in use.
    /// </summary>
    public bool AddThing(Thing thing)
    {
        if (thing is null)
            throw new ArgumentNullException(nameof(thing));

        if (IsInventoryNumberTaken(thing.InventoryNumber))
            return false;

        _items.Add(thing);
        return true;
    }

    /// <summary>Inventory numbers are unique across transport and things; case and outer spaces are ignored.</summary>
    public bool IsInventoryNumberTaken(string inventoryNumber)
    {
        ArgumentNullException.ThrowIfNull(inventoryNumber);

        var key = inventoryNumber.Trim();
        return _items.Any(i => string.Equals(i.InventoryNumber.Trim(), key, StringComparison.OrdinalIgnoreCase));
    }

    public IReadOnlyList<IInventoriable> AllItems => _items.AsReadOnly();

    public IReadOnlyList<Transport> AllTransports =>
        _items.OfType<Transport>().ToList().AsReadOnly();

    public IReadOnlyList<Thing> AllThings =>
        _items.OfType<Thing>().ToList().AsReadOnly();

    public int UnitCount => _items.Count;

    public double TotalDailyEnergyKWh =>
        _items.OfType<IEnergyConsumer>().Sum(c => c.DailyEnergyConsumptionKWh);

    /// <summary>Everything with a beginner rating (transport and things) that is simple enough for beginners.</summary>
    public IReadOnlyList<IBeginnerRated> BeginnerFriendlyItems =>
        _items.OfType<IBeginnerRated>()
              .Where(i => i.SimplicityForBeginner >= BeginnerSimplicityLimit)
              .ToList()
              .AsReadOnly();
}
