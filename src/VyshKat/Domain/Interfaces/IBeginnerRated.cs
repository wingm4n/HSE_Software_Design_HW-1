namespace VyshKat.Domain.Interfaces;

/// <summary>
/// An inventory item that has a "simplicity for a beginner" rating.
/// Whether a rating is good enough is a business rule, see FleetService.BeginnerSimplicityLimit.
/// </summary>
public interface IBeginnerRated : IInventoriable
{
    /// <summary>Lowest allowed beginner rating.</summary>
    const int MinSimplicity = 1;
    /// <summary>Highest allowed beginner rating.</summary>
    const int MaxSimplicity = 10;

    /// <summary>How easy the item is for a beginner, from <see cref="MinSimplicity"/> to <see cref="MaxSimplicity"/>.</summary>
    int SimplicityForBeginner { get; }
}
