using System.Buffers;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Unicode;
using BenchmarkDotNet.Attributes;
using static SimpleTextTemplate.Benchmarks.Constants;
using ScribanTemplate = Scriban.Template;

namespace SimpleTextTemplate.Benchmarks;

public class RenderIntBenchmark
{
    const string IntTemplate = "abcdef{{ IntValue }}abcdef";

    readonly ArrayBufferWriter<byte> _bufferWriter = new();

    Template _intTemplate;
    ScribanTemplate _intScribanTemplate;
    ScribanTemplate _intScribanLiquidTemplate;
    CompositeFormat _compositeFormat;

    SampleContext _generatorContext;
    Dictionary<byte[], object> _context;
    Dictionary<string, object> _scribanContext;

    [GlobalSetup]
    public void Setup()
    {
        _intTemplate = Template.Parse(Encoding.UTF8.GetBytes(IntTemplate));
        _intScribanTemplate = ScribanTemplate.Parse(IntTemplate);
        _intScribanLiquidTemplate = ScribanTemplate.ParseLiquid(IntTemplate);
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
    public byte[] SimpleTextTemplate_Generator_Int()
    {
        var writer = TemplateWriter.Create(_bufferWriter);
        TemplateRenderer.Render(ref writer, IntTemplate, in _generatorContext);
        writer.Flush();

        var result = _bufferWriter.WrittenSpan.ToArray();
        _bufferWriter.ResetWrittenCount();

        return result;
    }

    [Benchmark(Description = DescriptionSimpleTextTemplate)]
    public byte[] SimpleTextTemplate_Int()
    {
        _intTemplate.Render(_bufferWriter, _context);

        var result = _bufferWriter.WrittenSpan.ToArray();
        _bufferWriter.ResetWrittenCount();

        return result;
    }

    [Benchmark(Description = DescriptionScriban)]
    public string Scriban_Int() => _intScribanTemplate.Render(_scribanContext);

    [Benchmark(Description = DescriptionScribanLiquid)]
    public string ScribanLiquid_Int() => _intScribanLiquidTemplate.Render(_scribanContext);

    [Benchmark(Description = DescriptionUtf8TryWrite)]
    public byte[] Utf8TryWrite_Int()
    {
        Utf8.TryWrite(_bufferWriter.GetSpan(), $"abcdef{_generatorContext.IntValue}abcdef", out var bytesWritten);
        _bufferWriter.Advance(bytesWritten);

        var result = _bufferWriter.WrittenSpan.ToArray();
        _bufferWriter.ResetWrittenCount();

        return result;
    }

    [Benchmark(Description = DescriptionInterpolatedStringHandler)]
    public string InterpolatedStringHandler_Int()
    {
        DefaultInterpolatedStringHandler handler = $"abcdef{_generatorContext.IntValue}abcdef";
        return handler.ToStringAndClear();
    }

    [Benchmark(Description = DescriptionCompositeFormat)]
    public string CompositeFormat_Int()
        => string.Format(CultureInfo.InvariantCulture, _compositeFormat, _generatorContext.IntValue);
}
