using System.Buffers;
using SimpleTextTemplate.Tests.Assertions;

namespace SimpleTextTemplate.Writer.Tests;

public sealed class TemplateWriterWriteStringTest
{
    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("a")]
    [Arguments("abc01234567890")]
    public async Task 文字列_バッファーライターに書き込み(string? value)
    {
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        writer.WriteString(value);
        writer.Flush();

        await Assert.That(bufferWriter.WrittenMemory)
            .IsUtf8SequenceEqualTo(value);
    }

    [Test]
    public async Task 長い文字列_バッファーライターに書き込み()
    {
        var value = new string('a', 1024);
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        writer.WriteString(value);
        writer.Flush();

        await Assert.That(bufferWriter.WrittenMemory)
            .IsUtf8SequenceEqualTo(value);
    }

    [Test]
    public async Task 文字列を複数回追加_バッファーライターに書き込み()
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

        await Assert.That(bufferWriter.WrittenMemory)
            .Count()
            .IsEqualTo(value.Length * count)
            .And
            .All(static x => x == 'a');
    }
}
