using System.Buffers;
using System.Text;
using FluentAssertions;
using Xunit;

namespace SimpleTextTemplate.Writer.Tests;

public sealed class TemplateWriterWriteLiteralTest
{
    [Theory]
    [InlineData("")]
    [InlineData("a")]
    [InlineData("abc01234567890")]
    public void 文字列_バッファーライターに書き込み(string value)
    {
        var utf8Value = Encoding.UTF8.GetBytes(value);
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        writer.WriteLiteral(utf8Value);
        writer.Flush();

        bufferWriter.WrittenSpan.ToArray()
            .Should()
            .Equal(utf8Value);
    }

    [Fact]
    public void 長い文字列_バッファーライターに書き込み()
    {
        var value = new string('a', 1024);
        var utf8Value = Encoding.UTF8.GetBytes(value);
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        writer.WriteLiteral(utf8Value);
        writer.Flush();

        bufferWriter.WrittenSpan.ToArray()
            .Should()
            .Equal(utf8Value);
    }

    [Fact]
    public void 文字列を複数回追加_バッファーライターに書き込み()
    {
        var value = new string('a', 30);
        var utf8Value = Encoding.UTF8.GetBytes(value);
        var bufferWriter = new ArrayBufferWriter<byte>();
        var count = 0;

        var writer = TemplateWriter.Create(bufferWriter);

        for (; count < 10; count++)
        {
            writer.WriteLiteral(utf8Value);
        }

        writer.Flush();

        var array = bufferWriter.WrittenSpan.ToArray();
        array.Should().OnlyContain(static x => x == (byte)'a');
        array.Should().HaveCount(value.Length * count);
    }
}
