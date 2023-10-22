using System.Text;
using BenchmarkDotNet.Attributes;
using ScribanTemplate = Scriban.Template;

namespace SimpleTextTemplate.Benchmarks;

public class ParseBenchmark
{
    byte[] _utf8Source = null!;
    string _utf16Source = null!;

    string _format = null!;

    [GlobalSetup]
    public void Setup()
    {
        _utf8Source = Encoding.UTF8.GetBytes(ZTemplate.Source);
        _utf16Source = ZTemplate.Source;

        _format = "abcdef{0}01234567890";
    }

    [Benchmark(Baseline = true)]
    public Template SimpleTextTemplate() => Template.Parse(_utf8Source);

    [Benchmark]
    public ScribanTemplate Scriban() => ScribanTemplate.Parse(_utf16Source);

    [Benchmark]
    public ScribanTemplate ScribanLiquid() => ScribanTemplate.ParseLiquid(_utf16Source);

    [Benchmark(Description = "(CompositeFormat)")]
    public CompositeFormat CompositeFormatParse() => CompositeFormat.Parse(_format);
}
