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

public partial class RenderIntBenchmark
{
    const string IntTemplate = "{{ IntValue }}{{ IntValue }}{{ IntValue }}{{ IntValue }}{{ IntValue }}";

    readonly ArrayBufferWriter<byte> _bufferWriter = new(BufferSize);

    Template _template;
    ScribanTemplate _scribanTemplate;
    ScribanTemplate _scribanLiquidTemplate;
    CompositeFormat _compositeFormat;

    SampleContext _generatorContext;
    Dictionary<byte[], object> _context;
    Dictionary<string, object> _scribanContext;

    [GeneratedRegex("{{ IntValue }}")]
    static partial Regex Regex { get; }

    [GlobalSetup]
    public void Setup()
    {
        _template = Template.Parse(Encoding.UTF8.GetBytes(IntTemplate));
        _scribanTemplate = ScribanTemplate.Parse(IntTemplate);
        _scribanLiquidTemplate = ScribanTemplate.ParseLiquid(IntTemplate);
        _compositeFormat = CompositeFormat.Parse(Format);

        _generatorContext = new();
        _context = Context.Create();
        _context.Add("IntValue"u8.ToArray(), _generatorContext.IntValue);
        _scribanContext = new()
        {
            { "IntValue", _generatorContext.IntValue }
        };
    }

    [Benchmark(Baseline = true, Description = DescriptionSimpleTextTemplateGenerator)]
    public ReadOnlyMemory<byte> SimpleTextTemplate_Generator()
    {
        _bufferWriter.ResetWrittenCount();

        var writer = TemplateWriter.Create(_bufferWriter);
        TemplateRenderer.Render(ref writer, IntTemplate, in _generatorContext);
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

        Utf8.TryWrite(
            _bufferWriter.GetSpan(),
            $"{_generatorContext.IntValue}{_generatorContext.IntValue}{_generatorContext.IntValue}{_generatorContext.IntValue}{_generatorContext.IntValue}",
            out var bytesWritten);
        _bufferWriter.Advance(bytesWritten);

        return _bufferWriter.WrittenMemory;
    }

    [Benchmark(Description = DescriptionInterpolatedStringHandler)]
    public string InterpolatedStringHandler()
    {
        DefaultInterpolatedStringHandler handler = $"{_generatorContext.IntValue}{_generatorContext.IntValue}{_generatorContext.IntValue}{_generatorContext.IntValue}{_generatorContext.IntValue}";
        return handler.ToStringAndClear();
    }

    [Benchmark(Description = DescriptionCompositeFormat)]
    public string System_Text_CompositeFormat()
        => string.Format(CultureInfo.InvariantCulture, _compositeFormat, _generatorContext.IntValue);

    [Benchmark(Description = DescriptionRegex)]
    public string System_Text_RegularExpressions_Regex()
        => Regex.Replace(IntTemplate, _generatorContext.IntValue.ToString(CultureInfo.InvariantCulture));

    [Benchmark(Description = DescriptionScriban)]
    public string Scriban()
        => _scribanTemplate.Render(_scribanContext);

    [Benchmark(Description = DescriptionScribanLiquid)]
    public string Scriban_Liquid()
        => _scribanLiquidTemplate.Render(_scribanContext);
}
