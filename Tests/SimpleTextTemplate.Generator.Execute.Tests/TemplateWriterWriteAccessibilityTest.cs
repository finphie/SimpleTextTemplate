using System.Buffers;
using System.Text;
using FluentAssertions;
using Xunit;

namespace SimpleTextTemplate.Generator.Execute.Tests;

public sealed class TemplateWriterWriteAccessibilityTest
{
    [Fact]
    public void 静的Internalフィールド()
    {
        var context = new AccessibilityTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = TemplateWriter.Create(bufferWriter))
        {
            writer.Write("{{ _internalStaticField }}", in context);
        }

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("InternalStaticField");
    }

    [Fact]
    public void Internalフィールド()
    {
        var context = new AccessibilityTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = TemplateWriter.Create(bufferWriter))
        {
            writer.Write("{{ _internalField }}", in context);
        }

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("InternalField");
    }

    [Fact]
    public void 静的ProtectedInternalフィールド()
    {
        var context = new AccessibilityTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = TemplateWriter.Create(bufferWriter))
        {
            writer.Write("{{ ProtectedInternalStaticField }}", in context);
        }

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("_ProtectedInternalStaticField");
    }

    [Fact]
    public void ProtectedInternalフィールド()
    {
        var context = new AccessibilityTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = TemplateWriter.Create(bufferWriter))
        {
            writer.Write("{{ ProtectedInternalField }}", in context);
        }

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("_ProtectedInternalField");
    }

    [Fact]
    public void 静的Internalプロパティ()
    {
        var context = new AccessibilityTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = TemplateWriter.Create(bufferWriter))
        {
            writer.Write("{{ InternalStaticProperty }}", in context);
        }

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("_InternalStaticProperty");
    }

    [Fact]
    public void Internalプロパティ()
    {
        var context = new AccessibilityTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = TemplateWriter.Create(bufferWriter))
        {
            writer.Write("{{ InternalProperty }}", in context);
        }

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("_InternalProperty");
    }

    [Fact]
    public void 静的ProtectedInternalプロパティ()
    {
        var context = new AccessibilityTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = TemplateWriter.Create(bufferWriter))
        {
            writer.Write("{{ ProtectedInternalStaticProperty }}", in context);
        }

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("_ProtectedInternalStaticProperty");
    }

    [Fact]
    public void ProtectedInternalプロパティ()
    {
        var context = new AccessibilityTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = TemplateWriter.Create(bufferWriter))
        {
            writer.Write("{{ ProtectedInternalProperty }}", in context);
        }

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("_ProtectedInternalProperty");
    }
}
