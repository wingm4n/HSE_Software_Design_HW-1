namespace VyshKat.Domain.Entities.Things;

/// <summary>
/// Protective gear worn by a rider (helmet, knee pads, elbow pads, ...).
/// Adds a size and the protected body part; every concrete kind describes its own repair.
/// </summary>
public abstract class SafetyThing : Thing
{
    /// <summary>Size label of the gear (for example S / M / L).</summary>
    public string Size { get; }
    /// <summary>Body part that this gear protects ("Head", "Knees", ...).</summary>
    public string SavedPartOfTheBody { get; }


    /// <summary>
    /// Creates a piece of safety gear.
    /// </summary>
    /// <param name="inventoryNumber">Inventory number, must not be blank.</param>
    /// <param name="name">Kind name of the gear.</param>
    /// <param name="size">Size label (for example S / M / L), must not be blank.</param>
    /// <param name="savedPartOfTheBody">Protected body part, supplied by the concrete subclass.</param>
    /// <exception cref="ArgumentException">Inventory number, name or size is blank.</exception>
    protected SafetyThing(string inventoryNumber, string name, string size, string savedPartOfTheBody)
        : base(inventoryNumber, name)
    {
        if (string.IsNullOrWhiteSpace(size))
            throw new ArgumentException("Size cannot be empty.", nameof(size));

        Size = size;
        SavedPartOfTheBody = savedPartOfTheBody;
    }

    /// <summary>Base description followed by the size and the protected body part.</summary>
    public override string ToString() => $"{base.ToString()} (size {Size}) (saves {SavedPartOfTheBody})";

    /// <summary>Describes the repair of this kind of gear. Polymorphic: each concrete kind returns its own text.</summary>
    public abstract string Repair();
}