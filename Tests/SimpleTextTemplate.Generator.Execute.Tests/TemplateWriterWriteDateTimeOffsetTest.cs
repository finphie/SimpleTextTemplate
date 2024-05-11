using System.Buffers;
using System.Globalization;
using System.Text;
using FluentAssertions;
using SimpleTextTemplate.Generator.Tests.Core;
using Xunit;

namespace SimpleTextTemplate.Generator.Execute.Tests;

public sealed class TemplateWriterWriteDateTimeOffsetTest
{
    [Fact]
    public void 静的フィールド()
    {
        var context = new DateTimeOffsetContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = TemplateWriter.Create(bufferWriter))
        {
            writer.Write(
                """
                {{ DateTimeOffsetStaticField }}
                {{ DateTimeOffsetStaticField:o }}
                {{ DateTimeOffsetStaticField:D:ja-JP }}
                {{ DateTimeOffsetStaticField::ja-JP }}
                """,
                in context);
        }

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("""
            01/01/2000 00:00:00 +09:00
            2000-01-01T00:00:00.0000000+09:00
            2000年1月1日土曜日
            2000/01/01 0:00:00 +09:00
            """);
    }

    [Fact]
    public void 静的フィールド_InvariantCultureを指定()
    {
        var context = new DateTimeOffsetContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = TemplateWriter.Create(bufferWriter))
        {
            writer.Write(
                """
                {{ DateTimeOffsetStaticField }}
                {{ DateTimeOffsetStaticField:o }}
                {{ DateTimeOffsetStaticField:D:ja-JP }}
                {{ DateTimeOffsetStaticField::ja-JP }}
                """,
                in context,
                CultureInfo.InvariantCulture);
        }

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("""
            01/01/2000 00:00:00 +09:00
            2000-01-01T00:00:00.0000000+09:00
            2000年1月1日土曜日
            2000/01/01 0:00:00 +09:00
            """);
    }

    [Fact]
    public void 静的フィールド_特定カルチャーを指定()
    {
        var context = new DateTimeOffsetContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = TemplateWriter.Create(bufferWriter))
        {
            writer.Write(
                """
                {{ DateTimeOffsetStaticField }}
                {{ DateTimeOffsetStaticField:o }}
                {{ DateTimeOffsetStaticField:D:ja-JP }}
                {{ DateTimeOffsetStaticField::ja-JP }}
                """,
                in context,
                CultureInfo.GetCultureInfo("ja-JP", true));
        }

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("""
            2000/01/01 0:00:00 +09:00
            2000-01-01T00:00:00.0000000+09:00
            2000年1月1日土曜日
            2000/01/01 0:00:00 +09:00
            """);
    }

    [Fact]
    public void フィールド()
    {
        var context = new DateTimeOffsetContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = TemplateWriter.Create(bufferWriter))
        {
            writer.Write(
                """
                {{ DateTimeOffsetField }}
                {{ DateTimeOffsetField:o }}
                {{ DateTimeOffsetField:D:ja-JP }}
                {{ DateTimeOffsetField::ja-JP }}
                """,
                in context);
        }

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("""
            01/01/2000 00:00:00 +09:00
            2000-01-01T00:00:00.0000000+09:00
            2000年1月1日土曜日
            2000/01/01 0:00:00 +09:00
            """);
    }

    [Fact]
    public void フィールド_InvariantCultureを指定()
    {
        var context = new DateTimeOffsetContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = TemplateWriter.Create(bufferWriter))
        {
            writer.Write(
                """
                {{ DateTimeOffsetField }}
                {{ DateTimeOffsetField:o }}
                {{ DateTimeOffsetField:D:ja-JP }}
                {{ DateTimeOffsetField::ja-JP }}
                """,
                in context,
                CultureInfo.InvariantCulture);
        }

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("""
            01/01/2000 00:00:00 +09:00
            2000-01-01T00:00:00.0000000+09:00
            2000年1月1日土曜日
            2000/01/01 0:00:00 +09:00
            """);
    }

    [Fact]
    public void フィールド_特定カルチャーを指定()
    {
        var context = new DateTimeOffsetContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = TemplateWriter.Create(bufferWriter))
        {
            writer.Write(
                """
                {{ DateTimeOffsetField }}
                {{ DateTimeOffsetField:o }}
                {{ DateTimeOffsetField:D:ja-JP }}
                {{ DateTimeOffsetField::ja-JP }}
                """,
                in context,
                CultureInfo.GetCultureInfo("ja-JP", true));
        }

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("""
            2000/01/01 0:00:00 +09:00
            2000-01-01T00:00:00.0000000+09:00
            2000年1月1日土曜日
            2000/01/01 0:00:00 +09:00
            """);
    }

    [Fact]
    public void 静的プロパティ()
    {
        var context = new DateTimeOffsetContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = TemplateWriter.Create(bufferWriter))
        {
            writer.Write(
                """
                {{ DateTimeOffsetStaticProperty }}
                {{ DateTimeOffsetStaticProperty:o }}
                {{ DateTimeOffsetStaticProperty:D:ja-JP }}
                {{ DateTimeOffsetStaticProperty::ja-JP }}
                """,
                in context);
        }

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("""
            01/01/2000 00:00:00 +09:00
            2000-01-01T00:00:00.0000000+09:00
            2000年1月1日土曜日
            2000/01/01 0:00:00 +09:00
            """);
    }

    [Fact]
    public void 静的プロパティ_InvariantCultureを指定()
    {
        var context = new DateTimeOffsetContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = TemplateWriter.Create(bufferWriter))
        {
            writer.Write(
                """
                {{ DateTimeOffsetStaticProperty }}
                {{ DateTimeOffsetStaticProperty:o }}
                {{ DateTimeOffsetStaticProperty:D:ja-JP }}
                {{ DateTimeOffsetStaticProperty::ja-JP }}
                """,
                in context,
                CultureInfo.InvariantCulture);
        }

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("""
            01/01/2000 00:00:00 +09:00
            2000-01-01T00:00:00.0000000+09:00
            2000年1月1日土曜日
            2000/01/01 0:00:00 +09:00
            """);
    }

    [Fact]
    public void 静的プロパティ_特定カルチャーを指定()
    {
        var context = new DateTimeOffsetContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = TemplateWriter.Create(bufferWriter))
        {
            writer.Write(
                """
                {{ DateTimeOffsetStaticProperty }}
                {{ DateTimeOffsetStaticProperty:o }}
                {{ DateTimeOffsetStaticProperty:D:ja-JP }}
                {{ DateTimeOffsetStaticProperty::ja-JP }}
                """,
                in context,
                CultureInfo.GetCultureInfo("ja-JP", true));
        }

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("""
            2000/01/01 0:00:00 +09:00
            2000-01-01T00:00:00.0000000+09:00
            2000年1月1日土曜日
            2000/01/01 0:00:00 +09:00
            """);
    }

    [Fact]
    public void プロパティ()
    {
        var context = new DateTimeOffsetContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = TemplateWriter.Create(bufferWriter))
        {
            writer.Write(
                """
                {{ DateTimeOffsetProperty }}
                {{ DateTimeOffsetProperty:o }}
                {{ DateTimeOffsetProperty:D:ja-JP }}
                {{ DateTimeOffsetProperty::ja-JP }}
                """,
                in context);
        }

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("""
            01/01/2000 00:00:00 +09:00
            2000-01-01T00:00:00.0000000+09:00
            2000年1月1日土曜日
            2000/01/01 0:00:00 +09:00
            """);
    }

    [Fact]
    public void プロパティ_InvariantCultureを指定()
    {
        var context = new DateTimeOffsetContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = TemplateWriter.Create(bufferWriter))
        {
            writer.Write(
                """
                {{ DateTimeOffsetProperty }}
                {{ DateTimeOffsetProperty:o }}
                {{ DateTimeOffsetProperty:D:ja-JP }}
                {{ DateTimeOffsetProperty::ja-JP }}
                """,
                in context,
                CultureInfo.InvariantCulture);
        }

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("""
            01/01/2000 00:00:00 +09:00
            2000-01-01T00:00:00.0000000+09:00
            2000年1月1日土曜日
            2000/01/01 0:00:00 +09:00
            """);
    }

    [Fact]
    public void プロパティ_特定カルチャーを指定()
    {
        var context = new DateTimeOffsetContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = TemplateWriter.Create(bufferWriter))
        {
            writer.Write(
                """
                {{ DateTimeOffsetProperty }}
                {{ DateTimeOffsetProperty:o }}
                {{ DateTimeOffsetProperty:D:ja-JP }}
                {{ DateTimeOffsetProperty::ja-JP }}
                """,
                in context,
                CultureInfo.GetCultureInfo("ja-JP", true));
        }

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("""
            2000/01/01 0:00:00 +09:00
            2000-01-01T00:00:00.0000000+09:00
            2000年1月1日土曜日
            2000/01/01 0:00:00 +09:00
            """);
    }
}
