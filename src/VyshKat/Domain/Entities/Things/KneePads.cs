namespace VyshKat.Domain.Entities.Things;

public sealed class KneePads : SafetyThing
{
    public KneePads(string inventoryNumber, string size)
        : base(inventoryNumber, "Knee pads", size, "Knees") { }

    public override string Repair() => "Repairing a pair of damaged Knee pads";
}