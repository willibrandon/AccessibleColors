using BenchmarkDotNet.Attributes;
using System.Drawing;

namespace AccessibleColors.Benchmarks;

[MemoryDiagnoser]
public class WcagContrastColorBenchmarks
{
    private Color _cachedColor;
    private Color _blackWhiteColor;
    private Color _dynamicColor;

    [GlobalSetup]
    public void Setup()
    {
        // Precompute a cached color
        _cachedColor = Color.FromArgb(120, 120, 120); // Mid-gray
        _blackWhiteColor = Color.FromArgb(20, 20, 20); // Dark gray
        _dynamicColor = Color.FromArgb(100, 120, 140); // Muted blue

        // Warm up the cache
        _cachedColor.GetContrastColor();
    }

    [Benchmark]
    public Color GetContrastColor_CachedColor_O1WithCachingUnsafe()
    {
        return _cachedColor.GetContrastColor();
    }

    [Benchmark]
    public Color GetContrastColor_BlackWhiteContrast_O1WithCachingUnsafe()
    {
        return _blackWhiteColor.GetContrastColor();
    }

    [Benchmark]
    public Color GetContrastColor_DynamicContrast_O1WithCachingUnsafe()
    {
        return _dynamicColor.GetContrastColor();
    }
}
