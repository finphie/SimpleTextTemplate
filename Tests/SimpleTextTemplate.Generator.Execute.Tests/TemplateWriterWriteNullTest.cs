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

        using (var writer = TemplateWriter.Create(bufferWriter))
        {
            writer.Write("{{ NullStringConstantField }}", in context);
            writer.Write("{{ NullObjectConstantField }}", in context);
        }

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .BeEmpty();
    }

    [Fact]
    public void 静的フィールド()
    {
        var context = new NullContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = TemplateWriter.Create(bufferWriter))
        {
            writer.Write("{{ NullStringStaticField }}", in context);
            writer.Write("{{ NullObjectStaticField }}", in context);
        }

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .BeEmpty();
    }

    [Fact]
    public void フィールド()
    {
        var context = new NullContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = TemplateWriter.Create(bufferWriter))
        {
            writer.Write("{{ NullStringField }}", in context);
            writer.Write("{{ NullObjectField }}", in context);
        }

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .BeEmpty();
    }

    [Fact]
    public void 静的プロパティ()
    {
        var context = new NullContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = TemplateWriter.Create(bufferWriter))
        {
            writer.Write("{{ NullStringStaticProperty }}", in context);
            writer.Write("{{ NullObjectStaticProperty }}", in context);
        }

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .BeEmpty();
    }

    [Fact]
    public void プロパティ()
    {
        var context = new NullContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = TemplateWriter.Create(bufferWriter))
        {
            writer.Write("{{ NullStringProperty }}", in context);
            writer.Write("{{ NullObjectProperty }}", in context);
        }

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .BeEmpty();
    }

    [Fact]
    public void Empty()
    {
        var context = new NullContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = TemplateWriter.Create(bufferWriter))
        {
            writer.Write("{{ EmptyStringConstantField }}", in context);
            writer.Write("{{ EmptyStringStaticField }}", in context);
        }

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .BeEmpty();
    }
}
