using System.Buffers;
using System.Text;
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
        var value = Test1.A;
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = new TemplateWriter<ArrayBufferWriter<byte>>(ref bufferWriter))
        {
            writer.WriteEnum(value);
        }

        bufferWriter.WrittenSpan.ToArray()
            .Should()
            .Equal(Encoding.UTF8.GetBytes(value.ToString()));
    }

    [Fact]
    public void 書式指定_バッファーライターに書き込み()
    {
        var value = Test1.A;
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = new TemplateWriter<ArrayBufferWriter<byte>>(ref bufferWriter))
        {
            writer.WriteEnum(value, "G");
        }

        bufferWriter.WrittenSpan.ToArray()
            .Should()
            .Equal(Encoding.UTF8.GetBytes(value.ToString("G")));
    }

    [Fact]
    public void Enumを複数回追加_バッファーライターに書き込み()
    {
        var value = Test2.AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA;
        var bufferWriter = new ArrayBufferWriter<byte>();
        var count = 0;

        using (var writer = new TemplateWriter<ArrayBufferWriter<byte>>(ref bufferWriter))
        {
            for (; count < 10; count++)
            {
                writer.WriteEnum(value);
            }
        }

        var array = bufferWriter.WrittenSpan.ToArray();
        array.Should().OnlyContain(static x => x == (byte)'A');
        array.Should().HaveCount(30 * count);
    }
}
