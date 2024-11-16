using System.Buffers;
using System.Text;
using FluentAssertions;
using Xunit;

namespace SimpleTextTemplate.Generator.Execute.Tests;

public sealed class TemplateRenderAccessibilityTest
{
    [Fact]
    public void 静的Internalフィールド()
    {
        var context = new AccessibilityTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        Template.Render(ref writer, "{{ _internalStaticField }}", in context);
        writer.Dispose();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("InternalStaticField");
    }

    [Fact]
    public void Internalフィールド()
    {
        var context = new AccessibilityTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        Template.Render(ref writer, "{{ _internalField }}", in context);
        writer.Dispose();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("InternalField");
    }

    [Fact]
    public void 静的ProtectedInternalフィールド()
    {
        var context = new AccessibilityTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        Template.Render(ref writer, "{{ ProtectedInternalStaticField }}", in context);
        writer.Dispose();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("_ProtectedInternalStaticField");
    }

    [Fact]
    public void ProtectedInternalフィールド()
    {
        var context = new AccessibilityTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        Template.Render(ref writer, "{{ ProtectedInternalField }}", in context);
        writer.Dispose();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("_ProtectedInternalField");
    }

    [Fact]
    public void 静的Internalプロパティ()
    {
        var context = new AccessibilityTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        Template.Render(ref writer, "{{ InternalStaticProperty }}", in context);
        writer.Dispose();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("_InternalStaticProperty");
    }

    [Fact]
    public void Internalプロパティ()
    {
        var context = new AccessibilityTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        Template.Render(ref writer, "{{ InternalProperty }}", in context);
        writer.Dispose();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("_InternalProperty");
    }

    [Fact]
    public void 静的ProtectedInternalプロパティ()
    {
        var context = new AccessibilityTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        Template.Render(ref writer, "{{ ProtectedInternalStaticProperty }}", in context);
        writer.Dispose();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("_ProtectedInternalStaticProperty");
    }

    [Fact]
    public void ProtectedInternalプロパティ()
    {
        var context = new AccessibilityTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        Template.Render(ref writer, "{{ ProtectedInternalProperty }}", in context);
        writer.Dispose();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("_ProtectedInternalProperty");
    }
}
