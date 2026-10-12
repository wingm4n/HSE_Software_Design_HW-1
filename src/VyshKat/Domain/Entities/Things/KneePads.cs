namespace VyshKat.Domain.Entities.Things;

/// <summary>A pair of knee pads; protects the knees.</summary>
public sealed class KneePads : SafetyThing
{
    /// <summary>
    /// Creates a pair of knee pads.
    /// </summary>
    /// <param name="inventoryNumber">Inventory number, must not be blank.</param>
    /// <param name="size">Size label (for example S / M / L), must not be blank.</param>
    /// <exception cref="ArgumentException">Inventory number or size is blank.</exception>
    public KneePads(string inventoryNumber, string size)
        : base(inventoryNumber, "Knee pads", size, "Knees") { }

    /// <summary>Repair description for knee pads.</summary>
    public override string Repair() => "Repairing a pair of damaged Knee pads";
}