using AccessibleColors.Benchmarks;
using BenchmarkDotNet.Running;

BenchmarkRunner.Run<WcagContrastColorBenchmarks>(new Config());
BenchmarkRunner.Run<ColorRampBenchmarks>(new Config());