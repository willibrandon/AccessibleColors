using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.CsProj;

namespace AccessibleColors.Benchmarks;

public class Config : ManualConfig
{
    public Config()
    {
        var baseJob = Job.Default
            .WithWarmupCount(3)
            .WithToolchain(CsProjCoreToolchain.NetCoreApp90)
            .WithPlatform(Platform.X64)
            .WithJit(Jit.RyuJit);

        AddDiagnoser(MemoryDiagnoser.Default);
        AddColumnProvider(DefaultConfig.Instance.GetColumnProviders().ToArray());
        AddExporter(DefaultConfig.Instance.GetExporters().ToArray());
        AddLogger(DefaultConfig.Instance.GetLoggers().ToArray());
        AddJob(baseJob.WithCustomBuildConfiguration("Release").WithId("Project"));
    }
}
