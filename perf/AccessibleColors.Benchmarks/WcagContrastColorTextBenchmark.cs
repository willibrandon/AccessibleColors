using BenchmarkDotNet.Attributes;
using System.Drawing;

namespace AccessibleColors.Benchmarks;

/// <summary>
/// Benchmarks the performance of GetContrastColorForText with varying text sizes and weights.
/// </summary>
[MemoryDiagnoser]
public class WcagContrastColorTextBenchmark
{
    private Color _lightBackground;
    private Color _darkBackground;
    private Color _midBackground;

    [GlobalSetup]
    public void Setup()
    {
        // Representative backgrounds:
        _lightBackground = Color.FromArgb(255, 255, 255); // White
        _darkBackground = Color.FromArgb(32, 32, 32);     // Dark gray
        _midBackground = Color.FromArgb(120, 120, 120);   // Mid-gray

        // Warm up by calling once:
        _lightBackground.GetContrastColorForText(12.0, false);
        _darkBackground.GetContrastColorForText(18.0, true);
        _midBackground.GetContrastColorForText(14.0, true);
    }

    [Benchmark]
    public Color GetContrastColorForText_NormalText_LightBg()
    {
        // Normal text on a light background
        return _lightBackground.GetContrastColorForText(12.0, false);
    }

    [Benchmark]
    public Color GetContrastColorForText_LargeBoldText_DarkBg()
    {
        // Large bold text on a dark background, should require only 3:1
        return _darkBackground.GetContrastColorForText(18.0, true);
    }

    [Benchmark]
    public Color GetContrastColorForText_Bold14ptText_MidBg()
    {
        // 14pt bold text on mid-gray background is considered large text at 3:1
        return _midBackground.GetContrastColorForText(14.0, true);
    }
}
