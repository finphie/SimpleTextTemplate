using System.Buffers;
using SimpleTextTemplate.Tests.Assertions;
using SimpleTextTemplate.Tests.TestData;

namespace SimpleTextTemplate.Generator.Execute.Tests;

public sealed class TemplateRendererRenderNonFormattableTest
{
    [Test]
    public async Task 静的フィールド()
    {
        var context = new NonFormattableContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, "{{ NonFormattableStaticField }}", in context);
        writer.Flush();

        await Assert.That(bufferWriter.WrittenMemory)
            .IsUtf8SequenceEqualTo("_NonFormattableStaticField");
    }

    [Test]
    public async Task フィールド()
    {
        var context = new NonFormattableContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, "{{ NonFormattableField }}", in context);
        writer.Flush();

        await Assert.That(bufferWriter.WrittenMemory)
            .IsUtf8SequenceEqualTo("_NonFormattableField");
    }

    [Test]
    public async Task 静的プロパティ()
    {
        var context = new NonFormattableContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, "{{ NonFormattableStaticProperty }}", in context);
        writer.Flush();

        await Assert.That(bufferWriter.WrittenMemory)
            .IsUtf8SequenceEqualTo("_NonFormattableStaticProperty");
    }

    [Test]
    public async Task プロパティ()
    {
        var context = new NonFormattableContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, "{{ NonFormattableProperty }}", in context);
        writer.Flush();

        await Assert.That(bufferWriter.WrittenMemory)
            .IsUtf8SequenceEqualTo("_NonFormattableProperty");
    }
}
