using System.Drawing;

namespace AccessibleColors;

/// <summary>
/// Provides functionality to generate a series (ramp) of WCAG-compliant colors
/// derived from a single base color.
/// </summary>
public static class ColorRampGenerator
{
    /// <summary>
    /// Generates a WCAG-compliant ramp from a base color.
    /// Each color meets at least a 4.5:1 contrast ratio against the chosen background.
    /// </summary>
    public static IReadOnlyList<Color> GenerateAccessibleRamp(Color baseColor, int steps, bool darkMode)
    {
        if (steps <= 0)
            Array.Empty<Color>();

        Color bg = darkMode ? Color.FromArgb(32, 32, 32) : Color.White;
        float bgLum = ColorUtilities.GetLuminance(bg);

        var (labL, labA, labB) = ColorUtilities.RGBToLAB(baseColor);
        var (origL, origC, origH) = ColorUtilities.LABToLCH(labL, labA, labB);

        float endL = ColorUtilities.FindCompliantL(origL, origC, origH, bgLum, darkMode);

        var result = new Color[steps];
        for (int i = 0; i < steps; i++)
        {
            float t = (float)i / (steps - 1);
            float guessL = origL + (endL - origL) * t;

            Color c = ColorUtilities.AttemptCompliance(guessL, origC, origH, bgLum, darkMode);
            result[i] = c;
        }

        return result;
    }
}
