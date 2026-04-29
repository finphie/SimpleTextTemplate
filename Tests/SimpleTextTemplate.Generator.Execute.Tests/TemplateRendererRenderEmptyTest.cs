using System.Buffers;
using SimpleTextTemplate.Generator.Tests.TestData;

namespace SimpleTextTemplate.Generator.Execute.Tests;

public sealed class TemplateRendererRenderEmptyTest
{
    [Test]
    public async Task 定数()
    {
        var context = new EmptyContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, "{{ EmptyStringConstantField }}", in context);
        writer.Flush();

        await Assert.That(bufferWriter.WrittenMemory)
            .IsEmpty();
    }

    [Test]
    public async Task 静的フィールド()
    {
        var context = new EmptyContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, "{{ EmptyStringStaticField }}", in context);
        writer.Flush();

        await Assert.That(bufferWriter.WrittenMemory)
            .IsEmpty();
    }

    [Test]
    public async Task フィールド()
    {
        var context = new EmptyContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, "{{ EmptyStringField }}", in context);
        writer.Flush();

        await Assert.That(bufferWriter.WrittenMemory)
            .IsEmpty();
    }

    [Test]
    public async Task 静的プロパティ()
    {
        var context = new EmptyContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, "{{ EmptyStringStaticProperty }}", in context);
        writer.Flush();

        await Assert.That(bufferWriter.WrittenMemory)
            .IsEmpty();
    }

    [Test]
    public async Task プロパティ()
    {
        var context = new EmptyContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, "{{ EmptyStringProperty }}", in context);
        writer.Flush();

        await Assert.That(bufferWriter.WrittenMemory)
            .IsEmpty();
    }
}
