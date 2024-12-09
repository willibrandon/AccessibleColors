using System.Drawing;
using Xunit.Abstractions;

namespace AccessibleColors.Tests;

public class ColorRampGeneratorTests(ITestOutputHelper output)
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
        Color baseColor = Color.FromArgb(0, 120, 215);
        int steps = 5;
        var ramp = ColorRampGenerator.GenerateAccessibleRamp(baseColor, steps, darkMode: true);

        Assert.Equal(steps, ramp.Count);

        foreach (var c in ramp)
        {
            Assert.True(IsWCAGCompliant(bg, c), $"Color {c} must be WCAG compliant on dark background.");
        }
    }

    /// <summary>
    /// Performs an exhaustive test of the <see cref="ColorRampGenerator.GenerateAccessibleRamp"/> method
    /// against all possible 24-bit RGB colors, multiple step counts, and both dark and light modes.
    /// 
    /// This test attempts to verify that every generated color meets the WCAG 2.2 contrast ratio of at
    /// least 4.5:1, ensuring no fallback colors are needed. It loops through all 16,777,216 colors,
    /// for each of 5 different step counts, and both dark and light mode scenarios, resulting in a
    /// total of 167,772,160 permutations.
    /// 
    /// Running this test is extremely time-consuming and not practical under normal circumstances,
    /// which is why it is skipped by default. It is left here as a reference to the comprehensive level
    /// of testing that can be performed if needed.
    /// 
    /// If run to completion on a sufficiently powerful machine, this test will:
    /// - Generate each ramp,
    /// - Verify compliance for every color in the ramp,
    /// - Count how often (if ever) a fallback color (black/white) is used.
    /// 
    /// A successful completion with zero fallback occurrences indicates that the ramp generator
    /// can produce fully compliant colors for all possible inputs.
    /// </summary>
    [Fact(Skip = "Extremely long-running test.")]
    public void GenerateAccessibleRamp_ExhaustiveTest()
    {
        int[] stepCounts = { 1, 2, 3, 4, 5 };
        bool[] darkModes = { false, true };

        Color darkBg = Color.FromArgb(32, 32, 32);
        Color lightBg = Color.White;

        // Calculate total permutations for informational purposes
        long totalPermutations = 256L * 256L * 256L * stepCounts.Length * darkModes.Length;
        long fallbackCount = 0;
        long count = 0;

        for (int r = 0; r <= 255; r++)
        {
            for (int g = 0; g <= 255; g++)
            {
                for (int b = 0; b <= 255; b++)
                {
                    Color baseColor = Color.FromArgb(r, g, b);

                    foreach (int steps in stepCounts)
                    {
                        foreach (bool darkMode in darkModes)
                        {
                            count++;
                            IReadOnlyList<Color> ramp = ColorRampGenerator.GenerateAccessibleRamp(baseColor, steps, darkMode);

                            Color bg = darkMode ? darkBg : lightBg;

                            foreach (var c in ramp)
                            {
                                bool compliant = WcagContrastColor.IsCompliant(bg, c);
                                Assert.True(compliant, $"Non-compliant color: {c} for base {baseColor}, steps={steps}, darkMode={darkMode}");

                                // Check fallback color
                                if ((darkMode && c == Color.White) || (!darkMode && c == Color.Black))
                                {
                                    fallbackCount++;
                                }
                            }

                            // Optional: print progress occasionally (commented out by default to reduce overhead)
                            if (count % 1_000_000 == 0)
                            {
                                output.WriteLine($"Tested {count}/{totalPermutations} permutations...");
                            }
                        }
                    }
                }
            }
        }

        output.WriteLine($"Total permutations tested: {totalPermutations}");
        output.WriteLine($"Fallback occurred {fallbackCount} times.");
        double fallbackPercentage = (fallbackCount / (double)totalPermutations) * 100.0;
        output.WriteLine($"Fallback Percentage: {fallbackPercentage:F2}%");
    }

    [Fact]
    public void GenerateAccessibleRamp_FallbackFrequencySampleTest()
    {
        // Configuration
        int colorSamples = 100_000;   // Number of random colors to sample
        int[] stepCounts = { 1, 5, 10 };
        bool[] darkModes = { false, true };

        // Backgrounds
        Color darkBg = Color.FromArgb(32, 32, 32);
        Color lightBg = Color.White;

        var rnd = new Random(0); // Fixed seed for reproducibility
        int fallbackCount = 0;
        int totalTests = 0;

        for (int i = 0; i < colorSamples; i++)
        {
            // Random base color
            int r = rnd.Next(256);
            int g = rnd.Next(256);
            int b = rnd.Next(256);
            Color baseColor = Color.FromArgb(r, g, b);

            foreach (int steps in stepCounts)
            {
                foreach (bool darkMode in darkModes)
                {
                    IReadOnlyList<Color> ramp = ColorRampGenerator.GenerateAccessibleRamp(baseColor, steps, darkMode);
                    Color bg = darkMode ? darkBg : lightBg;

                    foreach (Color c in ramp)
                    {
                        Assert.True(WcagContrastColor.IsCompliant(bg, c),
                            $"Non-compliant color {c} returned for base {baseColor}, steps={steps}, darkMode={darkMode}");

                        // Check if fallback occurred
                        if ((darkMode && c == Color.White) || (!darkMode && c == Color.Black))
                        {
                            fallbackCount++;
                        }
                    }

                    totalTests++;
                }
            }
        }

        double fallbackPercentage = (fallbackCount / (double)totalTests) * 100.0;
        output.WriteLine($"Total Tests: {totalTests}");
        output.WriteLine($"Fallback Occurrences: {fallbackCount}");
        output.WriteLine($"Fallback Percentage: {fallbackPercentage:F2}%");
    }

    [Fact]
    public void GenerateAccessibleRamp_LightModeCompliance()
    {
        // Light mode background = White
        Color bg = Color.White;
        Color baseColor = Color.FromArgb(50, 50, 50);
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
        Color bg = Color.FromArgb(32, 32, 32);
        Color baseColor = Color.FromArgb(128, 64, 64);
        int steps = 10;
        var ramp = ColorRampGenerator.GenerateAccessibleRamp(baseColor, steps, darkMode: true);

        Assert.Equal(steps, ramp.Count);

        foreach (var c in ramp)
        {
            Assert.True(IsWCAGCompliant(bg, c), "All generated colors must be accessible.");
        }
    }

    [Fact]
    public void GenerateAccessibleRamp_SingleStep_DarkMode()
    {
        // Use a bright yellow color that should easily exceed 4.5:1 contrast on a dark background.
        Color baseColor = Color.FromArgb(255, 255, 0); // Bright yellow
        int steps = 1;
        bool darkMode = true;
        var ramp = ColorRampGenerator.GenerateAccessibleRamp(baseColor, steps, darkMode);

        Assert.Single(ramp);

        Color bg = darkMode ? Color.FromArgb(32, 32, 32) : Color.White;
        Assert.True(IsWCAGCompliant(bg, ramp[0]), "Single-step ramp should still produce a compliant color.");
    }

    [Fact]
    public void GenerateAccessibleRamp_SingleStep_LightMode()
    {
        // Black (#000000) on white background is obviously compliant.
        Color baseColor = Color.Black;
        int steps = 1;
        bool darkMode = false;
        var ramp = ColorRampGenerator.GenerateAccessibleRamp(baseColor, steps, darkMode);

        Assert.Single(ramp);

        Color bg = darkMode ? Color.FromArgb(32, 32, 32) : Color.White;
        Assert.True(IsWCAGCompliant(bg, ramp[0]), "Single-step ramp on light background should be compliant.");
    }

    [Fact]
    public void GenerateAccessibleRamp_NonCompliantStart_LightMode()
    {
        // Light gray on white (non-compliant start)
        Color baseColor = Color.FromArgb(200, 200, 200);
        int steps = 5;
        bool darkMode = false;
        var ramp = ColorRampGenerator.GenerateAccessibleRamp(baseColor, steps, darkMode);

        Assert.Equal(steps, ramp.Count);
        Color bg = Color.White;

        foreach (var c in ramp)
        {
            Assert.True(IsWCAGCompliant(bg, c), $"Color {c} must be compliant on white.");
        }

        // Check that at least one color differs noticeably from the original to achieve compliance.
        Assert.True(ramp.Any(c => GetColorDifference(c, baseColor) > 10),
            "Expected at least one color in the ramp to differ noticeably from the original to achieve compliance.");
    }

    [Fact]
    public void GenerateAccessibleRamp_NonCompliantStart_DarkMode()
    {
        // Slightly lighter than black, but likely non-compliant on dark background
        Color baseColor = Color.FromArgb(45, 45, 45);
        int steps = 5;
        bool darkMode = true;
        var ramp = ColorRampGenerator.GenerateAccessibleRamp(baseColor, steps, darkMode);

        Assert.Equal(steps, ramp.Count);
        Color bg = Color.FromArgb(32, 32, 32);

        foreach (var c in ramp)
        {
            Assert.True(IsWCAGCompliant(bg, c), $"Color {c} must be compliant on dark background.");
        }

        // Expect some noticeable difference from the original
        Assert.True(ramp.Any(c => GetColorDifference(c, baseColor) > 10),
            "Expected at least one step to differ noticeably from the original to reach compliance.");
    }

    [Fact]
    public void GenerateAccessibleRamp_AlreadyCompliantNoGradient()
    {
        // Already compliant color on white
        Color baseColor = Color.FromArgb(30, 30, 30);
        int steps = 5;
        bool darkMode = false;
        var ramp = ColorRampGenerator.GenerateAccessibleRamp(baseColor, steps, darkMode);

        Assert.Equal(steps, ramp.Count);
        Color bg = Color.White;

        foreach (var c in ramp)
        {
            Assert.True(IsWCAGCompliant(bg, c));
        }

        // Minimal variation is acceptable. Just ensure compliance is stable.
        // No assertion on gradient needed since it's already compliant.
    }

    [Fact]
    public void GenerateAccessibleRamp_ConsistencyTest()
    {
        // Deterministic test
        Color baseColor = Color.FromArgb(100, 100, 100);
        int steps = 5;
        bool darkMode = true;

        var ramp1 = ColorRampGenerator.GenerateAccessibleRamp(baseColor, steps, darkMode);
        var ramp2 = ColorRampGenerator.GenerateAccessibleRamp(baseColor, steps, darkMode);

        Assert.Equal(ramp1, ramp2);
    }

    [Fact]
    public void GenerateAccessibleRamp_ExtremeDarkOnDark()
    {
        // Starting from pure black in dark mode.
        Color baseColor = Color.Black;
        int steps = 5;
        bool darkMode = true;
        var ramp = ColorRampGenerator.GenerateAccessibleRamp(baseColor, steps, darkMode);

        Assert.Equal(steps, ramp.Count);
        Color bg = Color.FromArgb(32, 32, 32);

        foreach (var c in ramp)
        {
            Assert.True(IsWCAGCompliant(bg, c), $"Color {c} must be compliant on dark background.");
        }

        // Just ensure we made a change to achieve compliance, if needed.
        Assert.True(ramp.Any(c => GetColorDifference(c, baseColor) > 10),
            "Expected at least one step to differ from black to achieve compliance on dark background.");
    }

    /// <summary>
    /// Checks WCAG compliance for a given foreground against a background with a required ratio of 4.5.
    /// </summary>
    private static bool IsWCAGCompliant(Color background, Color foreground)
    {
        return WcagContrastColor.IsCompliant(background, foreground, 4.5);
    }

    /// <summary>
    /// A simple method to measure the approximate difference between two colors.
    /// This is not a precise color difference metric, but sufficient for test assertions.
    /// </summary>
    private static double GetColorDifference(Color c1, Color c2)
    {
        int dr = c1.R - c2.R;
        int dg = c1.G - c2.G;
        int db = c1.B - c2.B;
        return Math.Sqrt(dr * dr + dg * dg + db * db);
    }
}