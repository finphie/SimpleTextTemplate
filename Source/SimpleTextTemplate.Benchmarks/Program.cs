using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;

#if DEBUG
var x = new SimpleTextTemplate.Benchmarks.RenderConstantStringBenchmark();
x.Setup();

Console.WriteLine("1: " + System.Text.Encoding.UTF8.GetString(x.SimpleTextTemplate_Generator().Span));
Console.WriteLine("2: " + System.Text.Encoding.UTF8.GetString(x.SimpleTextTemplate().Span));
Console.WriteLine("3: " + System.Text.Encoding.UTF8.GetString(x.Utf8_TryWrite().Span));
Console.WriteLine("4: " + x.InterpolatedStringHandler());
Console.WriteLine("5: " + x.System_Text_CompositeFormat());
Console.WriteLine("6: " + x.System_Text_RegularExpressions_Regex());
Console.WriteLine("7: " + x.Scriban());
Console.WriteLine("8: " + x.Scriban_Liquid());
#endif

var config = DefaultConfig.Instance
    .AddJob(Job.Default.WithId("No PGO").WithRuntime(CoreRuntime.Core10_0).WithEnvironmentVariable("DOTNET_TieredPGO", "0"))
    .AddJob(Job.Default.WithId("PGO").WithRuntime(CoreRuntime.Core10_0))
    .AddDiagnoser(MemoryDiagnoser.Default)
    .HideColumns("StdDev", "Median", "RatioSD", "Alloc Ratio", "EnvironmentVariables");
BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, config);
