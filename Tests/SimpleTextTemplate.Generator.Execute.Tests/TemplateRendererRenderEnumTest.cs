using System.Buffers;
using System.Text;
using FluentAssertions;
using SimpleTextTemplate.Generator.Tests.Core;
using Xunit;

namespace SimpleTextTemplate.Generator.Execute.Tests;

public sealed class TemplateRendererRenderEnumTest
{
    [Fact]
    public void 静的フィールド()
    {
        const string Text = """
            {{ EnumStaticField }}
            {{ EnumStaticField:D }}
            """;
        var context = new EnumContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, Text, in context);
        writer.Dispose();

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
        const string Text = """
            {{ EnumField }}
            {{ EnumField:D }}
            """;
        var context = new EnumContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, Text, in context);
        writer.Dispose();

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
        const string Text = """
            {{ EnumStaticProperty }}
            {{ EnumStaticProperty:D }}
            """;
        var context = new EnumContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, Text, in context);
        writer.Dispose();

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
        const string Text = """
            {{ EnumProperty }}
            {{ EnumProperty:D }}
            """;
        var context = new EnumContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, Text, in context);
        writer.Dispose();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("""
            A
            0
            """);
    }
}
