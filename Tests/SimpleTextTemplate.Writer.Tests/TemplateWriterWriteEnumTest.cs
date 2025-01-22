using System.Buffers;
using System.Text;
using Shouldly;
using Xunit;

namespace SimpleTextTemplate.Writer.Tests;

public sealed class TemplateWriterWriteEnumTest
{
    enum Test1
    {
        A
    }

    enum Test2
    {
        AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
    }

    [Fact]
    public void 書式指定なし_バッファーライターに書き込み()
    {
        const Test1 Value = Test1.A;
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        writer.WriteEnum(Value);
        writer.Flush();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .ShouldBe("A");
    }

    [Fact]
    public void 書式指定_バッファーライターに書き込み()
    {
        const Test1 Value = Test1.A;
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        writer.WriteEnum(Value, "D");
        writer.Flush();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .ShouldBe("0");
    }

    [Fact]
    public void Enumを複数回追加_バッファーライターに書き込み()
    {
        const Test2 Value = Test2.AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA;
        var bufferWriter = new ArrayBufferWriter<byte>();
        var count = 0;

        var writer = TemplateWriter.Create(bufferWriter);

        for (; count < 10; count++)
        {
            writer.WriteEnum(Value);
        }

        writer.Flush();

        var array = Encoding.UTF8.GetString(bufferWriter.WrittenSpan);
        array.ShouldAllBe(static x => x == (byte)'A');
        array.Length.ShouldBe(30 * count);
    }
}
