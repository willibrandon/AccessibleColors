using System.Drawing;

namespace AccessibleColors.Tests;

public class ColorRampGeneratorTests
{
    [Fact]
    public void GenerateAccessibleRamp_ReturnsEmptyForZeroSteps()
    {
        Color baseColor = Color.FromArgb(100, 100, 100);
        var ramp = ColorRampGenerator.GenerateAccessibleRamp(baseColor, 0, darkMode: true);

        Assert.Empty(ramp);
    }

    [Fact]
    public void GenerateAccessibleRamp_CorrectNumberOfSteps()
    {
        Color baseColor = Color.Blue;
        int steps = 5;
        var ramp = ColorRampGenerator.GenerateAccessibleRamp(baseColor, steps, darkMode: false);

        Assert.NotNull(ramp);
        Assert.Equal(steps, ramp.Count);
    }

    [Fact]
    public void GenerateAccessibleRamp_DarkModeCompliance()
    {
        // Dark mode background:
        Color bg = Color.FromArgb(32, 32, 32);
        Color baseColor = Color.FromArgb(0, 120, 215); // A brand-like accent color
        int steps = 5;
        var ramp = ColorRampGenerator.GenerateAccessibleRamp(baseColor, steps, darkMode: true);

        Assert.Equal(steps, ramp.Count);

        foreach (var c in ramp)
        {
            Assert.True(IsWCAGCompliant(bg, c), $"Color {c} must be WCAG compliant on dark background.");
        }
    }

    [Fact]
    public void GenerateAccessibleRamp_LightModeCompliance()
    {
        // Light mode background = White
        Color bg = Color.White;
        Color baseColor = Color.FromArgb(50, 50, 50); // A darker base color
        int steps = 5;
        var ramp = ColorRampGenerator.GenerateAccessibleRamp(baseColor, steps, darkMode: false);

        Assert.Equal(steps, ramp.Count);

        foreach (var c in ramp)
        {
            Assert.True(IsWCAGCompliant(bg, c), $"Color {c} must be WCAG compliant on light background.");
        }
    }

    [Fact]
    public void GenerateAccessibleRamp_HandlesLargeNumberOfSteps()
    {
        // Even if it won't be 5 ns or O(1), just test correctness for a larger step count:
        Color bg = Color.FromArgb(32, 32, 32);
        Color baseColor = Color.FromArgb(128, 64, 64);
        int steps = 10; // More steps
        var ramp = ColorRampGenerator.GenerateAccessibleRamp(baseColor, steps, darkMode: true);

        Assert.Equal(steps, ramp.Count);

        foreach (var c in ramp)
        {
            Assert.True(IsWCAGCompliant(bg, c), "All generated colors must be accessible.");
        }
    }

    /// <summary>
    /// Helper method to check WCAG compliance for test verification.
    /// </summary>
    private static bool IsWCAGCompliant(Color background, Color foreground)
    {
        return WcagContrastColor.IsCompliant(background, foreground, 4.5);
    }
}
