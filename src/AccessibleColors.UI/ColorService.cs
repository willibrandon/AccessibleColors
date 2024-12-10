namespace AccessibleColors.UI;

/// <summary>
/// Provides color-related computations and conversions.
/// Separated into its own service class for cleaner architecture.
/// </summary>
public static class ColorService
{
    /// <summary>
    /// Blends a foreground color with a background color based on the foreground's alpha.
    /// </summary>
    /// <param name="fg">Foreground color, possibly transparent.</param>
    /// <param name="bg">Background color to blend onto.</param>
    /// <returns>The resulting fully opaque blended color.</returns>
    public static Color BlendWithBackground(Color fg, Color bg)
    {
        float alpha = fg.A / 255f;
        int r = (int)((fg.R * alpha) + bg.R * (1 - alpha));
        int g = (int)((fg.G * alpha) + bg.G * (1 - alpha));
        int b = (int)((fg.B * alpha) + bg.B * (1 - alpha));
        return Color.FromArgb(255, r.Clamp(0, 255), g.Clamp(0, 255), b.Clamp(0, 255));
    }

    /// <summary>
    /// Changes the brightness of a color by a given factor.
    /// &lt;1 darkens, &gt;1 lightens.
    /// </summary>
    /// <param name="originalColor">The original color.</param>
    /// <param name="factor">The brightness factor.</param>
    /// <returns>The adjusted color.</returns>
    public static Color ChangeBrightness(Color originalColor, float factor)
    {
        RgbToHsl(originalColor, out float h, out float s, out float l);

        if (factor < 1.0f)
        {
            l *= factor;
        }
        else
        {
            l += (1 - l) * (factor - 1f);
        }

        l = Math.Min(Math.Max(l, 0), 1);
        return HslToRgb(h, s, l);
    }

    /// <summary>
    /// Converts an HSL color to an RGB color.
    /// </summary>
    /// <param name="h">Hue [0..1]</param>
    /// <param name="s">Saturation [0..1]</param>
    /// <param name="l">Lightness [0..1]</param>
    /// <returns>The resulting RGB color.</returns>
    public static Color HslToRgb(float h, float s, float l)
    {
        float r, g, b;
        if (Math.Abs(s) < 0.00001f)
        {
            r = g = b = l;
        }
        else
        {
            float q = l < 0.5f ? l * (1f + s) : l + s - l * s;
            float p = 2f * l - q;

            r = HueToRgb(p, q, h + 1f / 3f);
            g = HueToRgb(p, q, h);
            b = HueToRgb(p, q, h - 1f / 3f);
        }

        return Color.FromArgb(
            255,
            ((int)(r * 255f)).Clamp(0, 255),
            ((int)(g * 255f)).Clamp(0, 255),
            ((int)(b * 255f)).Clamp(0, 255)
        );
    }

    /// <summary>
    /// Helper method for HSL to RGB conversion.
    /// </summary>
    private static float HueToRgb(float p, float q, float t)
    {
        if (t < 0f) t += 1f;
        if (t > 1f) t -= 1f;

        if (t < 1f / 6f) return p + (q - p) * 6f * t;
        if (t < 1f / 2f) return q;
        if (t < 2f / 3f) return p + (q - p) * (2f / 3f - t) * 6f;
        return p;
    }

    /// <summary>
    /// Converts an RGB color to HSL representation.
    /// </summary>
    /// <param name="color">The RGB color.</param>
    /// <param name="h">Hue [0..1]</param>
    /// <param name="s">Saturation [0..1]</param>
    /// <param name="l">Lightness [0..1]</param>
    public static void RgbToHsl(Color color, out float h, out float s, out float l)
    {
        float r = color.R / 255f;
        float g = color.G / 255f;
        float b = color.B / 255f;

        float max = Math.Max(r, Math.Max(g, b));
        float min = Math.Min(r, Math.Min(g, b));
        float delta = max - min;

        l = (max + min) / 2f;

        if (Math.Abs(delta) < 0.00001f)
        {
            s = 0f;
            h = 0f;
        }
        else
        {
            s = l < 0.5f ? delta / (max + min) : delta / (2f - max - min);

            float hue;
            if (Math.Abs(max - r) < 0.00001f)
                hue = (g - b) / delta + (g < b ? 6f : 0f);
            else if (Math.Abs(max - g) < 0.00001f)
                hue = (b - r) / delta + 2f;
            else
                hue = (r - g) / delta + 4f;

            h = hue / 6f;
            if (h < 0f) h += 1f;
            if (h > 1f) h -= 1f;
        }
    }
}
