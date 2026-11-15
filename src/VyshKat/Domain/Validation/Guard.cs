using VyshKat.Domain.Interfaces;

namespace VyshKat.Domain.Validation;

/// <summary>
/// Shared constructor argument checks, so every entity validates the same field the same way.
/// Always throws <see cref="ArgumentException"/> (or a subclass) - the UI relies on that.
/// </summary>
internal static class Guard
{
    /// <summary>
    /// Returns <paramref name="value"/> if it contains non-whitespace characters.
    /// </summary>
    /// <param name="value">Value to check.</param>
    /// <param name="paramName">Name of the checked parameter (goes into the exception).</param>
    /// <param name="displayName">Field name used in the error message.</param>
    /// <exception cref="ArgumentException">The value is null, empty or whitespace.</exception>
    public static string NotBlank(string? value, string paramName, string displayName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{displayName} cannot be empty.", paramName);

        return value;
    }

    /// <summary>Daily energy must be a finite number >= 0 (NaN and Infinity are rejected too).</summary>
    public static double EnergyConsumption(double value, string paramName)
    {
        if (!double.IsFinite(value) || value < 0)
            throw new ArgumentOutOfRangeException(
                paramName, value, "Daily energy consumption must be a non-negative finite number.");

        return value;
    }

    /// <summary>
    /// Returns <paramref name="value"/> if it lies within the beginner-rating range
    /// (<see cref="IBeginnerRated.MinSimplicity"/> to <see cref="IBeginnerRated.MaxSimplicity"/>).
    /// </summary>
    /// <param name="value">Rating to check.</param>
    /// <param name="paramName">Name of the checked parameter.</param>
    /// <exception cref="ArgumentOutOfRangeException">The rating is outside the allowed range.</exception>
    public static int Simplicity(int value, string paramName)
    {
        if (value < IBeginnerRated.MinSimplicity || value > IBeginnerRated.MaxSimplicity)
            throw new ArgumentOutOfRangeException(
                paramName, value,
                $"Simplicity must be between {IBeginnerRated.MinSimplicity} and {IBeginnerRated.MaxSimplicity}.");

        return value;
    }
}
