namespace VyshKat.Domain.Entities.Transports;

/// <summary>
/// A plain (non-electric) bicycle. It consumes no energy, so it deliberately does not implement
/// <see cref="VyshKat.Domain.Interfaces.IEnergyConsumer"/> and is skipped by the energy total.
/// </summary>
public sealed class Bicycle : Transport
{
    /// <summary>
    /// Creates a bicycle with the given inventory number and beginner rating.
    /// </summary>
    /// <param name="inventoryNumber">Inventory number, must not be blank.</param>
    /// <param name="simplicityForBeginner">Beginner rating in the range 1-10.</param>
    /// <exception cref="ArgumentException">Some argument is invalid.</exception>
    public Bicycle(string inventoryNumber, int simplicityForBeginner)
        : base(inventoryNumber, "Bicycle", simplicityForBeginner)
    {
    }

    /// <summary>Base description followed by a note that this is a plain bicycle.</summary>
    public override string ToString() =>
        $"{base.ToString()} (just a Bicycle)";
}