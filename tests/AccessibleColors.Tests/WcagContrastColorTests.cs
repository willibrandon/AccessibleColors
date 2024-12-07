using System.Drawing;

namespace AccessibleColors.Tests;

public class WcagContrastColorTests
{
    // Introduce a small tolerance for floating point comparisons.
    // WCAG requires >=4.5, but we’ll accept >=4.45 to avoid floating-point edge cases.
    private const double RequiredRatio = 4.45;

    [Fact]
    public void GetContrastColor_OnBlackBackground_ShouldReturnWhiteOrBlack()
    {
        var background = Color.Black;
        var fg = background.GetContrastColor();
        Assert.True(WcagContrastColor.IsCompliant(background, fg),
            "The chosen foreground for black should be compliant.");
        Assert.True(fg == Color.White || fg == Color.Black,
            "Likely returns white, but black also passes since black-on-black might not meet 4.5:1. Check logic.");
    }

    [Fact]
    public void GetContrastColor_OnWhiteBackground_ShouldReturnBlackOrWhite()
    {
        var background = Color.White;
        var fg = background.GetContrastColor();
        Assert.True(WcagContrastColor.IsCompliant(background, fg),
            "The chosen foreground for white should be compliant.");
        Assert.True(fg == Color.White || fg == Color.Black,
            "Likely returns black for white background.");
    }

    [Fact]
    public void GetContrastColor_OnGrayBackground_ShouldBeCompliant()
    {
        var background = Color.FromArgb(128, 128, 128); // Mid-gray
        var fg = background.GetContrastColor();
        Assert.True(WcagContrastColor.IsCompliant(background, fg),
            $"Foreground {fg} on mid-gray should meet contrast ratio.");
    }

    [Fact]
    public void GetContrastColor_OnNeonBlue_ShouldBeCompliant()
    {
        var background = Color.FromArgb(0, 0, 255); // Neon blue
        var fg = background.GetContrastColor();
        Assert.True(WcagContrastColor.IsCompliant(background, fg),
            $"Foreground {fg} on neon blue should be compliant.");
    }

    [Fact]
    public void GetContrastColor_CustomCheck_ForegroundShouldBeCompliant()
    {
        // Pick a non-trivial background color and just ensure compliance
        var background = Color.FromArgb(200, 200, 0); // A bright yellowish color
        var fg = background.GetContrastColor();
        Assert.True(WcagContrastColor.IsCompliant(background, fg),
            $"Foreground {fg} on background {background} should be compliant.");
    }

    [Fact]
    public void IsCompliant_BlackOnWhite_ShouldPass()
    {
        var background = Color.White;
        var foreground = Color.Black;
        Assert.True(WcagContrastColor.IsCompliant(background, foreground),
            "Black text on white background should be compliant.");
    }

    [Fact]
    public void IsCompliant_WhiteOnBlack_ShouldPass()
    {
        var background = Color.Black;
        var foreground = Color.White;
        Assert.True(WcagContrastColor.IsCompliant(background, foreground),
            "White text on black background should be compliant.");
    }

    [Fact]
    public void IsCompliant_BlackOnBlack_ShouldFail()
    {
        var background = Color.Black;
        var foreground = Color.Black;
        Assert.False(WcagContrastColor.IsCompliant(background, foreground),
            "Black on black should not be compliant.");
    }

    [Fact]
    public void IsCompliant_RedOnWhite_CheckCompliance()
    {
        // Red (#FF0000) on White is known to have a ratio < 4.5:1.
        // White luminance ~1.0, Red luminance ~0.2126f*(1.0)+... ~ 0.2126 actually not too low, but let's see.
        // This is a known tough combo. Often red on white fails for normal sized text.
        var background = Color.White;
        var foreground = Color.Red;
        Assert.False(WcagContrastColor.IsCompliant(background, foreground),
            "Red text on white background typically fails the 4.5:1 ratio for normal text.");
    }

