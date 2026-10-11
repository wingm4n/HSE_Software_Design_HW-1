using VyshKat.Domain.Interfaces;
namespace VyshKat.Domain.Entities.Things;

public abstract class Thing : IInventoriable
{
    public string Name { get; }
    public string InventoryNumber { get; }

    protected Thing(string inventoryNumber, string name)
    {
        if (string.IsNullOrWhiteSpace(inventoryNumber))
            throw new ArgumentException("Inventory number cannot be empty.", nameof(name));
        InventoryNumber = inventoryNumber;

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));
        Name = name;
    }

    public override string ToString() => $"{Name} [{InventoryNumber}]";
}