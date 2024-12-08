using System.Drawing;
using System.Runtime.CompilerServices;

namespace AccessibleColors;

/// <summary>
/// Provides functionality to generate a series (ramp) of WCAG-compliant colors
/// derived from a single base color. This is useful for creating accessible UI themes
/// with multiple related colors (e.g., hover, pressed states) all maintaining proper contrast.
/// 
/// Strategy:
/// 1. Convert the base color to LCH (LAB-based) color space.
/// 2. Determine whether to lighten (dark mode) or darken (light mode) the base color.
/// 3. For each step, start with a linear guess for L (lightness), then perform a minimal binary search
///    and minor hue/chroma adjustments if necessary to ensure each color meets the required WCAG ratio.
/// </summary>
public static class ColorRampGenerator
{
    private const double RequiredRatio = 4.5;
    private const float Xn = 0.95047f;
    private const float Yn = 1.00000f;
    private const float Zn = 1.08883f;

    /// <summary>
    /// Generates a WCAG-compliant ramp from a base color. Each color in the ramp
    /// is adjusted to ensure it meets at least a 4.5:1 contrast ratio against a chosen background.
    /// 
    /// The ramp is created by:
    /// - Converting the base color to LCH color space.
    /// - Determining a compliant end lightness (L) value by a quick search.
    /// - Interpolating between the original and the compliant L value for each step.
    /// - Performing a small binary search and minimal adjustments on each step if needed.
    /// 
    /// This approach is relatively fast and ensures compliance for each generated color.
    /// </summary>
    /// <param name="baseColor">The starting base color from which the ramp will be derived.</param>
    /// <param name="steps">The number of colors to generate in the ramp. Must be greater than 0.</param>
    /// <param name="darkMode">
    /// If true, indicates the background is dark and the ramp should produce lighter colors.
    /// If false, the background is considered light and the ramp will produce darker colors.
    /// </param>
    /// <returns>
    /// An <see cref="IReadOnlyList{Color}"/> of length <paramref name="steps"/> containing colors
    /// that each meet the WCAG contrast requirements against the chosen background (dark or light).
    /// Returns an empty list if <paramref name="steps"/> ≤ 0.
    /// </returns>
    public static IReadOnlyList<Color> GenerateAccessibleRamp(Color baseColor, int steps, bool darkMode)
    {
        if (steps <= 0)
            return [];

        Color bg = darkMode ? Color.FromArgb(32, 32, 32) : Color.White;
        float bgLum = GetLuminance(bg);

        var (labL, labA, labB) = RGBToLAB(baseColor);
        var (origL, origC, origH) = LABToLCH(labL, labA, labB);

        float endL = FindCompliantL(origL, origC, origH, bgLum, darkMode);

        var result = new Color[steps];
        for (int i = 0; i < steps; i++)
        {
            float t = (float)i / (steps - 1);
            float guessL = origL + (endL - origL) * t;

            // Ensure compliance for each color:
            Color c = AttemptCompliance(guessL, origC, origH, bgLum, darkMode);
            result[i] = c;
        }

        return result;
    }

    /// <summary>
    /// Attempts to produce a compliant color for a given L, C, H by:
    /// 1. Checking the initial guess.
    /// 2. Doing a minimal binary search on L if not compliant.
    /// 3. If still not compliant, trying small C (chroma) adjustments.
    /// 4. Lastly, trying small H (hue) shifts if needed.
    /// 
    /// Returns the best compliant color found or the fallback color if none achieve compliance.
    /// </summary>
    /// <param name="L">The initial L (lightness) value guessed for this step.</param>
    /// <param name="C">The current chroma value.</param>
    /// <param name="H">The current hue value.</param>
    /// <param name="bgLum">The background luminance.</param>
    /// <param name="darkMode">Indicates whether we're targeting a dark background or a light one.</param>
    /// <returns>A compliant color if possible, or the fallback color with the given L, C, H.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Color AttemptCompliance(float L, float C, float H, float bgLum, bool darkMode)
    {
        if (CheckCompliance(L, C, H, bgLum)) return LCHToRGB(L, C, H);

        float minL = darkMode ? L : 0f;
        float maxL = darkMode ? 100f : L;
        for (int attempt = 0; attempt < 2; attempt++)
        {
            float midL = (minL + maxL) * 0.5f;
            if (CheckCompliance(midL, C, H, bgLum))
            {
                L = midL;
                break;
            }
            else
            {
                double ratio = GetRatio(midL, C, H, bgLum);
                if (darkMode)
                {
                    // Need lighter if ratio < RequiredRatio
                    if (ratio < RequiredRatio) minL = midL; else maxL = midL;
                }
                else
                {
                    // Need darker if ratio < RequiredRatio
                    if (ratio < RequiredRatio) maxL = midL; else minL = midL;
                }
                L = midL;
            }
        }

        if (CheckCompliance(L, C, H, bgLum)) return LCHToRGB(L, C, H);

        // Adjust chroma:
        for (int attempt = 0; attempt < 2; attempt++)
        {
            float decC = MathF.Max(C - 5f, 0f);
            if (CheckCompliance(L, decC, H, bgLum)) return LCHToRGB(L, decC, H);

            float incC = MathF.Min(C + 5f, 100f);
            if (CheckCompliance(L, incC, H, bgLum)) return LCHToRGB(L, incC, H);
        }

        // Adjust hue:
        for (int attempt = 0; attempt < 2; attempt++)
        {
            float plusH = (H + 2f) % 360f;
            if (CheckCompliance(L, C, plusH, bgLum)) { return LCHToRGB(L, C, plusH); }

            float minusH = (H - 2f + 360f) % 360f;
            if (CheckCompliance(L, C, minusH, bgLum)) { return LCHToRGB(L, C, minusH); }
        }

        // Fallback if no adjustments yield compliance:
        return LCHToRGB(L, C, H);
    }

