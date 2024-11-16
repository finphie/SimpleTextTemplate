using System.Buffers;
using System.Text;
using FluentAssertions;
using SimpleTextTemplate.Generator.Tests.Core;
using Xunit;

namespace SimpleTextTemplate.Generator.Execute.Tests;

public sealed class TemplateWriterWriteStringTest
{
    [Fact]
    public void 定数()
    {
        var context = new StringContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        Template.Render(ref writer, "{{ StringConstantField }}", in context);
        writer.Dispose();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("_StringConstantField");
    }

    [Fact]
    public void 静的フィールド()
    {
        var context = new StringContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        Template.Render(ref writer, "{{ StringStaticField }}", in context);
        writer.Dispose();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("_StringStaticField");
    }

    [Fact]
    public void フィールド()
    {
        var context = new StringContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        Template.Render(ref writer, "{{ StringField }}", in context);
        writer.Dispose();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("_StringField");
    }

    [Fact]
    public void 静的プロパティ()
    {
        var context = new StringContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        Template.Render(ref writer, "{{ StringStaticProperty }}", in context);
        writer.Dispose();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("_StringStaticProperty");
    }

    [Fact]
    public void プロパティ()
    {
        var context = new StringContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        Template.Render(ref writer, "{{ StringProperty }}", in context);
        writer.Dispose();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("_StringProperty");
    }
}
