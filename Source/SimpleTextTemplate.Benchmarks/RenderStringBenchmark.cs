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
    const string StringTemplate = "abcdef{{ StringValue }}abcdef";

    readonly ArrayBufferWriter<byte> _bufferWriter = new();

    Template _stringTemplate;
    ScribanTemplate _stringScribanTemplate;
    ScribanTemplate _stringScribanLiquidTemplate;
    CompositeFormat _compositeFormat;

    SampleContext _generatorContext;
    Dictionary<byte[], object> _context;
    Dictionary<string, object> _scribanContext;

    [GlobalSetup]
    public void Setup()
    {
        _stringTemplate = Template.Parse(Encoding.UTF8.GetBytes(StringTemplate));
        _stringScribanTemplate = ScribanTemplate.Parse(StringTemplate);
        _stringScribanLiquidTemplate = ScribanTemplate.ParseLiquid(StringTemplate);
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
    public byte[] SimpleTextTemplate_Generator_String()
    {
        var writer = TemplateWriter.Create(_bufferWriter);
        TemplateRenderer.Render(ref writer, StringTemplate, in _generatorContext);
        writer.Flush();

        var result = _bufferWriter.WrittenSpan.ToArray();
        _bufferWriter.ResetWrittenCount();

        return result;
    }

    [Benchmark(Description = DescriptionSimpleTextTemplate)]
    public byte[] SimpleTextTemplate_String()
    {
        _stringTemplate.Render(_bufferWriter, _context);

        var result = _bufferWriter.WrittenSpan.ToArray();
        _bufferWriter.ResetWrittenCount();

        return result;
    }

    [Benchmark(Description = DescriptionScriban)]
    public string Scriban_String() => _stringScribanTemplate.Render(_scribanContext);

    [Benchmark(Description = DescriptionScribanLiquid)]
    public string ScribanLiquid_String() => _stringScribanLiquidTemplate.Render(_scribanContext);

    [Benchmark(Description = DescriptionUtf8TryWrite)]
    public byte[] Utf8TryWrite_String()
    {
        Utf8.TryWrite(_bufferWriter.GetSpan(), $"abcdef{_generatorContext.StringValue}abcdef", out var bytesWritten);
        _bufferWriter.Advance(bytesWritten);

        var result = _bufferWriter.WrittenSpan.ToArray();
        _bufferWriter.ResetWrittenCount();

        return result;
    }

    [Benchmark(Description = DescriptionInterpolatedStringHandler)]
    public string InterpolatedStringHandler_String()
    {
        DefaultInterpolatedStringHandler handler = $"abcdef{_generatorContext.StringValue}abcdef";
        return handler.ToStringAndClear();
    }

    [Benchmark(Description = DescriptionCompositeFormat)]
    public string CompositeFormat_String()
        => string.Format(CultureInfo.InvariantCulture, _compositeFormat, _generatorContext.StringValue);
}
