using System.Buffers;
using SimpleTextTemplate.Tests.Assertions;

namespace SimpleTextTemplate.Generator.Execute.Tests;

public sealed class TemplateRendererRenderAccessibilityTest
{
    [Test]
    public async Task 静的Internalフィールド()
    {
        var context = new AccessibilityTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, "{{ _internalStaticField }}", in context);
        writer.Flush();

        await Assert.That(bufferWriter.WrittenMemory)
            .IsUtf8SequenceEqualTo("InternalStaticField");
    }

    [Test]
    public async Task Internalフィールド()
    {
        var context = new AccessibilityTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, "{{ _internalField }}", in context);
        writer.Flush();

        await Assert.That(bufferWriter.WrittenMemory)
            .IsUtf8SequenceEqualTo("InternalField");
    }

    [Test]
    public async Task 静的ProtectedInternalフィールド()
    {
        var context = new AccessibilityTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, "{{ ProtectedInternalStaticField }}", in context);
        writer.Flush();

        await Assert.That(bufferWriter.WrittenMemory)
            .IsUtf8SequenceEqualTo("_ProtectedInternalStaticField");
    }

    [Test]
    public async Task ProtectedInternalフィールド()
    {
        var context = new AccessibilityTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, "{{ ProtectedInternalField }}", in context);
        writer.Flush();

        await Assert.That(bufferWriter.WrittenMemory)
            .IsUtf8SequenceEqualTo("_ProtectedInternalField");
    }

    [Test]
    public async Task 静的Internalプロパティ()
    {
        var context = new AccessibilityTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, "{{ InternalStaticProperty }}", in context);
        writer.Flush();

        await Assert.That(bufferWriter.WrittenMemory)
            .IsUtf8SequenceEqualTo("_InternalStaticProperty");
    }

    [Test]
    public async Task Internalプロパティ()
    {
        var context = new AccessibilityTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, "{{ InternalProperty }}", in context);
        writer.Flush();

        await Assert.That(bufferWriter.WrittenMemory)
            .IsUtf8SequenceEqualTo("_InternalProperty");
    }

    [Test]
    public async Task 静的ProtectedInternalプロパティ()
    {
        var context = new AccessibilityTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, "{{ ProtectedInternalStaticProperty }}", in context);
        writer.Flush();

        await Assert.That(bufferWriter.WrittenMemory)
            .IsUtf8SequenceEqualTo("_ProtectedInternalStaticProperty");
    }

    [Test]
    public async Task ProtectedInternalプロパティ()
    {
        var context = new AccessibilityTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, "{{ ProtectedInternalProperty }}", in context);
        writer.Flush();

        await Assert.That(bufferWriter.WrittenMemory)
            .IsUtf8SequenceEqualTo("_ProtectedInternalProperty");
    }
}
