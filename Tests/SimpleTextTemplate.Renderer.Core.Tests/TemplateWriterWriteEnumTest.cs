using System.Buffers;
using FluentAssertions;
using Xunit;

namespace SimpleTextTemplate.Renderer.Core.Tests;

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

        using (var writer = TemplateWriter.Create(bufferWriter))
        {
            writer.WriteEnum(Value);
        }

        bufferWriter.WrittenSpan.ToArray()
            .Should()
            .Equal("A"u8.ToArray());
    }

    [Fact]
    public void 書式指定_バッファーライターに書き込み()
    {
        const Test1 Value = Test1.A;
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = TemplateWriter.Create(bufferWriter))
        {
            writer.WriteEnum(Value, "D");
        }

        bufferWriter.WrittenSpan.ToArray()
            .Should()
            .Equal("0"u8.ToArray());
    }

    [Fact]
    public void Enumを複数回追加_バッファーライターに書き込み()
    {
        const Test2 Value = Test2.AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA;
        var bufferWriter = new ArrayBufferWriter<byte>();
        var count = 0;

        using (var writer = TemplateWriter.Create(bufferWriter))
        {
            for (; count < 10; count++)
            {
                writer.WriteEnum(Value);
            }
        }

        var array = bufferWriter.WrittenSpan.ToArray();
        array.Should().OnlyContain(static x => x == (byte)'A');
        array.Should().HaveCount(30 * count);
    }
}
