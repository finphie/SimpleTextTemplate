using System.Buffers;
using SimpleTextTemplate.Tests.Assertions;
using SimpleTextTemplate.Tests.TestData;

namespace SimpleTextTemplate.Generator.Execute.Tests;

public sealed class TemplateRendererRenderCharArrayTest
{
    [Test]
    public async Task 静的フィールド()
    {
        var context = new CharArrayContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, "{{ CharsStaticField }}", in context);
        writer.Flush();

        await Assert.That(bufferWriter.WrittenMemory)
            .IsUtf8SequenceEqualTo("_CharsStaticField");
    }

    [Test]
    public async Task フィールド()
    {
        var context = new CharArrayContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, "{{ CharsField }}", in context);
        writer.Flush();

        await Assert.That(bufferWriter.WrittenMemory)
            .IsUtf8SequenceEqualTo("_CharsField");
    }

    [Test]
    public async Task 静的プロパティ()
    {
        var context = new CharArrayContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, "{{ CharsStaticProperty }}", in context);
        writer.Flush();

        await Assert.That(bufferWriter.WrittenMemory)
            .IsUtf8SequenceEqualTo("_CharsStaticProperty");
    }

    [Test]
    public async Task プロパティ()
    {
        var context = new CharArrayContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, "{{ CharsProperty }}", in context);
        writer.Flush();

        await Assert.That(bufferWriter.WrittenMemory)
            .IsUtf8SequenceEqualTo("_CharsProperty");
    }

    [Test]
    public async Task 静的ReadOnlySpanプロパティ()
    {
        var context = new CharArrayContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, "{{ CharsSpanStaticProperty }}", in context);
        writer.Flush();

        await Assert.That(bufferWriter.WrittenMemory)
            .IsUtf8SequenceEqualTo("_CharsSpanStaticProperty");
    }

    [Test]
    public async Task ReadOnlySpanプロパティ()
    {
        var context = new CharArrayContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, "{{ CharsSpanProperty }}", in context);
        writer.Flush();

        await Assert.That(bufferWriter.WrittenMemory)
            .IsUtf8SequenceEqualTo("_CharsSpanProperty");
    }
}
