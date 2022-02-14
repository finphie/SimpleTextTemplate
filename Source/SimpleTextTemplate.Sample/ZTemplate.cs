using System.Buffers;

namespace SimpleTextTemplate.Sample;

/// <summary>
/// テンプレートレンダリングを行うクラスです。
/// </summary>
static partial class ZTemplate
{
    [Template("{{ Identifier }}")]
    public static partial void Render(IBufferWriter<byte> bufferWriter, SampleContext context);

    [TemplateFile("Templates/Pages.html")]
    public static partial void RenderFile(IBufferWriter<byte> bufferWriter, SampleContext context);
}
