using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Unicode;
using BenchmarkDotNet.Attributes;
using ScribanTemplate = Scriban.Template;

namespace SimpleTextTemplate.Benchmarks;

partial class RenderBenchmark
{
    const string StringCategory = "String";
    const string StringTemplate = "abcdef{{ StringValue }}abcdef";

    Template _stringTemplate;
    ScribanTemplate _stringScribanTemplate;
    ScribanTemplate _stringScribanLiquidTemplate;

    [Benchmark(Baseline = true, Description = DescriptionSimpleTextTemplateGenerator)]
    [BenchmarkCategory(StringCategory)]
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
    [BenchmarkCategory(StringCategory)]
    public byte[] SimpleTextTemplate_String()
    {
        _stringTemplate.Render(_bufferWriter, _context);

        var result = _bufferWriter.WrittenSpan.ToArray();
        _bufferWriter.ResetWrittenCount();

        return result;
    }

    [Benchmark(Description = DescriptionScriban)]
    [BenchmarkCategory(StringCategory)]
    public string Scriban_String() => _stringScribanTemplate.Render(_scribanContext);

    [Benchmark(Description = DescriptionScribanLiquid)]
    [BenchmarkCategory(StringCategory)]
    public string ScribanLiquid_String() => _stringScribanLiquidTemplate.Render(_scribanContext);

    [Benchmark(Description = DescriptionUtf8TryWrite)]
    [BenchmarkCategory(StringCategory)]
    public byte[] Utf8TryWrite_String()
    {
        Utf8.TryWrite(_bufferWriter.GetSpan(), $"abcdef{_generatorContext.StringValue}abcdef", out var bytesWritten);
        _bufferWriter.Advance(bytesWritten);

        var result = _bufferWriter.WrittenSpan.ToArray();
        _bufferWriter.ResetWrittenCount();

        return result;
    }

    [Benchmark(Description = DescriptionInterpolatedStringHandler)]
    [BenchmarkCategory(StringCategory)]
    public string InterpolatedStringHandler_String()
    {
        DefaultInterpolatedStringHandler handler = $"abcdef{_generatorContext.StringValue}abcdef";
        return handler.ToStringAndClear();
    }

    [Benchmark(Description = DescriptionCompositeFormat)]
    [BenchmarkCategory(StringCategory)]
    public string CompositeFormat_String()
        => string.Format(CultureInfo.InvariantCulture, _compositeFormat, _generatorContext.StringValue);

    void SetupString()
    {
        _stringTemplate = Template.Parse(Encoding.UTF8.GetBytes(StringTemplate));
        _stringScribanTemplate = ScribanTemplate.Parse(StringTemplate);
        _stringScribanLiquidTemplate = ScribanTemplate.ParseLiquid(StringTemplate);
    }
}
