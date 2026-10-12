namespace VyshKat.Domain.Interfaces;

/// <summary>
/// An inventory item that has a "simplicity for a beginner" rating.
/// Whether a rating is good enough is a business rule, see FleetService.BeginnerSimplicityLimit.
/// </summary>
public interface IBeginnerRated : IInventoriable
{
    const int MinSimplicity = 1;
    const int MaxSimplicity = 10;

    int SimplicityForBeginner { get; }
}
