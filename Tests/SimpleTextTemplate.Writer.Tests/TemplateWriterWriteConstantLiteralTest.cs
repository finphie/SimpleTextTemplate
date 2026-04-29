using System.Buffers;
using System.Text;
using SimpleTextTemplate.Tests.Assertions;

namespace SimpleTextTemplate.Writer.Tests;

public sealed class TemplateWriterWriteConstantLiteralTest
{
    [Test]
    [Arguments("0")]
    [Arguments("01")]
    [Arguments("012")]
    [Arguments("0123")]
    [Arguments("01234")]
    [Arguments("012345")]
    [Arguments("0123456")]
    [Arguments("01234567")]
    [Arguments("012345678")]
    [Arguments("0123456789")]
    public async Task 文字列_バッファーライターに書き込み(string value)
    {
        var utf8Value = Encoding.UTF8.GetBytes(value);
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        writer.WriteConstantLiteral(utf8Value);
        writer.Flush();

        await Assert.That(bufferWriter.WrittenMemory)
            .IsUtf8SequenceEqualTo(value);
    }

    [Test]
    [Arguments(1)]
    [Arguments(2)]
    [Arguments(3)]
    [Arguments(4)]
    [Arguments(5)]
    [Arguments(6)]
    [Arguments(7)]
    [Arguments(8)]
    [Arguments(9)]
    [Arguments(10)]
    public async Task 指定された長さの文字列を複数回追加_バッファーライターに書き込み(int length)
    {
        var value = new string('a', length);
        var utf8Value = Encoding.UTF8.GetBytes(value);
        var bufferWriter = new ArrayBufferWriter<byte>();
        var count = 0;

        var writer = TemplateWriter.Create(bufferWriter);

        for (; count < 10; count++)
        {
            writer.WriteConstantLiteral(utf8Value);
        }

        writer.Flush();

        await Assert.That(bufferWriter.WrittenMemory)
            .Count()
            .IsEqualTo(value.Length * count)
            .And
            .All(static x => x == 'a');
    }
}
