namespace VyshKat.Domain.Entities.Transports;

public sealed class Bicycle : Transport
{
    public Bicycle(string inventoryNumber, int simplicityForBeginner)
        : base(inventoryNumber, "Bicycle", simplicityForBeginner)
    {
    }

    public override string ToString() =>
        $"{base.ToString()} (just a Bicycle)";
}