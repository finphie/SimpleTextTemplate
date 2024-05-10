using System.Buffers;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Unicode;
using BenchmarkDotNet.Attributes;
using SimpleTextTemplate.Contexts;
using Utf8Utility;
using ScribanTemplate = Scriban.Template;

namespace SimpleTextTemplate.Benchmarks;

public partial class RenderBenchmark
{
    const string Identifier = nameof(Identifier);
    const string Source = "abcdef{{ " + Identifier + " }}01234567890";
    const string Format = "abcdef{0}01234567890";

    readonly ArrayBufferWriter<byte> _bufferWriter = new();

    string _message;
    string _utf16Source;

    IContext _context;
    SampleContext _contextObject;
    Dictionary<string, string> _model;

    Template _template;
    ScribanTemplate _scribanTemplate;
    ScribanTemplate _scribanLiquidTemplate;
    CompositeFormat _compositeFormat;

    [GlobalSetup]
    public void Setup()
    {
        _message = "Hello, World!";

        var source = Encoding.UTF8.GetBytes(Source);
        _utf16Source = Encoding.UTF8.GetString(source);

        var utf8Message = Encoding.UTF8.GetBytes(_message);

        _template = Template.Parse(source);
        var utf8Dict = new Utf8ArrayDictionary<object>();
        utf8Dict.TryAdd((Utf8Array)Identifier, utf8Message);
        _context = Context.Create(utf8Dict);

        _contextObject = new(utf8Message);

        _scribanTemplate = ScribanTemplate.Parse(_utf16Source);
        _scribanLiquidTemplate = ScribanTemplate.ParseLiquid(_utf16Source);
        _model = new()
        {
            { Identifier, _message }
        };

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
        var writer = TemplateWriter.Create(_bufferWriter);
        writer.Write(Source, in _contextObject);
        writer.Dispose();

        var result = _bufferWriter.WrittenSpan.ToArray();
        _bufferWriter.ResetWrittenCount();

        return result;
    }

    [Benchmark]
    public string Scriban() => _scribanTemplate.Render(_model);

    [Benchmark]
    public string ScribanLiquid() => _scribanLiquidTemplate.Render(_model);

    [Benchmark(Description = "(Utf8.TryWrite)")]
    public byte[] Utf8TryWrite()
    {
        Utf8.TryWrite(_bufferWriter.GetSpan(), $"abcdef{_message}01234567890", out var bytesWritten);
        _bufferWriter.Advance(bytesWritten);

        var result = _bufferWriter.WrittenSpan.ToArray();
        _bufferWriter.ResetWrittenCount();

        return result;
    }

    [Benchmark(Description = "(InterpolatedStringHandler)")]
    public string InterpolatedStringHandler()
    {
        DefaultInterpolatedStringHandler handler = $"abcdef{_message}01234567890";
        return handler.ToStringAndClear();
    }

    [Benchmark(Description = "(string.Format)")]
    public string StringFormat() => string.Format(CultureInfo.InvariantCulture, Format, _message);

    [Benchmark(Description = "(CompositeFormat)")]
    public string StringFormat_CF() => string.Format(CultureInfo.InvariantCulture, _compositeFormat, _message);

    internal readonly record struct SampleContext(byte[] Identifier);
}
