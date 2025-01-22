using System.Buffers;
using System.Globalization;
using System.Text;
using Shouldly;
using SimpleTextTemplate.Generator.Tests.Core;
using Xunit;

namespace SimpleTextTemplate.Generator.Execute.Tests;

public sealed class TemplateRendererRenderIntTest
{
    [Fact]
    public void 静的フィールド()
    {
        const string Text = """
            {{ IntStaticField }}
            {{ IntStaticField:N3 }}
            {{ IntStaticField:N3:es-ES }}
            {{ IntStaticField::ja-JP }}
            """;
        var context = new IntContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, Text, in context);
        writer.Flush();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .ShouldBe("""
            1234
            1,234.000
            1.234,000
            1234
            """);
    }

    [Fact]
    public void 静的フィールド_InvariantCultureを指定()
    {
        const string Text = """
            {{ IntStaticField }}
            {{ IntStaticField:N3 }}
            {{ IntStaticField:N3:es-ES }}
            {{ IntStaticField::ja-JP }}
            """;
        var context = new IntContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, Text, in context, CultureInfo.InvariantCulture);
        writer.Flush();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .ShouldBe("""
            1234
            1,234.000
            1.234,000
            1234
            """);
    }

    [Fact]
    public void 静的フィールド_特定カルチャーを指定()
    {
        const string Text = """
            {{ IntStaticField }}
            {{ IntStaticField:N3 }}
            {{ IntStaticField:N3:es-ES }}
            {{ IntStaticField::ja-JP }}
            """;
        var context = new IntContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, Text, in context, CultureInfo.GetCultureInfo("ja-JP", true));
        writer.Flush();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .ShouldBe("""
            1234
            1,234.000
            1.234,000
            1234
            """);
    }

    [Fact]
    public void フィールド()
    {
        const string Text = """
            {{ IntField }}
            {{ IntField:N3 }}
            {{ IntField:N3:es-ES }}
            {{ IntField::ja-JP }}
            """;
        var context = new IntContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, Text, in context);
        writer.Flush();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .ShouldBe("""
            1234
            1,234.000
            1.234,000
            1234
            """);
    }

    [Fact]
    public void フィールド_InvariantCultureを指定()
    {
        const string Text = """
            {{ IntField }}
            {{ IntField:N3 }}
            {{ IntField:N3:es-ES }}
            {{ IntField::ja-JP }}
            """;
        var context = new IntContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, Text, in context, CultureInfo.InvariantCulture);
        writer.Flush();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .ShouldBe("""
            1234
            1,234.000
            1.234,000
            1234
            """);
    }

    [Fact]
    public void フィールド_特定カルチャーを指定()
    {
        const string Text = """
            {{ IntField }}
            {{ IntField:N3 }}
            {{ IntField:N3:es-ES }}
            {{ IntField::ja-JP }}
            """;
        var context = new IntContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, Text, in context, CultureInfo.GetCultureInfo("ja-JP", true));
        writer.Flush();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .ShouldBe("""
            1234
            1,234.000
            1.234,000
            1234
            """);
    }

    [Fact]
    public void 静的プロパティ()
    {
        const string Text = """
            {{ IntStaticProperty }}
            {{ IntStaticProperty:N3 }}
            {{ IntStaticProperty:N3:es-ES }}
            {{ IntStaticProperty::ja-JP }}
            """;
        var context = new IntContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, Text, in context);
        writer.Flush();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .ShouldBe("""
            1234
            1,234.000
            1.234,000
            1234
            """);
    }

    [Fact]
    public void 静的プロパティ_InvariantCultureを指定()
    {
        const string Text = """
            {{ IntStaticProperty }}
            {{ IntStaticProperty:N3 }}
            {{ IntStaticProperty:N3:es-ES }}
            {{ IntStaticProperty::ja-JP }}
            """;
        var context = new IntContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, Text, in context, CultureInfo.InvariantCulture);
        writer.Flush();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .ShouldBe("""
            1234
            1,234.000
            1.234,000
            1234
            """);
    }

    [Fact]
    public void 静的プロパティ_特定カルチャーを指定()
    {
        const string Text = """
            {{ IntStaticProperty }}
            {{ IntStaticProperty:N3 }}
            {{ IntStaticProperty:N3:es-ES }}
            {{ IntStaticProperty::ja-JP }}
            """;
        var context = new IntContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, Text, in context, CultureInfo.GetCultureInfo("ja-JP", true));
        writer.Flush();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .ShouldBe("""
            1234
            1,234.000
            1.234,000
            1234
            """);
    }

    [Fact]
    public void プロパティ()
    {
        const string Text = """
            {{ IntProperty }}
            {{ IntProperty:N3 }}
            {{ IntProperty:N3:es-ES }}
            {{ IntProperty::ja-JP }}
            """;
        var context = new IntContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, Text, in context);
        writer.Flush();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .ShouldBe("""
            1234
            1,234.000
            1.234,000
            1234
            """);
    }

    [Fact]
    public void プロパティ_InvariantCultureを指定()
    {
        const string Text = """
            {{ IntProperty }}
            {{ IntProperty:N3 }}
            {{ IntProperty:N3:es-ES }}
            {{ IntProperty::ja-JP }}
            """;
        var context = new IntContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, Text, in context, CultureInfo.InvariantCulture);
        writer.Flush();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .ShouldBe("""
            1234
            1,234.000
            1.234,000
            1234
            """);
    }

    [Fact]
    public void プロパティ_特定カルチャーを指定()
    {
        const string Text = """
            {{ IntProperty }}
            {{ IntProperty:N3 }}
            {{ IntProperty:N3:es-ES }}
            {{ IntProperty::ja-JP }}
            """;
        var context = new IntContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, Text, in context, CultureInfo.GetCultureInfo("ja-JP", true));
        writer.Flush();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .ShouldBe("""
            1234
            1,234.000
            1.234,000
            1234
            """);
    }
}
