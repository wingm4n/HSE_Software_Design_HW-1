using VyshKat.Domain.Interfaces;
using VyshKat.Domain.Validation;

namespace VyshKat.Domain.Entities.Things;

public abstract class Thing : IInventoriable
{
    public string Name { get; }
    public string InventoryNumber { get; }

    protected Thing(string inventoryNumber, string name)
    {
        InventoryNumber = Guard.NotBlank(inventoryNumber, nameof(inventoryNumber), "Inventory number");
        Name = Guard.NotBlank(name, nameof(name), "Name");
    }

    public override string ToString() => $"{Name} [{InventoryNumber}]";
}
