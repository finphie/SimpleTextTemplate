using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Unicode;
using BenchmarkDotNet.Attributes;
using ScribanTemplate = Scriban.Template;

namespace SimpleTextTemplate.Benchmarks;

partial class RenderBenchmark
{
    const string IntCategory = "Int";
    const string IntTemplate = "abcdef{{ IntValue }}abcdef";

    Template _intTemplate;
    ScribanTemplate _intScribanTemplate;
    ScribanTemplate _intScribanLiquidTemplate;

    [Benchmark(Baseline = true, Description = DescriptionSimpleTextTemplateGenerator)]
    [BenchmarkCategory(IntCategory)]
    public byte[] SimpleTextTemplate_Generator_Int()
    {
        var writer = TemplateWriter.Create(_bufferWriter);
        writer.Write(IntTemplate, in _generatorContext);
        writer.Dispose();

        var result = _bufferWriter.WrittenSpan.ToArray();
        _bufferWriter.ResetWrittenCount();

        return result;
    }

    [Benchmark(Description = DescriptionSimpleTextTemplate)]
    [BenchmarkCategory(IntCategory)]
    public byte[] SimpleTextTemplate_Int()
    {
        _intTemplate.Render(_bufferWriter, _context);

        var result = _bufferWriter.WrittenSpan.ToArray();
        _bufferWriter.ResetWrittenCount();

        return result;
    }

    [Benchmark(Description = DescriptionScriban)]
    [BenchmarkCategory(IntCategory)]
    public string Scriban_Int() => _intScribanTemplate.Render(_scribanContext);

    [Benchmark(Description = DescriptionScribanLiquid)]
    [BenchmarkCategory(IntCategory)]
    public string ScribanLiquid_Int() => _intScribanLiquidTemplate.Render(_scribanContext);

    [Benchmark(Description = DescriptionUtf8TryWrite)]
    [BenchmarkCategory(IntCategory)]
    public byte[] Utf8TryWrite_Int()
    {
        Utf8.TryWrite(_bufferWriter.GetSpan(), $"abcdef{_generatorContext.IntValue}abcdef", out var bytesWritten);
        _bufferWriter.Advance(bytesWritten);

        var result = _bufferWriter.WrittenSpan.ToArray();
        _bufferWriter.ResetWrittenCount();

        return result;
    }

    [Benchmark(Description = DescriptionInterpolatedStringHandler)]
    [BenchmarkCategory(IntCategory)]
    public string InterpolatedStringHandler_Int()
    {
        DefaultInterpolatedStringHandler handler = $"abcdef{_generatorContext.IntValue}abcdef";
        return handler.ToStringAndClear();
    }

    [Benchmark(Description = DescriptionCompositeFormat)]
    [BenchmarkCategory(IntCategory)]
    public string CompositeFormat_Int()
        => string.Format(CultureInfo.InvariantCulture, _compositeFormat, _generatorContext.IntValue);

    void SetupInt()
    {
        _intTemplate = Template.Parse(Encoding.UTF8.GetBytes(IntTemplate));
        _intScribanTemplate = ScribanTemplate.Parse(IntTemplate);
        _intScribanLiquidTemplate = ScribanTemplate.ParseLiquid(IntTemplate);
    }
}
