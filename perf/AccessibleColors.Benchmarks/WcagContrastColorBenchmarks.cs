using BenchmarkDotNet.Attributes;
using System.Drawing;

namespace AccessibleColors.Benchmarks;

/// <summary>
/// Benchmarks for the GetContrastColor method without text size/weight considerations.
/// </summary>
[MemoryDiagnoser]
public class WcagContrastColorBenchmarks
{
    private Color _midGrayBackground;
    private Color _darkGrayBackground;
    private Color _mutedBlueBackground;

    [GlobalSetup]
    public void Setup()
    {
        // Representative backgrounds:
        _midGrayBackground = Color.FromArgb(120, 120, 120); // Mid-gray
        _darkGrayBackground = Color.FromArgb(20, 20, 20);   // Dark gray
        _mutedBlueBackground = Color.FromArgb(100, 120, 140); // Muted blue

        // Warm up:
        _midGrayBackground.GetContrastColor();
    }

    /// <summary>
    /// Normal text scenario against a mid-gray background.
    /// </summary>
    [Benchmark]
    public Color GetContrastColor_NormalText_MidGrayBg()
    {
        return _midGrayBackground.GetContrastColor();
    }

    /// <summary>
    /// Normal text scenario against a dark gray background.
    /// </summary>
    [Benchmark]
    public Color GetContrastColor_NormalText_DarkGrayBg()
    {
        return _darkGrayBackground.GetContrastColor();
    }

    /// <summary>
    /// Normal text scenario against a muted blue background.
    /// </summary>
    [Benchmark]
    public Color GetContrastColor_NormalText_MutedBlueBg()
    {
        return _mutedBlueBackground.GetContrastColor();
    }
}
