using VyshKat.Domain.Entities.Transports;

namespace VyshKat.Domain.Interfaces;

/// <summary>
/// Technical inspection that decides whether new transport may enter the fleet.
/// It is a dependency of <see cref="VyshKat.Application.Services.FleetService"/>, so the inspection
/// rules can be replaced (or mocked in tests) without touching the fleet code.
/// </summary>
public interface IServiceCenter
{
    /// <summary>
    /// Inspects a vehicle.
    /// </summary>
    /// <param name="transport">The vehicle to inspect.</param>
    /// <returns><c>true</c> if the vehicle is accepted, <c>false</c> if it is declined.</returns>
    bool Inspect(Transport transport);
}