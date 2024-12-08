using AccessibleColors.Benchmarks;
using BenchmarkDotNet.Running;

BenchmarkRunner.Run([
    typeof(WcagContrastColorBenchmarks),
    typeof(WcagContrastColorTextBenchmark),
    typeof(ColorRampBenchmarks)],
    new Config());

//BenchmarkRunner.Run<WcagContrastColorBenchmarks>(new Config());
//BenchmarkRunner.Run<WcagContrastColorTextBenchmark>(new Config());
//BenchmarkRunner.Run<ColorRampBenchmarks>(new Config());