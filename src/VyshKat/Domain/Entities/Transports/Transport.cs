using VyshKat.Domain.Interfaces;
using VyshKat.Domain.Validation;

namespace VyshKat.Domain.Entities.Transports;

/// <summary>
/// Base class of every rentable vehicle (scooter, bicycle, e-bike, ...).
/// Holds the data common to all kinds: inventory number, display name and the
/// "simplicity for a beginner" rating. Kind-specific traits (for example energy consumption)
/// are added by subclasses through extra interfaces, so a new kind of vehicle is just a new subclass.
/// </summary>
public abstract class Transport : IBeginnerRated
{
    /// <summary>Human-readable kind of the vehicle ("Scooter", "Bicycle", ...). Set by the concrete subclass.</summary>
    public string Name { get; }
    /// <summary>Inventory number of this vehicle (never blank). Uniqueness inside the fleet is checked by <see cref="VyshKat.Application.Services.FleetService"/>.</summary>
    public string InventoryNumber { get; }

    /// <summary>How easy the vehicle is for a beginner, from 1 to 10 (validated in the constructor).</summary>
    public int SimplicityForBeginner { get; }

    /// <summary>
    /// Validates and stores the fields common to all vehicles.
    /// </summary>
    /// <param name="inventoryNumber">Inventory number, must not be blank.</param>
    /// <param name="name">Kind name, must not be blank.</param>
    /// <param name="simplicityForBeginner">Beginner rating in the range 1-10.</param>
    /// <exception cref="ArgumentException">Some argument is invalid (an out-of-range rating throws the derived <see cref="ArgumentOutOfRangeException"/>).</exception>
    protected Transport(string inventoryNumber, string name, int simplicityForBeginner)
    {
        InventoryNumber = Guard.NotBlank(inventoryNumber, nameof(inventoryNumber), "Inventory number");
        Name = Guard.NotBlank(name, nameof(name), "Name");
        SimplicityForBeginner = Guard.Simplicity(simplicityForBeginner, nameof(simplicityForBeginner));
    }

    /// <summary>Short description in the form <c>Name [InventoryNumber]</c>; subclasses append their own details.</summary>
    public override string ToString() => $"{Name} [{InventoryNumber}]";
}
