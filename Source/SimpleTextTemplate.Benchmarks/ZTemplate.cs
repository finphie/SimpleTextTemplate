using System.Buffers;

namespace SimpleTextTemplate.Benchmarks;

/// <summary>
/// テンプレートレンダリングを行うクラスです。
/// </summary>
static partial class ZTemplate
{
    public const string Identifier = nameof(SampleContext.Identifier);
    public const string Source = "{{ " + Identifier + " }}";

    [Template(Source)]
    public static partial void Render(IBufferWriter<byte> bufferWriter, SampleContext context);
}
