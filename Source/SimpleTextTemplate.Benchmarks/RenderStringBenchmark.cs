using System.Buffers;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Unicode;
using BenchmarkDotNet.Attributes;
using static SimpleTextTemplate.Benchmarks.Constants;
using ScribanTemplate = Scriban.Template;

namespace SimpleTextTemplate.Benchmarks;

public class RenderStringBenchmark
{
    const string StringTemplate = "{{ StringValue }}{{ StringValue }}{{ StringValue }}{{ StringValue }}{{ StringValue }}";

    readonly ArrayBufferWriter<byte> _bufferWriter = new();

    Template _stringTemplate;
    ScribanTemplate _scribanTemplate;
    ScribanTemplate _scribanLiquidTemplate;
    CompositeFormat _compositeFormat;

    SampleContext _generatorContext;
    Dictionary<byte[], object> _context;
    Dictionary<string, object> _scribanContext;

    [GlobalSetup]
    public void Setup()
    {
        _stringTemplate = Template.Parse(Encoding.UTF8.GetBytes(StringTemplate));
        _scribanTemplate = ScribanTemplate.Parse(StringTemplate);
        _scribanLiquidTemplate = ScribanTemplate.ParseLiquid(StringTemplate);
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
    public byte[] SimpleTextTemplate_Generator()
    {
        _bufferWriter.ResetWrittenCount();

        var writer = TemplateWriter.Create(_bufferWriter);
        TemplateRenderer.Render(ref writer, StringTemplate, in _generatorContext);
        writer.Flush();

        return _bufferWriter.WrittenSpan.ToArray();
    }

    [Benchmark(Description = DescriptionSimpleTextTemplate)]
    public byte[] SimpleTextTemplate()
    {
        _bufferWriter.ResetWrittenCount();

        _stringTemplate.Render(_bufferWriter, _context);

        return _bufferWriter.WrittenSpan.ToArray();
    }

    [Benchmark(Description = DescriptionUtf8TryWrite)]
    public byte[] Utf8_TryWrite()
    {
        _bufferWriter.ResetWrittenCount();

        Utf8.TryWrite(
            _bufferWriter.GetSpan(),
            $"{_generatorContext.StringValue}{_generatorContext.StringValue}{_generatorContext.StringValue}{_generatorContext.StringValue}{_generatorContext.StringValue}",
            out var bytesWritten);
        _bufferWriter.Advance(bytesWritten);

        return _bufferWriter.WrittenSpan.ToArray();
    }

    [Benchmark(Description = DescriptionInterpolatedStringHandler)]
    public string InterpolatedStringHandler()
    {
        _bufferWriter.ResetWrittenCount();

        DefaultInterpolatedStringHandler handler = $"{_generatorContext.StringValue}{_generatorContext.StringValue}{_generatorContext.StringValue}{_generatorContext.StringValue}{_generatorContext.StringValue}";
        return handler.ToStringAndClear();
    }

    [Benchmark(Description = DescriptionCompositeFormat)]
    public string System_Text_CompositeFormat()
    {
        _bufferWriter.ResetWrittenCount();

        return string.Format(CultureInfo.InvariantCulture, _compositeFormat, _generatorContext.StringValue);
    }

    [Benchmark(Description = DescriptionScriban)]
    public string Scriban()
    {
        _bufferWriter.ResetWrittenCount();

        return _scribanTemplate.Render(_scribanContext);
    }

    [Benchmark(Description = DescriptionScribanLiquid)]
    public string Scriban_Liquid()
    {
        _bufferWriter.ResetWrittenCount();

        return _scribanLiquidTemplate.Render(_scribanContext);
    }
}
