using System.Text;
using System.Text.RegularExpressions;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Microsoft.Toolkit.HighPerformance.Buffers;
using SimpleTextTemplate.Contexts;
using Utf8Utility;
using ScribanTemplate = Scriban.Template;

namespace SimpleTextTemplate.Benchmarks;

[SimpleJob(RuntimeMoniker.Net60)]
[MemoryDiagnoser]
public class RenderBenchmark
{
    const string Identifier = "Identifier";
    const string Message = "Hello, World!";
    const string Pattern = "{{ *" + Identifier + " *}}";

    readonly ArrayPoolBufferWriter<byte> _bufferWriter = new();

    string? _utf16Source;

    IContext? _context;
    Dictionary<string, string>? _model;

    Template _template;
    ScribanTemplate? _scribanTemplate;
    ScribanTemplate? _scribanLiquidTemplate;
    Regex? _regex;

    [GlobalSetup]
    public void Setup()
    {
        var source = File.ReadAllBytes("Templates/Page.html");
        _utf16Source = Encoding.UTF8.GetString(source);

        _template = Template.Parse(source);
        var utf8Dict = new Utf8ArrayDictionary<Utf8Array>();
        utf8Dict.TryAdd((Utf8Array)Identifier, (Utf8Array)Message);
        _context = Context.Create(utf8Dict);

        _scribanTemplate = ScribanTemplate.Parse(_utf16Source);
        _scribanLiquidTemplate = ScribanTemplate.ParseLiquid(_utf16Source);
        _model = new()
        {
            { Identifier, Message }
        };

        _regex = new Regex(Pattern, RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.ECMAScript);
    }

    [GlobalCleanup]
    public void Cleanup() => _bufferWriter.Dispose();

    [Benchmark]
    public ReadOnlySpan<byte> SimpleTextTemplate()
    {
        _bufferWriter.Clear();
        _template.RenderTo(_bufferWriter, _context!);
        return _bufferWriter.WrittenSpan;
    }

    [Benchmark(Baseline = true)]
    public ReadOnlySpan<byte> ZSimpleTextTemplate()
    {
        _bufferWriter.Clear();
        ZTemplate.GeneratePageTemplate(_bufferWriter, _context);
        return _bufferWriter.WrittenSpan;
    }

    [Benchmark]
    public string Scriban() => _scribanTemplate!.Render(_model);

    [Benchmark]
    public string ScribanLiquid() => _scribanLiquidTemplate!.Render(_model);

    [Benchmark]
    public string Regex() => _regex!.Replace(_utf16Source!, Message);
}