    /// <summary>
    /// Calculates the contrast ratio between two luminance values according to WCAG.
    /// </summary>
    /// <param name="L1">Luminance of the first color.</param>
    /// <param name="L2">Luminance of the second color.</param>
    /// <returns>The WCAG contrast ratio (≥1.0), where higher is better contrast.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static double CalculateContrastRatio(float L1, float L2)
    {
        float lighter = (L1 > L2) ? L1 : L2;
        float darker = (L1 < L2) ? L1 : L2;
        return (lighter + 0.05) / (darker + 0.05);
    }

    /// <summary>
    /// Checks if a given L, C, H combination results in a color that meets the required WCAG ratio
    /// against the provided background luminance.
    /// </summary>
    /// <param name="L">Lightness.</param>
    /// <param name="C">Chroma.</param>
    /// <param name="H">Hue.</param>
    /// <param name="bgLum">Background luminance.</param>
    /// <returns>True if compliant, otherwise false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool CheckCompliance(float L, float C, float H, float bgLum)
    {
        double ratio = GetRatio(L, C, H, bgLum);
        return ratio >= RequiredRatio;
    }

    /// <summary>
    /// Finds a compliant L value by scanning upward (in dark mode) or downward (in light mode)
    /// in increments of 5 until a compliant lightness is found, or a limit is reached.
    /// </summary>
    /// <param name="L">Original lightness.</param>
    /// <param name="C">Chroma.</param>
    /// <param name="H">Hue.</param>
    /// <param name="bgLum">Background luminance.</param>
    /// <param name="darkMode">True if aiming for a dark background, else false.</param>
    /// <returns>A compliant L value, or 100f/0f if none found within the range.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float FindCompliantL(float L, float C, float H, float bgLum, bool darkMode)
    {
        float step = 5f;
        if (darkMode)
        {
            for (float testL = L; testL <= 100f; testL += step)
                if (CheckCompliance(testL, C, H, bgLum)) return testL;
            return 100f;
        }
        else
        {
            for (float testL = L; testL >= 0f; testL -= step)
                if (CheckCompliance(testL, C, H, bgLum)) return testL;
            return 0f;
        }
    }

    /// <summary>
    /// Clamps a floating-point value to a byte range [0..255] after rounding.
    /// </summary>
    /// <param name="v">The value to clamp and round.</param>
    /// <returns>A byte representing the clamped result.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static byte ClampToByte(float v)
    {
        int val = (int)MathF.Round(v * 255f);
        if (val < 0) val = 0;
        if (val > 255) val = 255;
        return (byte)val;
    }

    /// <summary>
    /// Converts a given value t to the XYZ-lab intermediate form f(t).
    /// Used for LAB conversions.
    /// </summary>
    /// <param name="t">A normalized XYZ value.</param>
    /// <returns>The adjusted value after applying f(t).</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float Fxyz(float t)
    {
        return (t > 0.008856f) ? MathF.Pow(t, 1f / 3f) : (7.787f * t) + (16f / 116f);
    }

    /// <summary>
    /// Computes the luminance of a given Color by converting its sRGB components to linear space
    /// and applying the standard luminance coefficients.
    /// </summary>
    /// <param name="c">The Color to compute luminance for.</param>
    /// <returns>A float representing the luminance (0.0 to 1.0).</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float GetLuminance(Color c)
    {
        float r = SrgbToLinear(c.R), g = SrgbToLinear(c.G), b = SrgbToLinear(c.B);
        return 0.2126f * r + 0.7152f * g + 0.0722f * b;
    }

    /// <summary>
    /// Computes the contrast ratio for a given L, C, H combination against a background luminance.
    /// </summary>
    /// <param name="L">Lightness.</param>
    /// <param name="C">Chroma.</param>
    /// <param name="H">Hue.</param>
    /// <param name="bgLum">Background luminance.</param>
    /// <returns>The WCAG contrast ratio for the resulting color.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static double GetRatio(float L, float C, float H, float bgLum)
    {
        var rgb = LCHToRGB(L, C, H);
        float fgLum = GetLuminance(rgb);
        return CalculateContrastRatio(bgLum, fgLum);
    }

