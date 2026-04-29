using System.Buffers;
using SimpleTextTemplate.Generator.Tests.TestData;
using SimpleTextTemplate.Tests.Assertions;

namespace SimpleTextTemplate.Generator.Execute.Tests;

public sealed class TemplateRendererRenderByteArrayTest
{
    [Test]
    public async Task 静的フィールド()
    {
        var context = new ByteArrayContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, "{{ BytesStaticField }}", in context);
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
        TemplateRenderer.Render(ref writer, "{{ BytesField }}", in context);
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
        TemplateRenderer.Render(ref writer, "{{ BytesStaticProperty }}", in context);
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
        TemplateRenderer.Render(ref writer, "{{ BytesProperty }}", in context);
        writer.Flush();

        await Assert.That(bufferWriter.WrittenMemory)
            .IsUtf8SequenceEqualTo("_BytesProperty");
    }

    [Test]
    public async Task 静的ReadOnlySpanプロパティ()
    {
        var context = new ByteArrayContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, "{{ BytesSpanStaticProperty }}", in context);
        writer.Flush();

        await Assert.That(bufferWriter.WrittenMemory)
            .IsUtf8SequenceEqualTo("_BytesSpanStaticProperty");
    }

    [Test]
    public async Task ReadOnlySpanプロパティ()
    {
        var context = new ByteArrayContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, "{{ BytesSpanProperty }}", in context);
        writer.Flush();

        await Assert.That(bufferWriter.WrittenMemory)
            .IsUtf8SequenceEqualTo("_BytesSpanProperty");
    }
}
