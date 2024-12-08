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
    //private const double RequiredRatio = 4.5;

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

        bool blackPasses = ratioBlack >= ColorUtilities.RequiredRatioNormalText;
        bool whitePasses = ratioWhite >= ColorUtilities.RequiredRatioNormalText;

        if (blackPasses && whitePasses)
            return (ratioBlack > ratioWhite) ? Color.Black : Color.White;
        if (blackPasses) return Color.Black;
        if (whitePasses) return Color.White;

        return (ratioBlack > ratioWhite) ? Color.Black : Color.White;
    }

    /// <summary>
    /// Returns a WCAG-compliant foreground color for the specified background color,
    /// taking into account text size and weight, which may lower the required ratio from 4.5:1 to 3:1.
    /// This ensures that large or bold large text can choose a suitable foreground just like normal text scenarios.
    /// </summary>
    /// <param name="background">The background color.</param>
    /// <param name="textSizePt">Text size in points.</param>
    /// <param name="isBold">True if text is bold. 14pt bold is considered large text.</param>
    /// <returns>A color meeting or exceeding the required ratio for the given text size and weight.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Color GetContrastColorForText(this Color background, double textSizePt, bool isBold)
    {
        double required = ColorUtilities.GetRequiredRatioForText(textSizePt, isBold);
        return ColorUtilities.GetContrastColorWithRatio(background, required);
    }

    /// <summary>
    /// Determines whether the specified foreground and background color combination meets a given WCAG contrast ratio requirement.
    /// By default, it checks against the normal text standard (4.5:1).
    /// </summary>
    /// <param name="background">The background <see cref="Color"/> to test against.</param>
    /// <param name="foreground">The foreground <see cref="Color"/> to verify for compliance.</param>
    /// <param name="requiredRatio">
    /// The required WCAG contrast ratio. Defaults to <see cref="ColorUtilities.RequiredRatioNormalText"/> (4.5).
    /// </param>
    /// <returns>
    /// <c>true</c> if the contrast ratio between <paramref name="background"/> and <paramref name="foreground"/> 
    /// meets or exceeds <paramref name="requiredRatio"/>; otherwise, <c>false</c>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsCompliant(Color background, Color foreground, double requiredRatio = ColorUtilities.RequiredRatioNormalText)
    {
        float L_bg = ColorUtilities.GetLuminance(background);
        float L_fg = ColorUtilities.GetLuminance(foreground);
        double ratio = ColorUtilities.CalculateContrastRatio(L_bg, L_fg);
        return ratio >= requiredRatio;
    }

    /// <summary>
    /// Determines if a given foreground and background color combination meets WCAG requirements,
    /// considering the text size (in points) and whether it's bold.
    /// 
    /// WCAG rules:
    /// - Normal text: ≥4.5:1
    /// - Large text (≥18pt or ≥14pt bold): ≥3:1
    /// </summary>
    /// <param name="background">The background color.</param>
    /// <param name="foreground">The foreground (text) color.</param>
    /// <param name="textSizePt">Text size in points. E.g., 18 pt for large text.</param>
    /// <param name="isBold">True if the text is bold. 14pt bold is considered large text.</param>
    /// <returns>True if compliant under WCAG criteria for the given text size/weight, otherwise false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsTextCompliant(Color background, Color foreground, double textSizePt, bool isBold)
    {
        double required = ColorUtilities.GetRequiredRatioForText(textSizePt, isBold);
        return IsCompliant(background, foreground, required);
    }
}