    /// <summary>
    /// The inverse f(t) function used in LAB conversions.
    /// </summary>
    /// <param name="t">A value in Lab intermediate form.</param>
    /// <returns>The restored XYZ value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float InvFxyz(float t)
    {
        float t3 = t * t * t;
        return (t3 > 0.008856f) ? t3 : (t - (16f / 116f)) / 7.787f;
    }

    /// <summary>
    /// Converts LAB to LCH color space.
    /// </summary>
    /// <param name="L">Lab Lightness.</param>
    /// <param name="a">Lab a component.</param>
    /// <param name="b">Lab b component.</param>
    /// <returns>(L, C, H) representing LCH color space.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static (float L, float C, float H) LABToLCH(float L, float a, float b)
    {
        float C = MathF.Sqrt(a * a + b * b);
        float H = MathF.Atan2(b, a) * (180f / MathF.PI);
        if (H < 0) H += 360f;
        return (L, C, H);
    }

    /// <summary>
    /// Converts LAB to RGB via XYZ conversion.
    /// </summary>
    /// <param name="L">Lab Lightness.</param>
    /// <param name="a">Lab a component.</param>
    /// <param name="b">Lab b component.</param>
    /// <returns>A Color representing the converted RGB value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Color LABToRGB(float L, float a, float b)
    {
        float Y = (L + 16f) / 116f;
        float X = a / 500f + Y;
        float Z = Y - b / 200f;

        X = Xn * InvFxyz(X);
        Y = Yn * InvFxyz(Y);
        Z = Zn * InvFxyz(Z);

        float R = X * 3.2406f + Y * (-1.5372f) + Z * (-0.4986f);
        float G = X * (-0.9689f) + Y * 1.8758f + Z * 0.0415f;
        float B = X * 0.0557f + Y * (-0.2040f) + Z * 1.0570f;

        return Color.FromArgb(ClampToByte(LinearToSrgb(R)),
                              ClampToByte(LinearToSrgb(G)),
                              ClampToByte(LinearToSrgb(B)));
    }

    /// <summary>
    /// Converts LCH to LAB color space.
    /// </summary>
    /// <param name="L">LCH Lightness.</param>
    /// <param name="C">LCH Chroma.</param>
    /// <param name="H">LCH Hue.</param>
    /// <returns>(L, a, b) in LAB color space.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static (float L, float a, float b) LCHToLAB(float L, float C, float H)
    {
        float hRad = H * MathF.PI / 180f;
        float A = C * MathF.Cos(hRad);
        float B = C * MathF.Sin(hRad);
        return (L, A, B);
    }

    /// <summary>
    /// Converts LCH to RGB by first converting LCH to LAB, then LAB to RGB.
    /// </summary>
    /// <param name="L">LCH Lightness.</param>
    /// <param name="C">LCH Chroma.</param>
    /// <param name="H">LCH Hue.</param>
    /// <returns>A Color in RGB space.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Color LCHToRGB(float L, float C, float H)
    {
        var (ll, aa, bb) = LCHToLAB(L, C, H);
        return LABToRGB(ll, aa, bb);
    }

    /// <summary>
    /// Converts a linear value to sRGB gamma space.
    /// </summary>
    /// <param name="v">A linearized value (0.0 to 1.0).</param>
    /// <returns>The sRGB gamma-corrected value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float LinearToSrgb(float v)
    {
        if (v <= 0.0f) return 0.0f;
        if (v >= 1.0f) return 1.0f;
        return (v <= 0.0031308f) ? (v * 12.92f) : (1.055f * MathF.Pow(v, 1f / 2.4f) - 0.055f);
    }

    /// <summary>
    /// Converts an RGB Color to LAB color space.
    /// </summary>
    /// <param name="c">The input RGB Color.</param>
    /// <returns>(L, a, b) in LAB color space.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static (float L, float a, float b) RGBToLAB(Color c)
    {
        float r = SrgbToLinear(c.R), g = SrgbToLinear(c.G), bVal = SrgbToLinear(c.B);
        float X = (r * 0.4124f + g * 0.3576f + bVal * 0.1805f) / Xn;
        float Y = (r * 0.2126f + g * 0.7152f + bVal * 0.0722f) / Yn;
        float Z = (r * 0.0193f + g * 0.1192f + bVal * 0.9505f) / Zn;

        X = Fxyz(X); Y = Fxyz(Y); Z = Fxyz(Z);

        float L = 116f * Y - 16f;
        float A = 500f * (X - Y);
        float B = 200f * (Y - Z);
        return (L, A, B);
    }

    /// <summary>
    /// Converts an sRGB component (0-255) to its linear equivalent.
    /// </summary>
    /// <param name="comp">The sRGB component (R,G,B) as a byte.</param>
    /// <returns>The linearized value between 0.0 and 1.0.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float SrgbToLinear(byte comp)
    {
        float v = comp / 255f;
        return (v <= 0.03928f) ? v / 12.92f : MathF.Pow((v + 0.055f) / 1.055f, 2.4f);
    }
}
