using System.Buffers;
using System.Text;
using FluentAssertions;
using SimpleTextTemplate.Generator.Tests.Core;
using Xunit;
using static SimpleTextTemplate.TemplateRenderer;

namespace SimpleTextTemplate.Generator.Execute.Tests;

public sealed class TemplateRendererRenderUsingStaticTest
{
    [Fact]
    public void 静的フィールド()
    {
        var context = new ByteArrayContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        Render(ref writer, "{{ BytesStaticField }}", in context);
        writer.Flush();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("_BytesStaticField");
    }

    [Fact]
    public void フィールド()
    {
        var context = new ByteArrayContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        Render(ref writer, "{{ BytesField }}", in context);
        writer.Flush();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("_BytesField");
    }

    [Fact]
    public void 静的プロパティ()
    {
        var context = new ByteArrayContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        Render(ref writer, "{{ BytesStaticProperty }}", in context);
        writer.Flush();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("_BytesStaticProperty");
    }

    [Fact]
    public void プロパティ()
    {
        var context = new ByteArrayContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        Render(ref writer, "{{ BytesProperty }}", in context);
        writer.Flush();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("_BytesProperty");
    }
}
