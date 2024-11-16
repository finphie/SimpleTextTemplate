using System.Buffers;
using System.Text;
using FluentAssertions;
using SimpleTextTemplate.Generator.Tests.Core;
using Xunit;

namespace SimpleTextTemplate.Generator.Execute.Tests;

public sealed class TemplateWriterWriteNullTest
{
    [Fact]
    public void 定数()
    {
        var context = new NullContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        Template.Render(ref writer, "{{ NullStringConstantField }}", in context);
        Template.Render(ref writer, "{{ NullObjectConstantField }}", in context);
        writer.Dispose();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .BeEmpty();
    }

    [Fact]
    public void 静的フィールド()
    {
        var context = new NullContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        Template.Render(ref writer, "{{ NullStringStaticField }}", in context);
        Template.Render(ref writer, "{{ NullObjectStaticField }}", in context);
        writer.Dispose();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .BeEmpty();
    }

    [Fact]
    public void フィールド()
    {
        var context = new NullContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        Template.Render(ref writer, "{{ NullStringField }}", in context);
        Template.Render(ref writer, "{{ NullObjectField }}", in context);
        writer.Dispose();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .BeEmpty();
    }

    [Fact]
    public void 静的プロパティ()
    {
        var context = new NullContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        Template.Render(ref writer, "{{ NullStringStaticProperty }}", in context);
        Template.Render(ref writer, "{{ NullObjectStaticProperty }}", in context);
        writer.Dispose();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .BeEmpty();
    }

    [Fact]
    public void プロパティ()
    {
        var context = new NullContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        Template.Render(ref writer, "{{ NullStringProperty }}", in context);
        Template.Render(ref writer, "{{ NullObjectProperty }}", in context);
        writer.Dispose();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .BeEmpty();
    }

    [Fact]
    public void Empty()
    {
        var context = new NullContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        Template.Render(ref writer, "{{ EmptyStringConstantField }}", in context);
        Template.Render(ref writer, "{{ EmptyStringStaticField }}", in context);
        writer.Dispose();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .BeEmpty();
    }
}
