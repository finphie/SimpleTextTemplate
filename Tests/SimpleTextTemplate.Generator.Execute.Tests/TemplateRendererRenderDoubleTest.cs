using System.Buffers;
using System.Globalization;
using SimpleTextTemplate.Tests.Assertions;
using SimpleTextTemplate.Tests.TestData;

namespace SimpleTextTemplate.Generator.Execute.Tests;

public sealed class TemplateRendererRenderDoubleTest
{
    [Test]
    public async Task 定数()
    {
        const string Text = """
            {{ DoubleConstantField }}
            {{ DoubleConstantField:N3 }}
            {{ DoubleConstantField::es-ES }}
            {{ DoubleConstantField:N3:es-ES }}
            """;
        var context = new DoubleContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, Text, in context);
        writer.Flush();

        await Assert.That(bufferWriter.WrittenMemory)
            .IsUtf8SequenceEqualTo("""
                1234.567
                1,234.567
                1234,567
                1.234,567
                """);
    }

    [Test]
    public async Task 定数_InvariantCulture指定()
    {
        const string Text = """
            {{ DoubleConstantField }}
            {{ DoubleConstantField:N3 }}
            {{ DoubleConstantField::es-ES }}
            {{ DoubleConstantField:N3:es-ES }}
            """;
        var context = new DoubleContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, Text, in context, CultureInfo.InvariantCulture);
        writer.Flush();

        await Assert.That(bufferWriter.WrittenMemory)
            .IsUtf8SequenceEqualTo("""
                1234.567
                1,234.567
                1234,567
                1.234,567
                """);
    }

    [Test]
    public async Task 定数_特定カルチャー指定()
    {
        const string Text = """
            {{ DoubleConstantField }}
            {{ DoubleConstantField:N3 }}
            {{ DoubleConstantField::ja-JP }}
            {{ DoubleConstantField:N3:ja-JP }}
            """;
        var context = new DoubleContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, Text, in context, CultureInfo.GetCultureInfo("es-ES", true));
        writer.Flush();

        await Assert.That(bufferWriter.WrittenMemory)
            .IsUtf8SequenceEqualTo("""
                1234,567
                1.234,567
                1234.567
                1,234.567
                """);
    }

    [Test]
    public async Task 静的フィールド()
    {
        const string Text = """
            {{ DoubleStaticField }}
            {{ DoubleStaticField:N3 }}
            {{ DoubleStaticField::es-ES }}
            {{ DoubleStaticField:N3:es-ES }}
            """;
        var context = new DoubleContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, Text, in context);
        writer.Flush();

        await Assert.That(bufferWriter.WrittenMemory)
            .IsUtf8SequenceEqualTo("""
                2345.678
                2,345.678
                2345,678
                2.345,678
                """);
    }

    [Test]
    public async Task 静的フィールド_InvariantCulture指定()
    {
        const string Text = """
            {{ DoubleStaticField }}
            {{ DoubleStaticField:N3 }}
            {{ DoubleStaticField::es-ES }}
            {{ DoubleStaticField:N3:es-ES }}
            """;
        var context = new DoubleContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, Text, in context, CultureInfo.InvariantCulture);
        writer.Flush();

        await Assert.That(bufferWriter.WrittenMemory)
            .IsUtf8SequenceEqualTo("""
                2345.678
                2,345.678
                2345,678
                2.345,678
                """);
    }

    [Test]
    public async Task 静的フィールド_特定カルチャー指定()
    {
        const string Text = """
            {{ DoubleStaticField }}
            {{ DoubleStaticField:N3 }}
            {{ DoubleStaticField::ja-JP }}
            {{ DoubleStaticField:N3:ja-JP }}
            """;
        var context = new DoubleContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, Text, in context, CultureInfo.GetCultureInfo("es-ES", true));
        writer.Flush();

        await Assert.That(bufferWriter.WrittenMemory)
            .IsUtf8SequenceEqualTo("""
                2345,678
                2.345,678
                2345.678
                2,345.678
                """);
    }

    [Test]
    public async Task フィールド()
    {
        const string Text = """
            {{ DoubleField }}
            {{ DoubleField:N3 }}
            {{ DoubleField::es-ES }}
            {{ DoubleField:N3:es-ES }}
            """;
        var context = new DoubleContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, Text, in context);
        writer.Flush();

        await Assert.That(bufferWriter.WrittenMemory)
            .IsUtf8SequenceEqualTo("""
                3456.789
                3,456.789
                3456,789
                3.456,789
                """);
    }

    [Test]
    public async Task フィールド_InvariantCulture指定()
    {
        const string Text = """
            {{ DoubleField }}
            {{ DoubleField:N3 }}
            {{ DoubleField::es-ES }}
            {{ DoubleField:N3:es-ES }}
            """;
        var context = new DoubleContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, Text, in context, CultureInfo.InvariantCulture);
        writer.Flush();

        await Assert.That(bufferWriter.WrittenMemory)
            .IsUtf8SequenceEqualTo("""
                3456.789
                3,456.789
                3456,789
                3.456,789
                """);
    }

    [Test]
    public async Task フィールド_特定カルチャー指定()
    {
        const string Text = """
            {{ DoubleField }}
            {{ DoubleField:N3 }}
            {{ DoubleField::ja-JP }}
            {{ DoubleField:N3:ja-JP }}
            """;
        var context = new DoubleContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, Text, in context, CultureInfo.GetCultureInfo("es-ES", true));
        writer.Flush();

        await Assert.That(bufferWriter.WrittenMemory)
            .IsUtf8SequenceEqualTo("""
                3456,789
                3.456,789
                3456.789
                3,456.789
                """);
    }

    [Test]
    public async Task 静的プロパティ()
    {
        const string Text = """
            {{ DoubleStaticProperty }}
            {{ DoubleStaticProperty:N3 }}
            {{ DoubleStaticProperty::es-ES }}
            {{ DoubleStaticProperty:N3:es-ES }}
            """;
        var context = new DoubleContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, Text, in context);
        writer.Flush();

        await Assert.That(bufferWriter.WrittenMemory)
            .IsUtf8SequenceEqualTo("""
                4567.891
                4,567.891
                4567,891
                4.567,891
                """);
    }

    [Test]
    public async Task 静的プロパティ_InvariantCulture指定()
    {
        const string Text = """
            {{ DoubleStaticProperty }}
            {{ DoubleStaticProperty:N3 }}
            {{ DoubleStaticProperty::es-ES }}
            {{ DoubleStaticProperty:N3:es-ES }}
            """;
        var context = new DoubleContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, Text, in context, CultureInfo.InvariantCulture);
        writer.Flush();

        await Assert.That(bufferWriter.WrittenMemory)
            .IsUtf8SequenceEqualTo("""
                4567.891
                4,567.891
                4567,891
                4.567,891
                """);
    }

    [Test]
    public async Task 静的プロパティ_特定カルチャー指定()
    {
        const string Text = """
            {{ DoubleStaticProperty }}
            {{ DoubleStaticProperty:N3 }}
            {{ DoubleStaticProperty::ja-JP }}
            {{ DoubleStaticProperty:N3:ja-JP }}
            """;
        var context = new DoubleContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, Text, in context, CultureInfo.GetCultureInfo("es-ES", true));
        writer.Flush();

        await Assert.That(bufferWriter.WrittenMemory)
            .IsUtf8SequenceEqualTo("""
                4567,891
                4.567,891
                4567.891
                4,567.891
                """);
    }

    [Test]
    public async Task プロパティ()
    {
        const string Text = """
            {{ DoubleProperty }}
            {{ DoubleProperty:N3 }}
            {{ DoubleProperty::es-ES }}
            {{ DoubleProperty:N3:es-ES }}
            """;
        var context = new DoubleContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, Text, in context);
        writer.Flush();

        await Assert.That(bufferWriter.WrittenMemory)
            .IsUtf8SequenceEqualTo("""
                5678.912
                5,678.912
                5678,912
                5.678,912
                """);
    }

    [Test]
    public async Task プロパティ_InvariantCulture指定()
    {
        const string Text = """
            {{ DoubleProperty }}
            {{ DoubleProperty:N3 }}
            {{ DoubleProperty::es-ES }}
            {{ DoubleProperty:N3:es-ES }}
            """;
        var context = new DoubleContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, Text, in context, CultureInfo.InvariantCulture);
        writer.Flush();

        await Assert.That(bufferWriter.WrittenMemory)
            .IsUtf8SequenceEqualTo("""
                5678.912
                5,678.912
                5678,912
                5.678,912
                """);
    }

    [Test]
    public async Task プロパティ_特定カルチャー指定()
    {
        const string Text = """
            {{ DoubleProperty }}
            {{ DoubleProperty:N3 }}
            {{ DoubleProperty::ja-JP }}
            {{ DoubleProperty:N3:ja-JP }}
            """;
        var context = new DoubleContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, Text, in context, CultureInfo.GetCultureInfo("es-ES", true));
        writer.Flush();

        await Assert.That(bufferWriter.WrittenMemory)
            .IsUtf8SequenceEqualTo("""
                5678,912
                5.678,912
                5678.912
                5,678.912
                """);
    }
}
