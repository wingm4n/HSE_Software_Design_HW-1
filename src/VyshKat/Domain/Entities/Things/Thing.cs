using VyshKat.Domain.Interfaces;
using VyshKat.Domain.Validation;

namespace VyshKat.Domain.Entities.Things;

/// <summary>
/// Base class of non-transport items on the balance (safety gear, chargers, stations, ...).
/// A thing is only inventoriable; extra capabilities (energy, beginner rating) are added by subclasses
/// through the corresponding interfaces. Things are put on balance without a technical inspection.
/// </summary>
public abstract class Thing : IInventoriable
{
    /// <summary>Human-readable kind of the thing ("Helmet", "Charger", ...). Set by the concrete subclass.</summary>
    public string Name { get; }
    /// <summary>Inventory number of this thing (never blank). Uniqueness inside the fleet is checked by <see cref="VyshKat.Application.Services.FleetService"/>.</summary>
    public string InventoryNumber { get; }

    /// <summary>
    /// Validates and stores the fields common to all things.
    /// </summary>
    /// <param name="inventoryNumber">Inventory number, must not be blank.</param>
    /// <param name="name">Kind name, must not be blank.</param>
    /// <exception cref="ArgumentException">Inventory number or name is blank.</exception>
    protected Thing(string inventoryNumber, string name)
    {
        InventoryNumber = Guard.NotBlank(inventoryNumber, nameof(inventoryNumber), "Inventory number");
        Name = Guard.NotBlank(name, nameof(name), "Name");
    }

    /// <summary>Short description in the form <c>Name [InventoryNumber]</c>; subclasses append their own details.</summary>
    public override string ToString() => $"{Name} [{InventoryNumber}]";
}
