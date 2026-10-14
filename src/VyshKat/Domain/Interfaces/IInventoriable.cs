namespace VyshKat.Domain.Interfaces;

/// <summary>
/// Anything that is put on the balance and therefore must have an inventory number
/// (transport and things; staff, for example, deliberately do not implement it).
/// </summary>
public interface IInventoriable
{
    /// <summary>Human-readable name of the item's kind.</summary>
    string Name { get; }
    /// <summary>Inventory number of the item.</summary>
    string InventoryNumber { get; }
}