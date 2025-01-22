using System.Buffers;
using System.Text;
using Shouldly;
using SimpleTextTemplate.Generator.Tests.Core;
using Xunit;

namespace SimpleTextTemplate.Generator.Execute.Tests;

public sealed class TemplateRendererRenderNullTest
{
    [Fact]
    public void 定数()
    {
        var context = new NullContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, "{{ NullStringConstantField }}", in context);
        TemplateRenderer.Render(ref writer, "{{ NullObjectConstantField }}", in context);
        writer.Flush();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .ShouldBeEmpty();
    }

    [Fact]
    public void 静的フィールド()
    {
        var context = new NullContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, "{{ NullStringStaticField }}", in context);
        TemplateRenderer.Render(ref writer, "{{ NullObjectStaticField }}", in context);
        writer.Flush();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .ShouldBeEmpty();
    }

    [Fact]
    public void フィールド()
    {
        var context = new NullContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, "{{ NullStringField }}", in context);
        TemplateRenderer.Render(ref writer, "{{ NullObjectField }}", in context);
        writer.Flush();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .ShouldBeEmpty();
    }

    [Fact]
    public void 静的プロパティ()
    {
        var context = new NullContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, "{{ NullStringStaticProperty }}", in context);
        TemplateRenderer.Render(ref writer, "{{ NullObjectStaticProperty }}", in context);
        writer.Flush();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .ShouldBeEmpty();
    }

    [Fact]
    public void プロパティ()
    {
        var context = new NullContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, "{{ NullStringProperty }}", in context);
        TemplateRenderer.Render(ref writer, "{{ NullObjectProperty }}", in context);
        writer.Flush();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .ShouldBeEmpty();
    }

    [Fact]
    public void Empty()
    {
        var context = new NullContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, "{{ EmptyStringConstantField }}", in context);
        TemplateRenderer.Render(ref writer, "{{ EmptyStringStaticField }}", in context);
        writer.Flush();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .ShouldBeEmpty();
    }
}
