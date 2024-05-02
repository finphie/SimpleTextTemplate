using System.Buffers;
using System.Text;
using FluentAssertions;
using SimpleTextTemplate.Generator.Tests.Core;
using Xunit;

namespace SimpleTextTemplate.Generator.Execute.Tests;

public sealed class TemplateWriterWriteEnumTest
{
    [Fact]
    public void 静的フィールド()
    {
        var context = new EnumContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = TemplateWriter.Create(bufferWriter))
        {
            writer.Write(
                """
                {{ EnumStaticField }}
                {{ EnumStaticField:D }}
                """,
                in context);
        }

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("""
            A
            0
            """);
    }

    [Fact]
    public void フィールド()
    {
        var context = new EnumContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = TemplateWriter.Create(bufferWriter))
        {
            writer.Write(
                """
                {{ EnumField }}
                {{ EnumField:D }}
                """,
                in context);
        }

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("""
            A
            0
            """);
    }

    [Fact]
    public void 静的プロパティ()
    {
        var context = new EnumContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = TemplateWriter.Create(bufferWriter))
        {
            writer.Write(
                """
                {{ EnumStaticProperty }}
                {{ EnumStaticProperty:D }}
                """,
                in context);
        }

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("""
            A
            0
            """);
    }

    [Fact]
    public void プロパティ()
    {
        var context = new EnumContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = TemplateWriter.Create(bufferWriter))
        {
            writer.Write(
                """
                {{ EnumProperty }}
                {{ EnumProperty:D }}
                """,
                in context);
        }

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("""
            A
            0
            """);
    }
}
