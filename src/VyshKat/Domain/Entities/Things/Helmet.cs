namespace VyshKat.Domain.Entities.Things;

/// <summary>A helmet; protects the head.</summary>
public sealed class Helmet : SafetyThing
{
    /// <summary>
    /// Creates a helmet.
    /// </summary>
    /// <param name="inventoryNumber">Inventory number, must not be blank.</param>
    /// <param name="size">Size label (for example S / M / L), must not be blank.</param>
    /// <exception cref="ArgumentException">Inventory number or size is blank.</exception>
    public Helmet(string inventoryNumber, string size)
        : base(inventoryNumber, "Helmet", size, "Head") { }

    /// <summary>Repair description for a helmet.</summary>
    public override string Repair() => "Repairing a damaged Helmet";
}