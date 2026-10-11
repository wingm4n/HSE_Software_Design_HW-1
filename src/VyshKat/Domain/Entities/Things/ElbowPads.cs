namespace VyshKat.Domain.Entities.Things;

public sealed class ElbowPads : SafetyThing
{
    public ElbowPads(string inventoryNumber, string size)
        : base(inventoryNumber, "Elbow pads", size, "Elbows") { }

    public override string Repair() => "Repairing a pair of damaged Elbow pads";
}