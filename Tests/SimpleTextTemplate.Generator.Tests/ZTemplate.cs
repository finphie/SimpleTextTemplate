using System.Buffers;

namespace SimpleTextTemplate.Generator.Tests;

/// <summary>
/// テンプレートレンダリングを行うクラスです。
/// </summary>
static partial class ZTemplate
{
    [Template("{{ Identifier }}")]
    public static partial void Render(IBufferWriter<byte> bufferWriter, TestContext context);

    [TemplateFile("Templates/Test.txt")]
    public static partial void FileRender(IBufferWriter<byte> bufferWriter, TestContext context);
}
