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

    // Precompute LUT for sRGB -> linear once.
    private static readonly float[] sRGBToLinear;

    // Pointer to LUT data for unsafe indexing.
    private static readonly IntPtr sRGBToLinearPtr;

    static WcagContrastColor()
    {
        sRGBToLinear = new float[256];
        for (int i = 0; i < 256; i++)
        {
            float v = i / 255f;
            sRGBToLinear[i] = NormalizeSrgbComponent(v);
        }

        unsafe
        {
            fixed (float* p = sRGBToLinear)
            {
                sRGBToLinearPtr = (IntPtr)p;
            }
        }
    }

    /// <summary>
    /// Returns a WCAG-compliant foreground color for the specified background color.
    /// By default, it ensures a ratio of at least 4.5:1, suitable for normal text.
    /// If both black and white pass, it chooses the one with the higher contrast ratio.
    /// Otherwise, it returns the first one that passes. If neither passes,
    /// it returns the one with the best (though insufficient) ratio.
    /// </summary>
    /// <param name="background">The background <see cref="Color"/> to find a suitable foreground for.</param>
    /// <returns>A <see cref="Color"/> that meets or exceeds the standard WCAG contrast ratio of 4.5:1.</returns>

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Color GetContrastColor(this Color background)
    {
        // Inline R/G/B extraction from ARGB
        int bgVal = background.ToArgb(); // ARGB: A in high byte, B in lowest byte
        byte B = (byte)(bgVal);
        byte G = (byte)(bgVal >> 8);
        byte R = (byte)(bgVal >> 16);
        // A = (byte)(bgVal >> 24); // Not needed

        // Unsafe direct indexing into LUT
        unsafe
        {
            float* lut = (float*)sRGBToLinearPtr;
            float R_lin = lut[R];
            float G_lin = lut[G];
            float B_lin = lut[B];

            float L_bg = 0.2126f * R_lin + 0.7152f * G_lin + 0.0722f * B_lin;

            // Calculate ratios with black and white
            double ratioBlack = CalculateContrastRatio(L_bg, 0.0f);
            double ratioWhite = CalculateContrastRatio(L_bg, 1.0f);

            bool blackPasses = ratioBlack >= RequiredRatio;
            bool whitePasses = ratioWhite >= RequiredRatio;

            if (blackPasses && whitePasses)
                return (ratioBlack > ratioWhite) ? Color.Black : Color.White;
            if (blackPasses) return Color.Black;
            if (whitePasses) return Color.White;

            // If neither passes, pick the best. Rare scenario.
            return (ratioBlack > ratioWhite) ? Color.Black : Color.White;
        }
    }

    /// <summary>
    /// Determines whether the specified foreground color meets or exceeds
    /// the given WCAG contrast ratio when drawn over the specified background color.
    /// </summary>
    /// <param name="background">The background color to test against.</param>
    /// <param name="foreground">The foreground color to verify.</param>
    /// <param name="requiredRatio">
    /// The required WCAG contrast ratio. Defaults to 4.5, which is the standard for normal text.
    /// </param>
    /// <returns>
    /// <c>true</c> if the contrast ratio between <paramref name="background"/> and <paramref name="foreground"/>
    /// is greater than or equal to <paramref name="requiredRatio"/>; otherwise <c>false</c>.
    /// </returns>
    public static bool IsCompliant(Color background, Color foreground, double requiredRatio = RequiredRatio)
    {
        float L_bg = GetLuminance(background);
        float L_fg = GetLuminance(foreground);
        double ratio = CalculateContrastRatio(L_bg, L_fg);
        return ratio >= requiredRatio;
    }

    /// <summary>
    /// Calculates the contrast ratio between two luminance values according to WCAG.
    /// </summary>
    /// <param name="L1">The luminance of the first color.</param>
    /// <param name="L2">The luminance of the second color.</param>
    /// <returns>
    /// The contrast ratio, a value >= 1.0, where higher indicates greater contrast.
    /// </returns>

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static double CalculateContrastRatio(float L1, float L2)
    {
        float lighter = (L1 > L2) ? L1 : L2;
        float darker = (L1 < L2) ? L1 : L2;
        return (lighter + 0.05) / (darker + 0.05);
    }

    /// <summary>
    /// Computes the relative luminance of a given <see cref="Color"/> using the sRGB to linear conversion table.
    /// </summary>
    /// <param name="c">The color to compute luminance for.</param>
    /// <returns>A float value representing the relative luminance (0.0 to 1.0).</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float GetLuminance(Color c)
    {
        unsafe
        {
            float* lut = (float*)sRGBToLinearPtr;
            float r = lut[c.R];
            float g = lut[c.G];
            float b = lut[c.B];
            return 0.2126f * r + 0.7152f * g + 0.0722f * b;
        }
    }

    /// <summary>
    /// Converts an sRGB component value (0.0 to 1.0) to its linear equivalent for luminance calculations.
    /// </summary>
    /// <param name="value">An sRGB component value between 0.0 and 1.0.</param>
    /// <returns>The linearized component value.</returns>

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float NormalizeSrgbComponent(float value)
    {
        return (value <= 0.03928f)
            ? value / 12.92f
            : MathF.Pow((value + 0.055f) / 1.055f, 2.4f);
    }
}