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

public partial class RenderConstantStringBenchmark
{
    const string TemplateText = "{{ ConstantStringValue }}{{ ConstantStringValue }}{{ ConstantStringValue }}{{ ConstantStringValue }}{{ ConstantStringValue }}";

    readonly ArrayBufferWriter<byte> _bufferWriter = new(BufferSize);

    Template _template;
    ScribanTemplate _scribanTemplate;
    ScribanTemplate _scribanLiquidTemplate;
    CompositeFormat _compositeFormat;

    SampleContext _generatorContext;
    Dictionary<byte[], object> _context;
    Dictionary<string, object> _scribanContext;

    [GeneratedRegex("{{ ConstantStringValue }}")]
    static partial Regex Regex { get; }

    [GlobalSetup]
    public void Setup()
    {
        _template = Template.Parse(Encoding.UTF8.GetBytes(TemplateText));
        _scribanTemplate = ScribanTemplate.Parse(TemplateText);
        _scribanLiquidTemplate = ScribanTemplate.ParseLiquid(TemplateText);
        _compositeFormat = CompositeFormat.Parse(Format);

        _generatorContext = new();
        _context = Context.Create();
        _context.Add("ConstantStringValue"u8.ToArray(), SampleContext.ConstantStringValue);
        _scribanContext = new()
        {
            { "ConstantStringValue", SampleContext.ConstantStringValue }
        };
    }

    [Benchmark(Baseline = true, Description = DescriptionSimpleTextTemplateGenerator)]
    public ReadOnlyMemory<byte> SimpleTextTemplate_Generator()
    {
        _bufferWriter.ResetWrittenCount();

        var writer = TemplateWriter.Create(_bufferWriter);
        TemplateRenderer.Render(ref writer, TemplateText, in _generatorContext);
        writer.Flush();

        return _bufferWriter.WrittenMemory;
    }

    [Benchmark(Description = DescriptionSimpleTextTemplate)]
    public ReadOnlyMemory<byte> SimpleTextTemplate()
    {
        _bufferWriter.ResetWrittenCount();

        _template.Render(_bufferWriter, _context);
        return _bufferWriter.WrittenMemory;
    }

    [Benchmark(Description = DescriptionUtf8TryWrite)]
    public ReadOnlyMemory<byte> Utf8_TryWrite()
    {
        _bufferWriter.ResetWrittenCount();

        var length = Encoding.UTF8.GetMaxByteCount(SampleContext.ConstantStringValue.Length * 5);
        Utf8.TryWrite(
            _bufferWriter.GetSpan(length),
            CultureInfo.InvariantCulture,
            $"{SampleContext.ConstantStringValue}{SampleContext.ConstantStringValue}{SampleContext.ConstantStringValue}{SampleContext.ConstantStringValue}{SampleContext.ConstantStringValue}",
            out var bytesWritten);
        _bufferWriter.Advance(bytesWritten);

        return _bufferWriter.WrittenMemory;
    }

    [Benchmark(Description = DescriptionInterpolatedStringHandler)]
    public string InterpolatedStringHandler()
    {
        DefaultInterpolatedStringHandler handler = $"{SampleContext.ConstantStringValue}{SampleContext.ConstantStringValue}{SampleContext.ConstantStringValue}{SampleContext.ConstantStringValue}{SampleContext.ConstantStringValue}";
        return handler.ToStringAndClear();
    }

    [Benchmark(Description = DescriptionCompositeFormat)]
    public string System_Text_CompositeFormat()
        => string.Format(CultureInfo.InvariantCulture, _compositeFormat, SampleContext.ConstantStringValue);

    [Benchmark(Description = DescriptionRegex)]
    public string System_Text_RegularExpressions_Regex()
        => Regex.Replace(TemplateText, SampleContext.ConstantStringValue);

    [Benchmark(Description = DescriptionScriban)]
    public string Scriban()
        => _scribanTemplate.Render(_scribanContext);

    [Benchmark(Description = DescriptionScribanLiquid)]
    public string Scriban_Liquid()
        => _scribanLiquidTemplate.Render(_scribanContext);
}
