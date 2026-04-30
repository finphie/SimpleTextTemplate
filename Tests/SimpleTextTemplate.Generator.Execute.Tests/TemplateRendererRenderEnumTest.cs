using System.Buffers;
using SimpleTextTemplate.Tests.Assertions;
using SimpleTextTemplate.Tests.TestData;

namespace SimpleTextTemplate.Generator.Execute.Tests;

public sealed class TemplateRendererRenderEnumTest
{
    [Test]
    public async Task 定数()
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

        await Assert.That(bufferWriter.WrittenMemory)
            .IsUtf8SequenceEqualTo("""
                Test1
                1
                00000001
                """);
    }

    [Test]
    public async Task 定数が無効な値()
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

        await Assert.That(bufferWriter.WrittenMemory)
            .IsUtf8SequenceEqualTo("""
                99
                99
                """);
    }

    [Test]
    public async Task Flags属性を付与したEnumの定数()
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

        await Assert.That(bufferWriter.WrittenMemory)
            .IsUtf8SequenceEqualTo("""
                Test1, Test2
                3
                """);
    }

    [Test]
    public async Task 静的フィールド()
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

        await Assert.That(bufferWriter.WrittenMemory)
            .IsUtf8SequenceEqualTo("""
                Test2
                2
                """);
    }

    [Test]
    public async Task フィールド()
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

        await Assert.That(bufferWriter.WrittenMemory)
            .IsUtf8SequenceEqualTo("""
                Test3
                3
                """);
    }

    [Test]
    public async Task 静的プロパティ()
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

        await Assert.That(bufferWriter.WrittenMemory)
            .IsUtf8SequenceEqualTo("""
                Test4
                4
                """);
    }

    [Test]
    public async Task プロパティ()
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

        await Assert.That(bufferWriter.WrittenMemory)
            .IsUtf8SequenceEqualTo("""
                Test5
                5
                """);
    }
}
