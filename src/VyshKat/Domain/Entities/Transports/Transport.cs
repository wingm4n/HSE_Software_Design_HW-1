using VyshKat.Domain.Interfaces;

namespace VyshKat.Domain.Entities.Transport;

public abstract class Transport : IInventoriable
{
    public string Name { get; }
    public string InventoryNumber { get; }

    public int SimplicityForBeginner { get; }

    protected Transport(string inventoryNumber, string name, int simplicityForBeginner)
    {
        if (string.IsNullOrWhiteSpace(inventoryNumber))
            throw new ArgumentException("Inventory number cannot be empty.", nameof(inventoryNumber));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));
        if (simplicityForBeginner < 1 || simplicityForBeginner > 10)
            throw new ArgumentOutOfRangeException(
                nameof(simplicityForBeginner),
                "Simplicity must be between 1 and 10.");

        InventoryNumber = inventoryNumber;
        Name = name;
        SimplicityForBeginner = simplicityForBeginner;
    }

    public override string ToString() => $"{Name} [{InventoryNumber}]";
}