namespace VyshKat.Domain.Entities.Things;

public abstract class SafetyThing : Thing
{
    public string Size { get; }
    public string SavedPartOfTheBody { get; }


    protected SafetyThing(string inventoryNumber, string name, string size, string savedPartOfTheBody)
        : base(inventoryNumber, name)
    {
        if (string.IsNullOrWhiteSpace(size))
            throw new ArgumentException("Size cannot be empty.", nameof(size));

        Size = size;
        SavedPartOfTheBody = savedPartOfTheBody;
    }

    public override string ToString() => $"{base.ToString()} (size {Size}) (saves {SavedPartOfTheBody})";

    public abstract string Repair();
}