using BenchmarkDotNet.Attributes;
using System.Drawing;

namespace AccessibleColors.Benchmarks;

/// <summary>
/// Benchmarks the performance of GetContrastForUIElement with various backgrounds
/// representing common UI element scenarios.
/// 
/// This test assumes that the UI element compliance defaults to a ratio of 3:1
/// (or another ratio as defined by the UI requirements) rather than the stricter 4.5:1 for text.
/// It's intended to show how quickly we can get a suitable color for UI elements like icons,
/// graphical components, and other non-text controls.
/// </summary>
[MemoryDiagnoser]
public class WcagContrastColorUIElementBenchmark
{
    private Color _lightBackground;
    private Color _darkBackground;
    private Color _midBackground;

    [GlobalSetup]
    public void Setup()
    {
        // Representative backgrounds:
        _lightBackground = Color.FromArgb(255, 255, 255); // White (typical light UI)
        _darkBackground = Color.FromArgb(32, 32, 32);     // Dark gray (typical dark UI)
        _midBackground = Color.FromArgb(120, 120, 120);   // Mid-gray

        // Warm up by calling once:
        WcagContrastColor.GetContrastColorForUIElement(_lightBackground);
        WcagContrastColor.GetContrastColorForUIElement(_darkBackground);
        WcagContrastColor.GetContrastColorForUIElement(_midBackground);
    }

    [Benchmark]
    public Color GetContrastForUIElement_LightBg()
    {
        // UI element over a light background
        return WcagContrastColor.GetContrastColorForUIElement(_lightBackground);
    }

    [Benchmark]
    public Color GetContrastForUIElement_DarkBg()
    {
        // UI element over a dark background
        return WcagContrastColor.GetContrastColorForUIElement(_darkBackground);
    }

    [Benchmark]
    public Color GetContrastForUIElement_MidBg()
    {
        // UI element over a mid-gray background
        return WcagContrastColor.GetContrastColorForUIElement(_midBackground);
    }
}