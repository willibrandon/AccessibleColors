using System.Drawing;
using System.Runtime.CompilerServices;

namespace AccessibleColors;

public static class ColorRampGenerator
{
    private const double RequiredRatio = 4.5;
    private const float Xn = 0.95047f; private const float Yn = 1.00000f; private const float Zn = 1.08883f;

    /// <summary>
    /// Generates a WCAG-compliant ramp from a base color as fast as possible.
    /// Strategy:
    /// - Convert base color to LCH once.
    /// - Decide direction based on darkMode.
    /// - For each step, guess L linearly, do a short binary search on L to find compliance.
    /// - If still not compliant, try a couple of small C or H adjustments.
    /// 
    /// This won't be nanoseconds, but it's relatively fast (small fixed number of attempts).
    /// </summary>
    public static IReadOnlyList<Color> GenerateAccessibleRamp(Color baseColor, int steps, bool darkMode)
    {
        if (steps <= 0)
            return [];

        Color bg = darkMode ? Color.FromArgb(32, 32, 32) : Color.White;
        float bgLum = GetLuminance(bg);

        // Convert baseColor to LCH
        var (labL, labA, labB) = RGBToLAB(baseColor);
        var (origL, origC, origH) = LABToLCH(labL, labA, labB);

        // Determine a target L range:
        // For darkMode, we want to go lighter, for lightMode, go darker.
        // We'll just pick a target end L that ensures compliance at the extremes.
        // Try finding a compliant L for the last step:
        float endL = FindCompliantL(origL, origC, origH, bgLum, darkMode);
        // We'll linearly interpolate L from origL to endL across steps.

        var result = new Color[steps];
        for (int i = 0; i < steps; i++)
        {
            float t = (float)i / (steps - 1);
            float guessL = origL + (endL - origL) * t;

            // Ensure compliance by quick binary search on L:
            Color c = AttemptCompliance(guessL, origC, origH, bgLum, darkMode);
            result[i] = c;
        }

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Color AttemptCompliance(float L, float C, float H, float bgLum, bool darkMode)
    {
        // Check initial guess:
        if (CheckCompliance(L, C, H, bgLum)) return LCHToRGB(L, C, H);

        // Binary search on L:
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
                // Decide direction:
                if (darkMode)
                {
                    // If not compliant, likely need to be lighter:
                    double ratio = GetRatio(midL, C, H, bgLum);
                    // If ratio < required, go lighter:
                    if (ratio < RequiredRatio) minL = midL; else maxL = midL;
                }
                else
                {
                    // LightMode: likely need darker:
                    double ratio = GetRatio(midL, C, H, bgLum);
                    if (ratio < RequiredRatio) maxL = midL; else minL = midL;
                }
                L = midL;
            }
        }

        if (CheckCompliance(L, C, H, bgLum)) return LCHToRGB(L, C, H);

        // Try small adjustments of C:
        for (int attempt = 0; attempt < 2; attempt++)
        {
            float decC = MathF.Max(C - 5f, 0f);
            if (CheckCompliance(L, decC, H, bgLum)) { return LCHToRGB(L, decC, H); }
            float incC = MathF.Min(C + 5f, 100f);
            if (CheckCompliance(L, incC, H, bgLum)) { return LCHToRGB(L, incC, H); }
        }

        // Try small hue shifts:
        for (int attempt = 0; attempt < 2; attempt++)
        {
            float plusH = (H + 2f) % 360f;
            if (CheckCompliance(L, C, plusH, bgLum)) { H = plusH; return LCHToRGB(L, C, H); }

            float minusH = (H - 2f + 360f) % 360f;
            if (CheckCompliance(L, C, minusH, bgLum)) { H = minusH; return LCHToRGB(L, C, H); }
        }

        // If still not compliant, return best attempt:
        // Just return LCHToRGB(L,C,H) as fallback.
        return LCHToRGB(L, C, H);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static double CalculateContrastRatio(float L1, float L2)
    {
        float lighter = (L1 > L2) ? L1 : L2;
        float darker = (L1 < L2) ? L1 : L2;
        return (lighter + 0.05) / (darker + 0.05);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool CheckCompliance(float L, float C, float H, float bgLum)
    {
        double ratio = GetRatio(L, C, H, bgLum);
        return ratio >= RequiredRatio;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float FindCompliantL(float L, float C, float H, float bgLum, bool darkMode)
    {
        // Try a quick linear search:
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static byte ClampToByte(float v)
    {
        int val = (int)MathF.Round(v * 255f);
        if (val < 0) val = 0;
        if (val > 255) val = 255;
        return (byte)val;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float Fxyz(float t)
    {
        return (t > 0.008856f) ? MathF.Pow(t, 1f / 3f) : (7.787f * t) + (16f / 116f);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float GetLuminance(Color c)
    {
        float r = SrgbToLinear(c.R), g = SrgbToLinear(c.G), b = SrgbToLinear(c.B);
        return 0.2126f * r + 0.7152f * g + 0.0722f * b;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static double GetRatio(float L, float C, float H, float bgLum)
    {
        var rgb = LCHToRGB(L, C, H);
        float fgLum = GetLuminance(rgb);
        return CalculateContrastRatio(bgLum, fgLum);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float InvFxyz(float t)
    {
        float t3 = t * t * t;
        return (t3 > 0.008856f) ? t3 : (t - (16f / 116f)) / 7.787f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static (float L, float C, float H) LABToLCH(float L, float a, float b)
    {
        float C = MathF.Sqrt(a * a + b * b);
        float H = MathF.Atan2(b, a) * (180f / MathF.PI);
        if (H < 0) H += 360f;
        return (L, C, H);
    }

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static (float L, float a, float b) LCHToLAB(float L, float C, float H)
    {
        float hRad = H * MathF.PI / 180f;
        float A = C * MathF.Cos(hRad);
        float B = C * MathF.Sin(hRad);
        return (L, A, B);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Color LCHToRGB(float L, float C, float H)
    {
        var (ll, aa, bb) = LCHToLAB(L, C, H);
        return LABToRGB(ll, aa, bb);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float LinearToSrgb(float v)
    {
        if (v <= 0.0f) return 0.0f;
        if (v >= 1.0f) return 1.0f;
        return (v <= 0.0031308f) ? (v * 12.92f) : (1.055f * MathF.Pow(v, 1f / 2.4f) - 0.055f);
    }

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float SrgbToLinear(byte comp)
    {
        float v = comp / 255f;
        return (v <= 0.03928f) ? v / 12.92f : MathF.Pow((v + 0.055f) / 1.055f, 2.4f);
    }
}
