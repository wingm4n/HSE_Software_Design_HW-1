using VyshKat.Domain.Entities.Transports;
using VyshKat.Domain.Interfaces;

namespace VyshKat.Application.Services;

/// <summary>
/// Technical inspection of new transport. Stateless and deterministic: the decision depends
/// only on the vehicle itself, so it is easy to test and easy to replace through <see cref="IServiceCenter"/>.
/// </summary>
/// <remarks>
/// Rule: a vehicle whose daily energy consumption is above <see cref="MaxDailyEnergyKWh"/> is considered
/// faulty (e.g. a damaged battery or charging circuit) and is declined. Vehicles without an energy
/// consumption (bicycles) always pass this rule.
/// Blank inventory numbers do not need a check here: the <see cref="Transport"/> constructor forbids them.
/// Uniqueness of inventory numbers is a property of the whole fleet, so it is checked in <see cref="FleetService"/>.
/// </remarks>
public sealed class ServiceCenter : IServiceCenter
{
    public const double MaxDailyEnergyKWh = 3.0;

    public bool Inspect(Transport transport)
    {
        if (transport is null)
            return false;

        if (transport is IEnergyConsumer consumer && consumer.DailyEnergyConsumptionKWh > MaxDailyEnergyKWh)
            return false;

        return true;
    }
}
