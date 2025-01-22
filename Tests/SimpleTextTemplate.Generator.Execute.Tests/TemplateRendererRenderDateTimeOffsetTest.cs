using System.Buffers;
using System.Globalization;
using System.Text;
using Shouldly;
using SimpleTextTemplate.Generator.Tests.Core;
using Xunit;

namespace SimpleTextTemplate.Generator.Execute.Tests;

public sealed class TemplateRendererRenderDateTimeOffsetTest
{
    [Fact]
    public void 静的フィールド()
    {
        const string Text = """
            {{ DateTimeOffsetStaticField }}
            {{ DateTimeOffsetStaticField:o }}
            {{ DateTimeOffsetStaticField:D:ja-JP }}
            {{ DateTimeOffsetStaticField::ja-JP }}
            """;

        var context = new DateTimeOffsetContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, Text, in context);
        writer.Flush();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .ShouldBe("""
            01/01/2000 00:00:00 +09:00
            2000-01-01T00:00:00.0000000+09:00
            2000年1月1日土曜日
            2000/01/01 0:00:00 +09:00
            """);
    }

    [Fact]
    public void 静的フィールド_InvariantCultureを指定()
    {
        const string Text = """
            {{ DateTimeOffsetStaticField }}
            {{ DateTimeOffsetStaticField:o }}
            {{ DateTimeOffsetStaticField:D:ja-JP }}
            {{ DateTimeOffsetStaticField::ja-JP }}
            """;

        var context = new DateTimeOffsetContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, Text, in context, CultureInfo.InvariantCulture);
        writer.Flush();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .ShouldBe("""
            01/01/2000 00:00:00 +09:00
            2000-01-01T00:00:00.0000000+09:00
            2000年1月1日土曜日
            2000/01/01 0:00:00 +09:00
            """);
    }

    [Fact]
    public void 静的フィールド_特定カルチャーを指定()
    {
        const string Text = """
            {{ DateTimeOffsetStaticField }}
            {{ DateTimeOffsetStaticField:o }}
            {{ DateTimeOffsetStaticField:D:ja-JP }}
            {{ DateTimeOffsetStaticField::ja-JP }}
            """;

        var context = new DateTimeOffsetContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, Text, in context, CultureInfo.GetCultureInfo("ja-JP", true));
        writer.Flush();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .ShouldBe("""
            2000/01/01 0:00:00 +09:00
            2000-01-01T00:00:00.0000000+09:00
            2000年1月1日土曜日
            2000/01/01 0:00:00 +09:00
            """);
    }

    [Fact]
    public void フィールド()
    {
        const string Text = """
            {{ DateTimeOffsetField }}
            {{ DateTimeOffsetField:o }}
            {{ DateTimeOffsetField:D:ja-JP }}
            {{ DateTimeOffsetField::ja-JP }}
            """;

        var context = new DateTimeOffsetContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, Text, in context);
        writer.Flush();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .ShouldBe("""
            01/01/2000 00:00:00 +09:00
            2000-01-01T00:00:00.0000000+09:00
            2000年1月1日土曜日
            2000/01/01 0:00:00 +09:00
            """);
    }

    [Fact]
    public void フィールド_InvariantCultureを指定()
    {
        const string Text = """
            {{ DateTimeOffsetField }}
            {{ DateTimeOffsetField:o }}
            {{ DateTimeOffsetField:D:ja-JP }}
            {{ DateTimeOffsetField::ja-JP }}
            """;
        var context = new DateTimeOffsetContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, Text, in context, CultureInfo.InvariantCulture);
        writer.Flush();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .ShouldBe("""
            01/01/2000 00:00:00 +09:00
            2000-01-01T00:00:00.0000000+09:00
            2000年1月1日土曜日
            2000/01/01 0:00:00 +09:00
            """);
    }

    [Fact]
    public void フィールド_特定カルチャーを指定()
    {
        const string Text = """
            {{ DateTimeOffsetField }}
            {{ DateTimeOffsetField:o }}
            {{ DateTimeOffsetField:D:ja-JP }}
            {{ DateTimeOffsetField::ja-JP }}
            """;
        var context = new DateTimeOffsetContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, Text, in context, CultureInfo.GetCultureInfo("ja-JP", true));
        writer.Flush();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .ShouldBe("""
            2000/01/01 0:00:00 +09:00
            2000-01-01T00:00:00.0000000+09:00
            2000年1月1日土曜日
            2000/01/01 0:00:00 +09:00
            """);
    }

    [Fact]
    public void 静的プロパティ()
    {
        const string Text = """
            {{ DateTimeOffsetStaticProperty }}
            {{ DateTimeOffsetStaticProperty:o }}
            {{ DateTimeOffsetStaticProperty:D:ja-JP }}
            {{ DateTimeOffsetStaticProperty::ja-JP }}
            """;
        var context = new DateTimeOffsetContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, Text, in context);
        writer.Flush();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .ShouldBe("""
            01/01/2000 00:00:00 +09:00
            2000-01-01T00:00:00.0000000+09:00
            2000年1月1日土曜日
            2000/01/01 0:00:00 +09:00
            """);
    }

    [Fact]
    public void 静的プロパティ_InvariantCultureを指定()
    {
        const string Text = """
            {{ DateTimeOffsetStaticProperty }}
            {{ DateTimeOffsetStaticProperty:o }}
            {{ DateTimeOffsetStaticProperty:D:ja-JP }}
            {{ DateTimeOffsetStaticProperty::ja-JP }}
            """;
        var context = new DateTimeOffsetContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, Text, in context, CultureInfo.InvariantCulture);
        writer.Flush();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .ShouldBe("""
            01/01/2000 00:00:00 +09:00
            2000-01-01T00:00:00.0000000+09:00
            2000年1月1日土曜日
            2000/01/01 0:00:00 +09:00
            """);
    }

    [Fact]
    public void 静的プロパティ_特定カルチャーを指定()
    {
        const string Text = """
            {{ DateTimeOffsetStaticProperty }}
            {{ DateTimeOffsetStaticProperty:o }}
            {{ DateTimeOffsetStaticProperty:D:ja-JP }}
            {{ DateTimeOffsetStaticProperty::ja-JP }}
            """;
        var context = new DateTimeOffsetContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, Text, in context, CultureInfo.GetCultureInfo("ja-JP", true));
        writer.Flush();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .ShouldBe("""
            2000/01/01 0:00:00 +09:00
            2000-01-01T00:00:00.0000000+09:00
            2000年1月1日土曜日
            2000/01/01 0:00:00 +09:00
            """);
    }

    [Fact]
    public void プロパティ()
    {
        const string Text = """
            {{ DateTimeOffsetProperty }}
            {{ DateTimeOffsetProperty:o }}
            {{ DateTimeOffsetProperty:D:ja-JP }}
            {{ DateTimeOffsetProperty::ja-JP }}
            """;
        var context = new DateTimeOffsetContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, Text, in context);
        writer.Flush();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .ShouldBe("""
            01/01/2000 00:00:00 +09:00
            2000-01-01T00:00:00.0000000+09:00
            2000年1月1日土曜日
            2000/01/01 0:00:00 +09:00
            """);
    }

    [Fact]
    public void プロパティ_InvariantCultureを指定()
    {
        const string Text = """
            {{ DateTimeOffsetProperty }}
            {{ DateTimeOffsetProperty:o }}
            {{ DateTimeOffsetProperty:D:ja-JP }}
            {{ DateTimeOffsetProperty::ja-JP }}
            """;
        var context = new DateTimeOffsetContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, Text, in context, CultureInfo.InvariantCulture);
        writer.Flush();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .ShouldBe("""
            01/01/2000 00:00:00 +09:00
            2000-01-01T00:00:00.0000000+09:00
            2000年1月1日土曜日
            2000/01/01 0:00:00 +09:00
            """);
    }

    [Fact]
    public void プロパティ_特定カルチャーを指定()
    {
        const string Text = """
            {{ DateTimeOffsetProperty }}
            {{ DateTimeOffsetProperty:o }}
            {{ DateTimeOffsetProperty:D:ja-JP }}
            {{ DateTimeOffsetProperty::ja-JP }}
            """;
        var context = new DateTimeOffsetContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, Text, in context, CultureInfo.GetCultureInfo("ja-JP", true));
        writer.Flush();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .ShouldBe("""
            2000/01/01 0:00:00 +09:00
            2000-01-01T00:00:00.0000000+09:00
            2000年1月1日土曜日
            2000/01/01 0:00:00 +09:00
            """);
    }
}
