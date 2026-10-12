using VyshKat.Domain.Entities.Things;
using VyshKat.Domain.Entities.Transport;
using VyshKat.Domain.Interfaces;

namespace VyshKat.Application.Services;

public sealed class FleetService
{
    private const int BeginnerSimplicityLimit = 6;

    private readonly IServiceCenter _serviceCenter;
    private readonly List<IInventoriable> _items = new();

    public FleetService(IServiceCenter serviceCenter)
    {
        _serviceCenter = serviceCenter ?? throw new ArgumentNullException(nameof(serviceCenter));
    }

    public bool AddTransport(Transport transport)
    {
        if (transport is null)
            throw new ArgumentNullException(nameof(transport));

        if (!_serviceCenter.Inspect(transport))
            return false;

        _items.Add(transport);
        return true;
    }

    public void AddThing(Thing thing)
    {
        if (thing is null)
            throw new ArgumentNullException(nameof(thing));

        _items.Add(thing);
    }


    public IReadOnlyList<IInventoriable> AllItems => _items.AsReadOnly();

    public IReadOnlyList<Transport> AllTransports =>
        _items.OfType<Transport>().ToList().AsReadOnly();

    public IReadOnlyList<Thing> AllThings =>
        _items.OfType<Thing>().ToList().AsReadOnly();

    public int UnitCount => _items.Count;

    public double TotalDailyEnergyKWh =>
        _items.OfType<IEnergyConsumer>().Sum(c => c.DailyEnergyConsumptionKWh);

    public IReadOnlyList<Transport> BeginnerFriendlyTransports =>
        _items.OfType<Transport>()
              .Where(t => t.SimplicityForBeginner >= BeginnerSimplicityLimit)
              .ToList()
              .AsReadOnly();
}