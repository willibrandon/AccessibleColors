using System.Drawing;
using System.Runtime.CompilerServices;

namespace AccessibleColors;

/// <summary>
/// Provides common color-related utilities and conversions, including sRGB-to-linear,
/// LAB/LCH conversions, and WCAG contrast calculations.
/// This class centralizes shared logic used by WcagContrastColor and ColorRampGenerator.
/// </summary>
internal static class ColorUtilities
{
    internal const double RequiredRatioNormalText = 4.5;
    internal const double RequiredRatioLargeText = 3.0;
    internal const double RequiredRatioUIElement = 3.0;

    private const float Xn = 0.95047f;
    private const float Yn = 1.00000f;
    private const float Zn = 1.08883f;

    // Precompute LUT for sRGB -> linear once
    private static readonly float[] sRGBToLinear;

    static ColorUtilities()
    {
        sRGBToLinear = new float[256];
        for (int i = 0; i < 256; i++)
        {
            float v = i / 255f;
            sRGBToLinear[i] = NormalizeSrgbComponent(v);
        }
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
    internal static Color AttemptCompliance(float L, float C, float H, float bgLum, bool darkMode)
    {
        if (CheckCompliance(L, C, H, bgLum, RequiredRatioNormalText))
            return LCHToRGB(L, C, H);

        float minL = darkMode ? L : 0f;
        float maxL = darkMode ? 100f : L;
        for (int attempt = 0; attempt < 2; attempt++)
        {
            float midL = (minL + maxL) * 0.5f;
            if (CheckCompliance(midL, C, H, bgLum, RequiredRatioNormalText))
            {
                L = midL;
                break;
            }
            else
            {
                double ratio = GetRatio(midL, C, H, bgLum);
                if (darkMode)
                {
                    // Need lighter if ratio < RequiredRatioNormalText
                    if (ratio < RequiredRatioNormalText) minL = midL; else maxL = midL;
                }
                else
                {
                    // Need darker if ratio < RequiredRatioNormalText
                    if (ratio < RequiredRatioNormalText) maxL = midL; else minL = midL;
                }
                L = midL;
            }
        }

        if (CheckCompliance(L, C, H, bgLum, RequiredRatioNormalText))
            return LCHToRGB(L, C, H);

        // Adjust chroma:
        for (int attempt = 0; attempt < 2; attempt++)
        {
            float decC = MathF.Max(C - 5f, 0f);
            if (CheckCompliance(L, decC, H, bgLum, RequiredRatioNormalText))
                return LCHToRGB(L, decC, H);

            float incC = MathF.Min(C + 5f, 100f);
            if (CheckCompliance(L, incC, H, bgLum, RequiredRatioNormalText))
                return LCHToRGB(L, incC, H);
        }

        // Adjust hue:
        for (int attempt = 0; attempt < 2; attempt++)
        {
            float plusH = (H + 2f) % 360f;
            if (CheckCompliance(L, C, plusH, bgLum, RequiredRatioNormalText))
                return LCHToRGB(L, C, plusH);

            float minusH = (H - 2f + 360f) % 360f;
            if (CheckCompliance(L, C, minusH, bgLum, RequiredRatioNormalText))
                return LCHToRGB(L, C, minusH);
        }

        // Fallback
        return LCHToRGB(L, C, H);
    }

    /// <summary>
    /// Calculates the WCAG contrast ratio between two luminance values.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static double CalculateContrastRatio(float L1, float L2)
    {
        float lighter = (L1 > L2) ? L1 : L2;
        float darker = (L1 < L2) ? L1 : L2;
        return (lighter + 0.05f) / (darker + 0.05f);
    }

    /// <summary>
    /// Checks compliance of a given LCH color against a background luminance.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool CheckCompliance(float L, float C, float H, float bgLum, double requiredRatio = 4.5)
    {
        double ratio = GetRatio(L, C, H, bgLum);
        return ratio >= requiredRatio;
    }

    /// <summary>
    /// Clamps a float [0..1] to a byte [0..255].
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static byte ClampToByte(float v)
    {
        int val = (int)MathF.Round(v * 255f);
        if (val < 0) val = 0;
        if (val > 255) val = 255;
        return (byte)val;
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
    internal static float FindCompliantL(float L, float C, float H, float bgLum, bool darkMode)
    {
        float step = 5f;
        if (darkMode)
        {
            for (float testL = L; testL <= 100f; testL += step)
                if (CheckCompliance(testL, C, H, bgLum, RequiredRatioNormalText))
                    return testL;
            return 100f;
        }
        else
        {
            for (float testL = L; testL >= 0f; testL -= step)
                if (CheckCompliance(testL, C, H, bgLum, RequiredRatioNormalText))
                    return testL;
            return 0f;
        }
    }

    /// <summary>
    /// f(t) function used in LAB conversions.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static float Fxyz(float t)
    {
        return (t > 0.008856f) ? MathF.Pow(t, 1f / 3f) : (7.787f * t) + (16f / 116f);
    }

    /// <summary>
    /// Returns a color (black or white) that meets or exceeds the specified 
    /// contrast ratio requirement against the given background color. 
    /// If both black and white pass, it returns the one with the greater contrast. 
    /// If neither strictly meets the requirement, it returns the one with the best ratio.
    /// </summary>
    /// <param name="background">The background <see cref="Color"/> against which to ensure contrast.</param>
    /// <param name="requiredRatio">
    /// The desired WCAG contrast ratio. Typically 4.5 for normal text, or 3.0 for large/bold text.
    /// </param>
    /// <returns>
    /// A <see cref="Color"/> (either black or white) that provides the highest 
    /// possible contrast ratio, meeting or exceeding the specified requirement if possible.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Color GetContrastColorWithRatio(Color background, double requiredRatio)
    {
        float bgLum = GetLuminance(background);
        double ratioBlack = CalculateContrastRatio(bgLum, 0.0f);
        double ratioWhite = CalculateContrastRatio(bgLum, 1.0f);

        bool blackPasses = ratioBlack >= requiredRatio;
        bool whitePasses = ratioWhite >= requiredRatio;

        if (blackPasses && whitePasses)
            return (ratioBlack > ratioWhite) ? Color.Black : Color.White;
        if (blackPasses) return Color.Black;
        if (whitePasses) return Color.White;

        // If neither passes, pick the best:
        return (ratioBlack > ratioWhite) ? Color.Black : Color.White;
    }

    /// <summary>
    /// Computes the luminance of a Color using the sRGB->linear LUT.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static float GetLuminance(Color c)
    {
        // Directly access the sRGBToLinear array:
        float r = sRGBToLinear[c.R];
        float g = sRGBToLinear[c.G];
        float b = sRGBToLinear[c.B];
        return 0.2126f * r + 0.7152f * g + 0.0722f * b;
    }

    /// <summary>
    /// Computes ratio of an LCH color against a given background luminance.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static double GetRatio(float L, float C, float H, float bgLum)
    {
        var rgb = LCHToRGB(L, C, H);
        float fgLum = GetLuminance(rgb);
        return CalculateContrastRatio(bgLum, fgLum);
    }

    /// <summary>
    /// Determines the required WCAG contrast ratio based on text size and weight.
    /// - Normal text: ≥4.5:1
    /// - Large text (≥18pt or ≥14pt bold): ≥3:1
    /// </summary>
    /// <param name="textSizePt">Text size in points.</param>
    /// <param name="isBold">True if text is bold. Bold + ≥14pt counts as large text.</param>
    /// <returns>The required WCAG ratio for the given conditions.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static double GetRequiredRatioForText(double textSizePt, bool isBold)
    {
        bool isLarge = (textSizePt >= 18.0) || (isBold && textSizePt >= 14.0);
        return isLarge ? RequiredRatioLargeText : RequiredRatioNormalText;
    }

    /// <summary>
    /// Inverse f(t) for LAB conversions.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static float InvFxyz(float t)
    {
        float t3 = t * t * t;
        return (t3 > 0.008856f) ? t3 : (t - (16f / 116f)) / 7.787f;
    }

    /// <summary>
    /// Converts LAB to LCH.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static (float L, float C, float H) LABToLCH(float L, float a, float b)
    {
        float C = MathF.Sqrt(a * a + b * b);
        float H = MathF.Atan2(b, a) * (180f / MathF.PI);
        if (H < 0) H += 360f;
        return (L, C, H);
    }

    /// <summary>
    /// Converts LAB to RGB.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Color LABToRGB(float L, float a, float b)
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
    /// Converts LCH to LAB.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static (float L, float a, float b) LCHToLAB(float L, float C, float H)
    {
        float hRad = H * MathF.PI / 180f;
        float A = C * MathF.Cos(hRad);
        float B = C * MathF.Sin(hRad);
        return (L, A, B);
    }

    /// <summary>
    /// Converts LCH to RGB by going through LAB.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Color LCHToRGB(float L, float C, float H)
    {
        var (ll, aa, bb) = LCHToLAB(L, C, H);
        return LABToRGB(ll, aa, bb);
    }

    /// <summary>
    /// Converts linear value to sRGB gamma space.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static float LinearToSrgb(float v)
    {
        if (v <= 0.0f) return 0.0f;
        if (v >= 1.0f) return 1.0f;
        return (v <= 0.0031308f) ? (v * 12.92f) : (1.055f * MathF.Pow(v, 1f / 2.4f) - 0.055f);
    }

    /// <summary>
    /// Converts sRGB component from [0..1] to linear.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float NormalizeSrgbComponent(float value)
    {
        return (value <= 0.03928f)
            ? value / 12.92f
            : MathF.Pow((value + 0.055f) / 1.055f, 2.4f);
    }

    /// <summary>
    /// Converts RGB to LAB.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static (float L, float a, float b) RGBToLAB(Color c)
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
    /// Converts an 8-bit sRGB component to linear space using the LUT.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static float SrgbToLinear(byte comp)
    {
        float v = comp / 255f;
        return (v <= 0.03928f) ? v / 12.92f : MathF.Pow((v + 0.055f) / 1.055f, 2.4f);
    }
}
