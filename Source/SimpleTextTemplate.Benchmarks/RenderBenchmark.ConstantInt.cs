using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.Unicode;
using BenchmarkDotNet.Attributes;

namespace SimpleTextTemplate.Benchmarks;

partial class RenderBenchmark
{
    const string ConstantIntCategory = "Constant Int";
    const string ConstantIntTemplate = "abcdef{{ ConstantIntValue }}abcdef";

    [Benchmark(Baseline = true, Description = DescriptionSimpleTextTemplateGenerator)]
    [BenchmarkCategory(ConstantIntCategory)]
    public byte[] SimpleTextTemplate_Generator_ConstantInt()
    {
        var writer = TemplateWriter.Create(_bufferWriter);
        writer.Write(ConstantIntTemplate, in _generatorContext);
        writer.Flush();

        var result = _bufferWriter.WrittenSpan.ToArray();
        _bufferWriter.ResetWrittenCount();

        return result;
    }

    [Benchmark(Description = DescriptionUtf8TryWrite)]
    [BenchmarkCategory(ConstantIntCategory)]
    public byte[] Utf8TryWrite_ConstantInt()
    {
        Utf8.TryWrite(_bufferWriter.GetSpan(), CultureInfo.InvariantCulture, $"abcdef{SampleContext.ConstantIntValue}abcdef", out var bytesWritten);
        _bufferWriter.Advance(bytesWritten);

        var result = _bufferWriter.WrittenSpan.ToArray();
        _bufferWriter.ResetWrittenCount();

        return result;
    }

    [Benchmark(Description = DescriptionInterpolatedStringHandler)]
    [BenchmarkCategory(ConstantIntCategory)]
    public string InterpolatedStringHandler_ConstantInt()
    {
        DefaultInterpolatedStringHandler handler = $"abcdef{SampleContext.ConstantIntValue}abcdef";
        return handler.ToStringAndClear();
    }

    [Benchmark(Description = DescriptionCompositeFormat)]
    [BenchmarkCategory(ConstantIntCategory)]
    public string CompositeFormat_ConstantInt() => string.Format(CultureInfo.InvariantCulture, _compositeFormat, SampleContext.ConstantIntValue);
}
