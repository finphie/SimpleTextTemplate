using System.Buffers;
using System.Text;
using Shouldly;
using SimpleTextTemplate.Generator.Tests.Core;
using Xunit;

namespace SimpleTextTemplate.Generator.Execute.Tests;

public sealed class TemplateRendererRenderEnumTest
{
    [Fact]
    public void 定数()
    {
        const string Text = """
            {{ EnumConstantField }}
            {{ EnumConstantField:D }}
            {{ EnumConstantField:X }}
            """;
        var context = new EnumContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, Text, in context);
        writer.Flush();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .ShouldBe("""
            Test1
            1
            00000001
            """);
    }

    [Fact]
    public void 定数が無効な値()
    {
        const string Text = """
            {{ EnumConstantFieldInvalidNumber }}
            {{ EnumConstantFieldInvalidNumber:D }}
            """;
        var context = new EnumContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, Text, in context);
        writer.Flush();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .ShouldBe("""
            99
            99
            """);
    }


    [Fact]
    public void Flags属性を付与したEnumの定数()
    {
        const string Text = """
            {{ FlagEnumConstantField }}
            {{ FlagEnumConstantField:D }}
            """;
        var context = new EnumContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, Text, in context);
        writer.Flush();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .ShouldBe("""
            Test1, Test2
            3
            """);
    }

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
        writer.Flush();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .ShouldBe("""
            Test2
            2
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
        writer.Flush();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .ShouldBe("""
            Test3
            3
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
        writer.Flush();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .ShouldBe("""
            Test4
            4
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
        writer.Flush();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .ShouldBe("""
            Test5
            5
            """);
    }
}
