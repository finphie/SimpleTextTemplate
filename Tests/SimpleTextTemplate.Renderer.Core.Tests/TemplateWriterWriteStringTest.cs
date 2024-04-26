using System.Buffers;
using System.Text;
using FluentAssertions;
using Xunit;

namespace SimpleTextTemplate.Renderer.Core.Tests;

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

        using (var writer = TemplateWriter.Create(bufferWriter))
        {
            writer.WriteString(value);
        }

        bufferWriter.WrittenSpan.ToArray()
            .Should()
            .Equal(Encoding.UTF8.GetBytes(value ?? string.Empty));
    }

    [Fact]
    public void 長い文字列_バッファーライターに書き込み()
    {
        var value = new string('a', 1024);
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = TemplateWriter.Create(bufferWriter))
        {
            writer.WriteString(value);
        }

        bufferWriter.WrittenSpan.ToArray()
            .Should()
            .Equal(Encoding.UTF8.GetBytes(value));
    }

    [Fact]
    public void 文字列を複数回追加_バッファーライターに書き込み()
    {
        var value = new string('a', 30);
        var bufferWriter = new ArrayBufferWriter<byte>();
        var count = 0;

        using (var writer = TemplateWriter.Create(bufferWriter))
        {
            for (; count < 10; count++)
            {
                writer.WriteString(value);
            }
        }

        var array = bufferWriter.WrittenSpan.ToArray();
        array.Should().OnlyContain(static x => x == (byte)'a');
        array.Should().HaveCount(value.Length * count);
    }
}
