using BenchmarkDotNet.Attributes;
using System.Drawing;

namespace AccessibleColors.Benchmarks;

[MemoryDiagnoser]
public class ColorRampBenchmarks
{
    private readonly Color _darkBaseColor = Color.FromArgb(0, 120, 215);
    private readonly Color _lightBaseColor = Color.FromArgb(50, 50, 50);

    [Params(5, 10, 20)]
    public int Steps;

    [Benchmark]
    public IReadOnlyList<Color> GenerateDarkModeRamp()
    {
        return ColorRampGenerator.GenerateAccessibleRamp(_darkBaseColor, Steps, darkMode: true);
    }

    [Benchmark]
    public IReadOnlyList<Color> GenerateLightModeRamp()
    {
        return ColorRampGenerator.GenerateAccessibleRamp(_lightBaseColor, Steps, darkMode: false);
    }
}
