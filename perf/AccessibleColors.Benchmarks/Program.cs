using AccessibleColors.Benchmarks;
using BenchmarkDotNet.Running;

BenchmarkRunner.Run([
    typeof(WcagContrastColorBenchmarks),
    typeof(WcagContrastColorTextBenchmark),
    typeof(WcagContrastColorUIElementBenchmark),
    typeof(ColorRampBenchmarks)],
    new Config());