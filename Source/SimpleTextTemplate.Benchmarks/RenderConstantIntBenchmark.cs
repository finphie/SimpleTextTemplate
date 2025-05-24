using System.Buffers;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Unicode;
using BenchmarkDotNet.Attributes;
using static SimpleTextTemplate.Benchmarks.Constants;

namespace SimpleTextTemplate.Benchmarks;

public class RenderConstantIntBenchmark
{
    const string ConstantIntTemplate = "abcdef{{ ConstantIntValue }}abcdef";

    readonly ArrayBufferWriter<byte> _bufferWriter = new();

    CompositeFormat _compositeFormat;

    SampleContext _generatorContext;

    [GlobalSetup]
    public void Setup()
    {
        _compositeFormat = CompositeFormat.Parse(Format);

        _generatorContext = new();
    }

    [Benchmark(Baseline = true, Description = DescriptionSimpleTextTemplateGenerator)]
    public byte[] SimpleTextTemplate_Generator_ConstantInt()
    {
        var writer = TemplateWriter.Create(_bufferWriter);
        TemplateRenderer.Render(ref writer, ConstantIntTemplate, in _generatorContext);
        writer.Flush();

        var result = _bufferWriter.WrittenSpan.ToArray();
        _bufferWriter.ResetWrittenCount();

        return result;
    }

    [Benchmark(Description = DescriptionUtf8TryWrite)]
    public byte[] Utf8TryWrite_ConstantInt()
    {
        Utf8.TryWrite(_bufferWriter.GetSpan(), CultureInfo.InvariantCulture, $"abcdef{SampleContext.ConstantIntValue}abcdef", out var bytesWritten);
        _bufferWriter.Advance(bytesWritten);

        var result = _bufferWriter.WrittenSpan.ToArray();
        _bufferWriter.ResetWrittenCount();

        return result;
    }

    [Benchmark(Description = DescriptionInterpolatedStringHandler)]
    public string InterpolatedStringHandler_ConstantInt()
    {
        DefaultInterpolatedStringHandler handler = $"abcdef{SampleContext.ConstantIntValue}abcdef";
        return handler.ToStringAndClear();
    }

    [Benchmark(Description = DescriptionCompositeFormat)]
    public string CompositeFormat_ConstantInt()
        => string.Format(CultureInfo.InvariantCulture, _compositeFormat, SampleContext.ConstantIntValue);
}
