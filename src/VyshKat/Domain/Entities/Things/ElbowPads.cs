namespace VyshKat.Domain.Entities.Things;

/// <summary>A pair of elbow pads; protects the elbows.</summary>
public sealed class ElbowPads : SafetyThing
{
    /// <summary>
    /// Creates a pair of elbow pads.
    /// </summary>
    /// <param name="inventoryNumber">Inventory number, must not be blank.</param>
    /// <param name="size">Size label (for example S / M / L), must not be blank.</param>
    /// <exception cref="ArgumentException">Inventory number or size is blank.</exception>
    public ElbowPads(string inventoryNumber, string size)
        : base(inventoryNumber, "Elbow pads", size, "Elbows") { }

    /// <summary>Repair description for elbow pads.</summary>
    public override string Repair() => "Repairing a pair of damaged Elbow pads";
}