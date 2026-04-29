using System.Buffers;
using SimpleTextTemplate.Generator.Tests.TestData;
using SimpleTextTemplate.Tests.Assertions;
using static SimpleTextTemplate.TemplateRenderer;

namespace SimpleTextTemplate.Generator.Execute.Tests;

public sealed class TemplateRendererRenderUsingStaticTest
{
    [Test]
    public async Task 静的フィールド()
    {
        var context = new ByteArrayContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        Render(ref writer, "{{ BytesStaticField }}", in context);
        writer.Flush();

        await Assert.That(bufferWriter.WrittenMemory)
            .IsUtf8SequenceEqualTo("_BytesStaticField");
    }

    [Test]
    public async Task フィールド()
    {
        var context = new ByteArrayContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        Render(ref writer, "{{ BytesField }}", in context);
        writer.Flush();

        await Assert.That(bufferWriter.WrittenMemory)
            .IsUtf8SequenceEqualTo("_BytesField");
    }

    [Test]
    public async Task 静的プロパティ()
    {
        var context = new ByteArrayContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        Render(ref writer, "{{ BytesStaticProperty }}", in context);
        writer.Flush();

        await Assert.That(bufferWriter.WrittenMemory)
            .IsUtf8SequenceEqualTo("_BytesStaticProperty");
    }

    [Test]
    public async Task プロパティ()
    {
        var context = new ByteArrayContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        Render(ref writer, "{{ BytesProperty }}", in context);
        writer.Flush();

        await Assert.That(bufferWriter.WrittenMemory)
            .IsUtf8SequenceEqualTo("_BytesProperty");
    }
}
