using System.Buffers;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using BenchmarkDotNet.Attributes;
using SimpleTextTemplate.Contexts;
using Utf8Utility;
using ScribanTemplate = Scriban.Template;

namespace SimpleTextTemplate.Benchmarks;

public partial class RenderBenchmark
{
    const string Format = "abcdef{0}01234567890";

    // lang=regex
    const string Pattern = "{{ *" + ZTemplate.Identifier + " *}}";

    readonly ArrayBufferWriter<byte> _bufferWriter = new();

    string _message = null!;
    string _utf16Source = null!;

    IContext _context = null!;
    SampleContext _contextObject;
    Dictionary<string, string> _model = null!;

    Template _template;
    ScribanTemplate _scribanTemplate = null!;
    ScribanTemplate _scribanLiquidTemplate = null!;
    Regex _regex = null!;
    CompositeFormat _compositeFormat = null!;

    [GlobalSetup]
    public void Setup()
    {
        _message = "Hello, World!";

        var source = Encoding.UTF8.GetBytes(ZTemplate.Source);
        _utf16Source = Encoding.UTF8.GetString(source);

        var utf8Message = (Utf8Array)_message;

        _template = Template.Parse(source);
        var utf8Dict = new Utf8ArrayDictionary<Utf8Array>();
        utf8Dict.TryAdd((Utf8Array)ZTemplate.Identifier, utf8Message);
        _context = Context.Create(utf8Dict);

        _contextObject = new(utf8Message.DangerousAsByteArray());

        _scribanTemplate = ScribanTemplate.Parse(_utf16Source);
        _scribanLiquidTemplate = ScribanTemplate.ParseLiquid(_utf16Source);
        _model = new()
        {
            { ZTemplate.Identifier, _message }
        };

        _regex = RegexInternal();
        _compositeFormat = CompositeFormat.Parse(Format);
    }

    [Benchmark]
    public byte[] SimpleTextTemplate()
    {
        _template.Render(_bufferWriter, _context);

        var result = _bufferWriter.WrittenSpan.ToArray();
        _bufferWriter.ResetWrittenCount();

        return result;
    }

    [Benchmark(Baseline = true)]
    public byte[] SimpleTextTemplate_SG()
    {
        ZTemplate.Render(_bufferWriter, _contextObject);

        var result = _bufferWriter.WrittenSpan.ToArray();
        _bufferWriter.ResetWrittenCount();

        return result;
    }

    [Benchmark]
    public string Scriban() => _scribanTemplate.Render(_model);

    [Benchmark]
    public string ScribanLiquid() => _scribanLiquidTemplate.Render(_model);

    [Benchmark(Description = "(Regex.Replace)")]
    public string Regex() => _regex.Replace(_utf16Source, _message);

    [Benchmark(Description = "(InterpolatedStringHandler)")]
    public string InterpolatedStringHandler()
    {
        DefaultInterpolatedStringHandler handler = $"abcdef{_message}01234567890";
        return handler.ToStringAndClear();
    }

    [Benchmark(Description = "(string.Format)")]
#pragma warning disable CA1863 // 'CompositeFormat' を使用してください
    public string StringFormat() => string.Format(CultureInfo.InvariantCulture, Format, _message);
#pragma warning restore CA1863 // 'CompositeFormat' を使用してください

    [Benchmark(Description = "(CompositeFormat)")]
    public string StringFormat_CF() => string.Format(CultureInfo.InvariantCulture, _compositeFormat, _message);

    [GeneratedRegex(Pattern, RegexOptions.Compiled | RegexOptions.ECMAScript | RegexOptions.CultureInvariant)]
    private static partial Regex RegexInternal();
}
