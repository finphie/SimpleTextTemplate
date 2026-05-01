using System.Buffers;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Text.Unicode;
using BenchmarkDotNet.Attributes;
using static SimpleTextTemplate.Benchmarks.Constants;
using ScribanTemplate = Scriban.Template;

namespace SimpleTextTemplate.Benchmarks;

public partial class RenderStringBenchmark
{
    const string Template = "{{ StringValue }}{{ StringValue }}{{ StringValue }}{{ StringValue }}{{ StringValue }}";

    readonly ArrayBufferWriter<byte> _bufferWriter = new(BufferSize);

    Template _stringTemplate;
    ScribanTemplate _scribanTemplate;
    ScribanTemplate _scribanLiquidTemplate;
    CompositeFormat _compositeFormat;

    SampleContext _generatorContext;
    Dictionary<byte[], object> _context;
    Dictionary<string, object> _scribanContext;

    [GeneratedRegex("{{ StringValue }}")]
    static partial Regex Regex { get; }

    [GlobalSetup]
    public void Setup()
    {
        _stringTemplate = global::SimpleTextTemplate.Template.Parse(Encoding.UTF8.GetBytes(Template));
        _scribanTemplate = ScribanTemplate.Parse(Template);
        _scribanLiquidTemplate = ScribanTemplate.ParseLiquid(Template);
        _compositeFormat = CompositeFormat.Parse(Format);

        _generatorContext = new();
        _context = Context.Create();
        _context.Add("StringValue"u8.ToArray(), _generatorContext.StringValue);
        _scribanContext = new()
        {
            { "StringValue", _generatorContext.StringValue }
        };
    }

    [Benchmark(Baseline = true, Description = DescriptionSimpleTextTemplateGenerator)]
    public ReadOnlyMemory<byte> SimpleTextTemplate_Generator()
    {
        _bufferWriter.ResetWrittenCount();

        var writer = TemplateWriter.Create(_bufferWriter);
        TemplateRenderer.Render(ref writer, Template, in _generatorContext);
        writer.Flush();

        return _bufferWriter.WrittenMemory;
    }

    [Benchmark(Description = DescriptionSimpleTextTemplate)]
    public ReadOnlyMemory<byte> SimpleTextTemplate()
    {
        _bufferWriter.ResetWrittenCount();

        _stringTemplate.Render(_bufferWriter, _context);
        return _bufferWriter.WrittenMemory;
    }

    [Benchmark(Description = DescriptionUtf8TryWrite)]
    public ReadOnlyMemory<byte> Utf8_TryWrite()
    {
        _bufferWriter.ResetWrittenCount();

        var length = Encoding.UTF8.GetMaxByteCount(_generatorContext.StringValue.Length * 5);
        Utf8.TryWrite(
            _bufferWriter.GetSpan(length),
            $"{_generatorContext.StringValue}{_generatorContext.StringValue}{_generatorContext.StringValue}{_generatorContext.StringValue}{_generatorContext.StringValue}",
            out var bytesWritten);
        _bufferWriter.Advance(bytesWritten);

        return _bufferWriter.WrittenMemory;
    }

    [Benchmark(Description = DescriptionInterpolatedStringHandler)]
    public string InterpolatedStringHandler()
    {
        DefaultInterpolatedStringHandler handler = $"{_generatorContext.StringValue}{_generatorContext.StringValue}{_generatorContext.StringValue}{_generatorContext.StringValue}{_generatorContext.StringValue}";
        return handler.ToStringAndClear();
    }

    [Benchmark(Description = DescriptionCompositeFormat)]
    public string System_Text_CompositeFormat()
        => string.Format(CultureInfo.InvariantCulture, _compositeFormat, _generatorContext.StringValue);

    [Benchmark(Description = DescriptionRegex)]
    public string System_Text_RegularExpressions_Regex()
        => Regex.Replace(Template, _generatorContext.StringValue);

    [Benchmark(Description = DescriptionScriban)]
    public string Scriban()
        => _scribanTemplate.Render(_scribanContext);

    [Benchmark(Description = DescriptionScribanLiquid)]
    public string Scriban_Liquid()
        => _scribanLiquidTemplate.Render(_scribanContext);
}
