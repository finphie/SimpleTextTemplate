using System.Buffers;
using SimpleTextTemplate.Generator.Tests.TestData;
using SimpleTextTemplate.Tests.Assertions;

namespace SimpleTextTemplate.Generator.Execute.Tests;

public sealed class TemplateRendererRenderStringTest
{
    [Test]
    public async Task 定数()
    {
        var context = new StringContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, "{{ StringConstantField }}", in context);
        writer.Flush();

        await Assert.That(bufferWriter.WrittenMemory)
            .IsUtf8SequenceEqualTo("_StringConstantField");
    }

    [Test]
    public async Task 静的フィールド()
    {
        var context = new StringContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, "{{ StringStaticField }}", in context);
        writer.Flush();

        await Assert.That(bufferWriter.WrittenMemory)
            .IsUtf8SequenceEqualTo("_StringStaticField");
    }

    [Test]
    public async Task フィールド()
    {
        var context = new StringContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, "{{ StringField }}", in context);
        writer.Flush();

        await Assert.That(bufferWriter.WrittenMemory)
            .IsUtf8SequenceEqualTo("_StringField");
    }

    [Test]
    public async Task 静的プロパティ()
    {
        var context = new StringContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, "{{ StringStaticProperty }}", in context);
        writer.Flush();

        await Assert.That(bufferWriter.WrittenMemory)
            .IsUtf8SequenceEqualTo("_StringStaticProperty");
    }

    [Test]
    public async Task プロパティ()
    {
        var context = new StringContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, "{{ StringProperty }}", in context);
        writer.Flush();

        await Assert.That(bufferWriter.WrittenMemory)
            .IsUtf8SequenceEqualTo("_StringProperty");
    }
}
