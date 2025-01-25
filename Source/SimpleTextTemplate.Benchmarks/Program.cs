using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;

var config = DefaultConfig.Instance
    .AddJob(Job.Default.WithId("No PGO").WithRuntime(CoreRuntime.Core90).WithEnvironmentVariable("DOTNET_TieredPGO", "0"))
    .AddJob(Job.Default.WithId("PGO").WithRuntime(CoreRuntime.Core90))
    .AddDiagnoser(MemoryDiagnoser.Default)
    .HideColumns("StdDev", "Median", "RatioSD", "Alloc Ratio", "EnvironmentVariables");
BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, config);
