using System.Buffers;
using System.Text;
using FluentAssertions;
using SimpleTextTemplate.Generator.Tests.Core;
using Xunit;

namespace SimpleTextTemplate.Generator.Execute.Tests;

public sealed class TemplateWriterWriteCharArrayTest
{
    [Fact]
    public void 静的フィールド()
    {
        var context = new CharArrayContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        Template.Render(ref writer, "{{ CharsStaticField }}", in context);
        writer.Dispose();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("_CharsStaticField");
    }

    [Fact]
    public void フィールド()
    {
        var context = new CharArrayContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        Template.Render(ref writer, "{{ CharsField }}", in context);
        writer.Dispose();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("_CharsField");
    }

    [Fact]
    public void 静的プロパティ()
    {
        var context = new CharArrayContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        Template.Render(ref writer, "{{ CharsStaticProperty }}", in context);
        writer.Dispose();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("_CharsStaticProperty");
    }

    [Fact]
    public void プロパティ()
    {
        var context = new CharArrayContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        Template.Render(ref writer, "{{ CharsProperty }}", in context);
        writer.Dispose();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("_CharsProperty");
    }

    [Fact]
    public void 静的ReadOnlySpanプロパティ()
    {
        var context = new CharArrayContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        Template.Render(ref writer, "{{ CharsSpanStaticProperty }}", in context);
        writer.Dispose();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("_CharsSpanStaticProperty");
    }

    [Fact]
    public void ReadOnlySpanプロパティ()
    {
        var context = new CharArrayContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        Template.Render(ref writer, "{{ CharsSpanProperty }}", in context);
        writer.Dispose();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("_CharsSpanProperty");
    }
}