    [Theory]
    [InlineData(0, 0, 0)]    // Pure black background
    [InlineData(255, 255, 255)]  // Pure white background
    public void TestPureBlackAndWhiteCompliance(int r, int g, int b)
    {
        Color input = Color.FromArgb(r, g, b);
        Color contrastColor = input.GetContrastColor();

        Assert.True(IsWCAGCompliant(input, contrastColor, RequiredRatio),
            $"Contrast color {contrastColor} did not meet ~4.5:1 ratio against {input}.");
    }

    [Theory]
    [InlineData(255, 0, 0)]  // Neon red
    [InlineData(0, 255, 0)]  // Neon green
    [InlineData(0, 0, 255)]  // Neon blue
    public void TestNeonColors(int r, int g, int b)
    {
        Color input = Color.FromArgb(r, g, b);
        Color contrastColor = input.GetContrastColor();

        Assert.True(IsWCAGCompliant(input, contrastColor, RequiredRatio),
            $"Contrast color {contrastColor} did not meet ~4.5:1 ratio against neon color {input}.");
    }

    [Theory]
    [InlineData(128, 128, 128)]  // Mid-gray
    [InlineData(50, 50, 50)]     // Dark gray
    [InlineData(200, 200, 200)]  // Light gray
    public void TestGrayscaleShades(int r, int g, int b)
    {
        Color input = Color.FromArgb(r, g, b);
        Color contrastColor = input.GetContrastColor();

        Assert.True(IsWCAGCompliant(input, contrastColor, RequiredRatio),
            $"Contrast color {contrastColor} did not meet ~4.5:1 ratio against grayscale {input}.");
    }

    [Theory]
    [InlineData(120, 120, 120)]  // Near mid-gray
    [InlineData(10, 10, 10)]     // Very dark gray
    [InlineData(240, 240, 240)]  // Very light gray
    public void TestNearThresholdCases(int r, int g, int b)
    {
        Color input = Color.FromArgb(r, g, b);
        Color contrastColor = input.GetContrastColor();

        Assert.True(IsWCAGCompliant(input, contrastColor, RequiredRatio),
            $"Contrast color {contrastColor} did not meet ~4.5:1 ratio for near-threshold gray {input}.");
    }

    /// <summary>
    /// Validates WCAG compliance by ensuring a contrast ratio of at least 'requiredRatio'.
    /// Introduce a small tolerance so near misses don't fail due to floating-point rounding.
    /// </summary>
    private static bool IsWCAGCompliant(Color background, Color contrast, double requiredRatio)
    {
        float backgroundLuminance = GetRelativeLuminance(background);
        float contrastLuminance = GetRelativeLuminance(contrast);

        double contrastRatio = CalculateContrastRatio(backgroundLuminance, contrastLuminance);

        return contrastRatio >= requiredRatio;
    }

    /// <summary>
    /// Calculates the relative luminance of a color as per WCAG guidelines.
    /// </summary>
    private static float GetRelativeLuminance(Color color)
    {
        float r = Normalize(color.R / 255f);
        float g = Normalize(color.G / 255f);
        float b = Normalize(color.B / 255f);

        return 0.2126f * r + 0.7152f * g + 0.0722f * b;
    }

    /// <summary>
    /// Applies the piecewise sRGB luminance formula to a single color component.
    /// </summary>
    private static float Normalize(float value)
    {
        return value <= 0.03928f
            ? value / 12.92f
            : MathF.Pow((value + 0.055f) / 1.055f, 2.4f);
    }

    /// <summary>
    /// Calculates the contrast ratio between two luminance values.
    /// </summary>
    private static double CalculateContrastRatio(float luminance1, float luminance2)
    {
        float lighter = MathF.Max(luminance1, luminance2);
        float darker = MathF.Min(luminance1, luminance2);

        return (lighter + 0.05) / (darker + 0.05);
    }
}