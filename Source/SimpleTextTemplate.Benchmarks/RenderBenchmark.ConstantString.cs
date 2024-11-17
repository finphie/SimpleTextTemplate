using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.Unicode;
using BenchmarkDotNet.Attributes;

namespace SimpleTextTemplate.Benchmarks;

partial class RenderBenchmark
{
    const string ConstantStringCategory = "Constant String";
    const string ConstantStringTemplate = "abcdef{{ ConstantStringValue }}abcdef";

    [Benchmark(Baseline = true, Description = DescriptionSimpleTextTemplateGenerator)]
    [BenchmarkCategory(ConstantStringCategory)]
    public byte[] SimpleTextTemplate_Generator_ConstantString()
    {
        var writer = TemplateWriter.Create(_bufferWriter);
        TemplateRenderer.Render(ref writer, ConstantStringTemplate, in _generatorContext);
        writer.Flush();

        var result = _bufferWriter.WrittenSpan.ToArray();
        _bufferWriter.ResetWrittenCount();

        return result;
    }

    [Benchmark(Description = DescriptionUtf8TryWrite)]
    [BenchmarkCategory(ConstantStringCategory)]
    public byte[] Utf8TryWrite_ConstantString()
    {
        Utf8.TryWrite(_bufferWriter.GetSpan(), $"abcdef{SampleContext.ConstantStringValue}abcdef", out var bytesWritten);
        _bufferWriter.Advance(bytesWritten);

        var result = _bufferWriter.WrittenSpan.ToArray();
        _bufferWriter.ResetWrittenCount();

        return result;
    }

    [Benchmark(Description = DescriptionInterpolatedStringHandler)]
    [BenchmarkCategory(ConstantStringCategory)]
    public string InterpolatedStringHandler_ConstantString()
    {
        DefaultInterpolatedStringHandler handler = $"abcdef{SampleContext.ConstantStringValue}abcdef";
        return handler.ToStringAndClear();
    }

    [Benchmark(Description = DescriptionCompositeFormat)]
    [BenchmarkCategory(ConstantStringCategory)]
    public string CompositeFormat_ConstantString() => string.Format(CultureInfo.InvariantCulture, _compositeFormat, SampleContext.ConstantStringValue);
}
