namespace VyshKat.Domain.Entities.Things;

public sealed class Helmet : SafetyThing
{
    public Helmet(string inventoryNumber, string size)
        : base(inventoryNumber, "Helmet", size, "Head") { }

    public override string Repair() => "Repairing a damaged Helmet";
}