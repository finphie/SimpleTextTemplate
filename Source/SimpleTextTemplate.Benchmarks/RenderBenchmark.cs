using System.Text;
using System.Text.RegularExpressions;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using CommunityToolkit.HighPerformance.Buffers;
using SimpleTextTemplate.Contexts;
using Utf8Utility;
using ScribanTemplate = Scriban.Template;

namespace SimpleTextTemplate.Benchmarks;

[SimpleJob(RuntimeMoniker.Net80)]
[MemoryDiagnoser]
public partial class RenderBenchmark
{
    const string Message = "Hello, World!";

    // lang=regex
    const string Pattern = "{{ *" + ZTemplate.Identifier + " *}}";

    string _utf16Source = null!;

    IContext _context = null!;
    SampleContext _contextObject;
    Dictionary<string, string> _model = null!;

    Template _template;
    ScribanTemplate _scribanTemplate = null!;
    ScribanTemplate _scribanLiquidTemplate = null!;
    Regex _regex = null!;

    [GlobalSetup]
    public void Setup()
    {
        var source = Encoding.UTF8.GetBytes(ZTemplate.Source);
        _utf16Source = Encoding.UTF8.GetString(source);

        var utf8Message = (Utf8Array)Message;

        _template = Template.Parse(source);
        var utf8Dict = new Utf8ArrayDictionary<Utf8Array>();
        utf8Dict.TryAdd((Utf8Array)ZTemplate.Identifier, utf8Message);
        _context = Context.Create(utf8Dict);

        _contextObject = new(utf8Message.DangerousAsByteArray());

        _scribanTemplate = ScribanTemplate.Parse(_utf16Source);
        _scribanLiquidTemplate = ScribanTemplate.ParseLiquid(_utf16Source);
        _model = new()
        {
            { ZTemplate.Identifier, Message }
        };

        _regex = RegexInternal();
    }

    [Benchmark]
    public byte[] SimpleTextTemplate()
    {
        using var bufferWriter = new ArrayPoolBufferWriter<byte>();
        _template.Render(bufferWriter, _context);
        return bufferWriter.WrittenSpan.ToArray();
    }

    [Benchmark(Baseline = true)]
    public byte[] SimpleTextTemplateSourceGenerator()
    {
        using var bufferWriter = new ArrayPoolBufferWriter<byte>();
        ZTemplate.Render(bufferWriter, _contextObject);
        return bufferWriter.WrittenSpan.ToArray();
    }

    [Benchmark]
    public string Scriban() => _scribanTemplate.Render(_model);

    [Benchmark]
    public string ScribanLiquid() => _scribanLiquidTemplate.Render(_model);

    [Benchmark]
    public string Regex() => _regex.Replace(_utf16Source, Message);

    [GeneratedRegex(Pattern, RegexOptions.Compiled | RegexOptions.ECMAScript | RegexOptions.CultureInvariant)]
    private static partial Regex RegexInternal();
}
