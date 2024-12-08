using System.Drawing;
using System.Runtime.CompilerServices;

namespace AccessibleColors;

/// <summary>
/// Provides O(1) methods to compute WCAG-compliant contrast colors.
/// Given a background color, returns a foreground color that meets or
/// exceeds the WCAG 2.2 contrast ratio threshold (default 4.5:1).
/// </summary>
public static class WcagContrastColor
{
    private const double RequiredRatio = 4.5;

    /// <summary>
    /// Returns a WCAG-compliant foreground color for the specified background color.
    /// By default, ensures a ratio of at least 4.5:1.
    /// If both black and white pass, chooses the one with higher contrast ratio.
    /// Otherwise, returns the best passing or fallback color.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Color GetContrastColor(this Color background)
    {
        // Inline R/G/B extraction from ARGB
        int bgVal = background.ToArgb();
        byte B = (byte)(bgVal);
        byte G = (byte)(bgVal >> 8);
        byte R = (byte)(bgVal >> 16);

        float bgLum = ColorUtilities.GetLuminance(Color.FromArgb(R, G, B));

        // Ratios with black and white
        double ratioBlack = ColorUtilities.CalculateContrastRatio(bgLum, 0.0f);
        double ratioWhite = ColorUtilities.CalculateContrastRatio(bgLum, 1.0f);

        bool blackPasses = ratioBlack >= RequiredRatio;
        bool whitePasses = ratioWhite >= RequiredRatio;

        if (blackPasses && whitePasses)
            return (ratioBlack > ratioWhite) ? Color.Black : Color.White;
        if (blackPasses) return Color.Black;
        if (whitePasses) return Color.White;

        return (ratioBlack > ratioWhite) ? Color.Black : Color.White;
    }

    /// <summary>
    /// Determines whether the specified foreground color meets/exceeds
    /// the given WCAG contrast ratio over the specified background color.
    /// </summary>
    public static bool IsCompliant(Color background, Color foreground, double requiredRatio = RequiredRatio)
    {
        float L_bg = ColorUtilities.GetLuminance(background);
        float L_fg = ColorUtilities.GetLuminance(foreground);
        double ratio = ColorUtilities.CalculateContrastRatio(L_bg, L_fg);
        return ratio >= requiredRatio;
    }
}
