using System.Drawing;

namespace AccessibleColors;

/// <summary>
/// Provides functionality to generate a series (ramp) of WCAG-compliant colors
/// derived from a single base color.
/// </summary>
public static class ColorRampGenerator
{
    /// <summary>
    /// Generates a WCAG-compliant color ramp from a specified base color, producing a series of colors 
    /// that each meet at least a 4.5:1 contrast ratio against an implied background.
    ///
    /// If <paramref name="darkMode"/> is true, the ramp is tuned for accessibility against a dark 
    /// background (specifically <c>Color.FromArgb(32, 32, 32)</c>). If false, the ramp is tuned for 
    /// accessibility against a light background (specifically <c>Color.White</c>).
    ///
    /// This method now guarantees compliance. If the base color and minimal adjustments fail, it will
    /// fall back to more extensive searching and, as a last resort, choose a known compliant color 
    /// (black or white).
    /// </summary>
    /// <param name="baseColor">
    /// The starting base color from which to derive the accessible ramp.
    /// </param>
    /// <param name="steps">
    /// The number of colors (steps) to include in the generated ramp.
    /// Each step in the ramp is derived between the original color and a compliant target to ensure 
    /// consistent progression.
    /// </param>
    /// <param name="darkMode">
    /// If true, the generated ramp will be accessible against a dark background 
    /// (<c>Color.FromArgb(32, 32, 32)</c>). If false, it will be accessible against a light background 
    /// (<c>Color.White</c>).
    /// </param>
    /// <returns>
    /// A list of <see cref="Color"/> objects forming a compliant color ramp. Each color in the ramp 
    /// meets at least a 4.5:1 contrast ratio against the chosen background. If necessary, the algorithm 
    /// adjusts and falls back to a guaranteed compliant color to ensure that every color returned 
    /// is WCAG-compliant.
    /// </returns>
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

            Color c = ColorUtilities.AttemptComplianceGuaranteed(guessL, origC, origH, bgLum, darkMode);
            result[i] = c;
        }

        return result;
    }
}
