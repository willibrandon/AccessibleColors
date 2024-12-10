namespace AccessibleColors.UI;

/// <summary>
/// Provides extension methods for numeric operations.
/// </summary>
public static class NumericExtensions
{
    /// <summary>
    /// Clamps the specified integer value between a minimum and maximum value.
    /// </summary>
    /// <param name="value">The integer value to clamp.</param>
    /// <param name="min">The inclusive minimum bound of the returned value.</param>
    /// <param name="max">The inclusive maximum bound of the returned value.</param>
    /// <returns>
    /// The clamped value, which will be within the range defined by <paramref name="min"/> and <paramref name="max"/>.
    /// If <paramref name="value"/> is less than <paramref name="min"/>, returns <paramref name="min"/>.
    /// If <paramref name="value"/> is greater than <paramref name="max"/>, returns <paramref name="max"/>.
    /// Otherwise, returns <paramref name="value"/>.
    /// </returns>
    public static int Clamp(this int value, int min, int max)
    {
        return Math.Min(Math.Max(value, min), max);
    }
}
