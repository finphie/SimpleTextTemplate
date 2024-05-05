using System.Buffers;
using System.Globalization;
using System.Text;
using FluentAssertions;
using SimpleTextTemplate.Generator.Tests.Core;
using Xunit;

namespace SimpleTextTemplate.Generator.Execute.Tests;

public sealed class TemplateWriterWriteIntTest
{
    [Fact]
    public void 静的フィールド()
    {
        var context = new IntContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = TemplateWriter.Create(bufferWriter))
        {
            writer.Write(
                """
                {{ IntStaticField }}
                {{ IntStaticField:N3 }}
                {{ IntStaticField:N3:es-ES }}
                {{ IntStaticField::ja-JP }}
                """,
                in context);
        }

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("""
            1234
            1,234.000
            1.234,000
            1234
            """);
    }

    [Fact]
    public void 静的フィールド_InvariantCultureを指定()
    {
        var context = new IntContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = TemplateWriter.Create(bufferWriter))
        {
            writer.Write(
                """
                {{ IntStaticField }}
                {{ IntStaticField:N3 }}
                {{ IntStaticField:N3:es-ES }}
                {{ IntStaticField::ja-JP }}
                """,
                in context,
                CultureInfo.InvariantCulture);
        }

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("""
            1234
            1,234.000
            1.234,000
            1234
            """);
    }

    [Fact]
    public void 静的フィールド_特定カルチャーを指定()
    {
        var context = new IntContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = TemplateWriter.Create(bufferWriter))
        {
            writer.Write(
                """
                {{ IntStaticField }}
                {{ IntStaticField:N3 }}
                {{ IntStaticField:N3:es-ES }}
                {{ IntStaticField::ja-JP }}
                """,
                in context,
                CultureInfo.GetCultureInfo("ja-JP", true));
        }

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("""
            1234
            1,234.000
            1.234,000
            1234
            """);
    }

    [Fact]
    public void フィールド()
    {
        var context = new IntContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = TemplateWriter.Create(bufferWriter))
        {
            writer.Write(
                """
                {{ IntField }}
                {{ IntField:N3 }}
                {{ IntField:N3:es-ES }}
                {{ IntField::ja-JP }}
                """,
                in context);
        }

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("""
            1234
            1,234.000
            1.234,000
            1234
            """);
    }

    [Fact]
    public void フィールド_InvariantCultureを指定()
    {
        var context = new IntContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = TemplateWriter.Create(bufferWriter))
        {
            writer.Write(
                """
                {{ IntField }}
                {{ IntField:N3 }}
                {{ IntField:N3:es-ES }}
                {{ IntField::ja-JP }}
                """,
                in context,
                CultureInfo.InvariantCulture);
        }

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("""
            1234
            1,234.000
            1.234,000
            1234
            """);
    }

    [Fact]
    public void フィールド_特定カルチャーを指定()
    {
        var context = new IntContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = TemplateWriter.Create(bufferWriter))
        {
            writer.Write(
                """
                {{ IntField }}
                {{ IntField:N3 }}
                {{ IntField:N3:es-ES }}
                {{ IntField::ja-JP }}
                """,
                in context,
                CultureInfo.GetCultureInfo("ja-JP", true));
        }

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("""
            1234
            1,234.000
            1.234,000
            1234
            """);
    }

    [Fact]
    public void 静的プロパティ()
    {
        var context = new IntContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = TemplateWriter.Create(bufferWriter))
        {
            writer.Write(
                """
                {{ IntStaticProperty }}
                {{ IntStaticProperty:N3 }}
                {{ IntStaticProperty:N3:es-ES }}
                {{ IntStaticProperty::ja-JP }}
                """,
                in context);
        }

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("""
            1234
            1,234.000
            1.234,000
            1234
            """);
    }

    [Fact]
    public void 静的プロパティ_InvariantCultureを指定()
    {
        var context = new IntContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = TemplateWriter.Create(bufferWriter))
        {
            writer.Write(
                """
                {{ IntStaticProperty }}
                {{ IntStaticProperty:N3 }}
                {{ IntStaticProperty:N3:es-ES }}
                {{ IntStaticProperty::ja-JP }}
                """,
                in context,
                CultureInfo.InvariantCulture);
        }

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("""
            1234
            1,234.000
            1.234,000
            1234
            """);
    }

    [Fact]
    public void 静的プロパティ_特定カルチャーを指定()
    {
        var context = new IntContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = TemplateWriter.Create(bufferWriter))
        {
            writer.Write(
                """
                {{ IntStaticProperty }}
                {{ IntStaticProperty:N3 }}
                {{ IntStaticProperty:N3:es-ES }}
                {{ IntStaticProperty::ja-JP }}
                """,
                in context,
                CultureInfo.GetCultureInfo("ja-JP", true));
        }

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("""
            1234
            1,234.000
            1.234,000
            1234
            """);
    }

    [Fact]
    public void プロパティ()
    {
        var context = new IntContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = TemplateWriter.Create(bufferWriter))
        {
            writer.Write(
                """
                {{ IntProperty }}
                {{ IntProperty:N3 }}
                {{ IntProperty:N3:es-ES }}
                {{ IntProperty::ja-JP }}
                """,
                in context);
        }

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("""
            1234
            1,234.000
            1.234,000
            1234
            """);
    }

    [Fact]
    public void プロパティ_InvariantCultureを指定()
    {
        var context = new IntContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = TemplateWriter.Create(bufferWriter))
        {
            writer.Write(
                """
                {{ IntProperty }}
                {{ IntProperty:N3 }}
                {{ IntProperty:N3:es-ES }}
                {{ IntProperty::ja-JP }}
                """,
                in context,
                CultureInfo.InvariantCulture);
        }

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("""
            1234
            1,234.000
            1.234,000
            1234
            """);
    }

    [Fact]
    public void プロパティ_特定カルチャーを指定()
    {
        var context = new IntContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = TemplateWriter.Create(bufferWriter))
        {
            writer.Write(
                """
                {{ IntProperty }}
                {{ IntProperty:N3 }}
                {{ IntProperty:N3:es-ES }}
                {{ IntProperty::ja-JP }}
                """,
                in context,
                CultureInfo.GetCultureInfo("ja-JP", true));
        }

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("""
            1234
            1,234.000
            1.234,000
            1234
            """);
    }
}
