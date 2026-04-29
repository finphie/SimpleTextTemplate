using System.Buffers;
using SimpleTextTemplate.Tests.Assertions;

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

    [Test]
    public async Task 書式指定なし_バッファーライターに書き込み()
    {
        const Test1 Value = Test1.A;
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        writer.WriteEnum(Value);
        writer.Flush();

        await Assert.That(bufferWriter.WrittenMemory)
            .IsUtf8SequenceEqualTo("A");
    }

    [Test]
    public async Task 書式指定_バッファーライターに書き込み()
    {
        const Test1 Value = Test1.A;
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        writer.WriteEnum(Value, "D");
        writer.Flush();

        await Assert.That(bufferWriter.WrittenMemory)
            .IsUtf8SequenceEqualTo("0");
    }

    [Test]
    public async Task Enumを複数回追加_バッファーライターに書き込み()
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

        await Assert.That(bufferWriter.WrittenMemory)
            .Count()
            .IsEqualTo(nameof(Test2.AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA).Length * count)
            .And
            .All(static x => x == 'A');
    }
}
