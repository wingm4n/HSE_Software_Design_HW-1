using VyshKat.Domain.Interfaces;
using VyshKat.Domain.Validation;

namespace VyshKat.Domain.Entities.Transports;

public abstract class Transport : IBeginnerRated
{
    public string Name { get; }
    public string InventoryNumber { get; }

    public int SimplicityForBeginner { get; }

    protected Transport(string inventoryNumber, string name, int simplicityForBeginner)
    {
        InventoryNumber = Guard.NotBlank(inventoryNumber, nameof(inventoryNumber), "Inventory number");
        Name = Guard.NotBlank(name, nameof(name), "Name");
        SimplicityForBeginner = Guard.Simplicity(simplicityForBeginner, nameof(simplicityForBeginner));
    }

    public override string ToString() => $"{Name} [{InventoryNumber}]";
}
