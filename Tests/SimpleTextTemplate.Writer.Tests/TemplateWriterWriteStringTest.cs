using System.Buffers;
using System.Text;
using Shouldly;
using Xunit;

namespace SimpleTextTemplate.Writer.Tests;

public sealed class TemplateWriterWriteStringTest
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("a")]
    [InlineData("abc01234567890")]
    public void 文字列_バッファーライターに書き込み(string? value)
    {
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        writer.WriteString(value);
        writer.Flush();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .ShouldBe(value ?? string.Empty);
    }

    [Fact]
    public void 長い文字列_バッファーライターに書き込み()
    {
        var value = new string('a', 1024);
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        writer.WriteString(value);
        writer.Flush();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .ShouldBe(value);
    }

    [Fact]
    public void 文字列を複数回追加_バッファーライターに書き込み()
    {
        var value = new string('a', 30);
        var bufferWriter = new ArrayBufferWriter<byte>();
        var count = 0;

        var writer = TemplateWriter.Create(bufferWriter);

        for (; count < 10; count++)
        {
            writer.WriteString(value);
        }

        writer.Flush();

        var array = bufferWriter.WrittenSpan.ToArray();
        array.ShouldAllBe(static x => x == (byte)'a');
        array.Length.ShouldBe(value.Length * count);
    }
}
