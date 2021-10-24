using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using ScribanTemplate = Scriban.Template;

namespace SimpleTextTemplate.Benchmarks;

[SimpleJob(RuntimeMoniker.Net60)]
[MemoryDiagnoser]
public class ParseBenchmark
{
    byte[]? _utf8Source;
    string? _utf16Source;

    [GlobalSetup]
    public void Setup()
    {
        _utf8Source = File.ReadAllBytes("Templates/Page.html");
        _utf16Source = Encoding.UTF8.GetString(_utf8Source);
    }

    [Benchmark(Baseline = true)]
    public Template SimpleTextTemplate() => Template.Parse(_utf8Source!);

    [Benchmark]
    public ScribanTemplate Scriban() => ScribanTemplate.Parse(_utf16Source);

    [Benchmark]
    public ScribanTemplate ScribanLiquid() => ScribanTemplate.ParseLiquid(_utf16Source);
}
