using System.Buffers;
using SimpleTextTemplate.Tests.TestData;

namespace SimpleTextTemplate.Generator.Execute.Tests;

public sealed class TemplateRendererRenderNullTest
{
    [Test]
    public async Task 定数()
    {
        var context = new NullContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, "{{ NullStringConstantField }}", in context);
        TemplateRenderer.Render(ref writer, "{{ NullObjectConstantField }}", in context);
        writer.Flush();

        await Assert.That(bufferWriter.WrittenMemory)
            .IsEmpty();
    }

    [Test]
    public async Task 静的フィールド_NullReferenceException()
    {
        var context = new NullContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var action1 = () =>
        {
            var writer = TemplateWriter.Create(bufferWriter);
            TemplateRenderer.Render(ref writer, "{{ NullBytesStaticField }}", in context);
        };
        await Assert.That(action1).ThrowsExactly<NullReferenceException>();

        var action2 = () =>
        {
            var writer = TemplateWriter.Create(bufferWriter);
            TemplateRenderer.Render(ref writer, "{{ NullCharsStaticField }}", in context);
        };
        await Assert.That(action2).ThrowsExactly<NullReferenceException>();

        var action3 = () =>
        {
            var writer = TemplateWriter.Create(bufferWriter);
            TemplateRenderer.Render(ref writer, "{{ NullStringStaticField }}", in context);
        };
        await Assert.That(action3).ThrowsExactly<NullReferenceException>();
    }

    [Test]
    public async Task 静的フィールド()
    {
        var context = new NullContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, "{{ NullObjectStaticField }}", in context);
        writer.Flush();

        await Assert.That(bufferWriter.WrittenMemory)
            .IsEmpty();
    }

    [Test]
    public async Task フィールド_NullReferenceException()
    {
        var context = new NullContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var action1 = () =>
        {
            var writer = TemplateWriter.Create(bufferWriter);
            TemplateRenderer.Render(ref writer, "{{ NullBytesField }}", in context);
        };
        await Assert.That(action1).ThrowsExactly<NullReferenceException>();

        var action2 = () =>
        {
            var writer = TemplateWriter.Create(bufferWriter);
            TemplateRenderer.Render(ref writer, "{{ NullCharsField }}", in context);
        };
        await Assert.That(action2).ThrowsExactly<NullReferenceException>();

        var action3 = () =>
        {
            var writer = TemplateWriter.Create(bufferWriter);
            TemplateRenderer.Render(ref writer, "{{ NullStringField }}", in context);
        };
        await Assert.That(action3).ThrowsExactly<NullReferenceException>();
    }

    [Test]
    public async Task フィールド()
    {
        var context = new NullContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, "{{ NullObjectField }}", in context);
        writer.Flush();

        await Assert.That(bufferWriter.WrittenMemory)
            .IsEmpty();
    }

    [Test]
    public async Task 静的プロパティ_NullReferenceException()
    {
        var context = new NullContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var action1 = () =>
        {
            var writer = TemplateWriter.Create(bufferWriter);
            TemplateRenderer.Render(ref writer, "{{ NullBytesStaticProperty }}", in context);
        };
        await Assert.That(action1).ThrowsExactly<NullReferenceException>();

        var action2 = () =>
        {
            var writer = TemplateWriter.Create(bufferWriter);
            TemplateRenderer.Render(ref writer, "{{ NullCharsStaticProperty }}", in context);
        };
        await Assert.That(action2).ThrowsExactly<NullReferenceException>();

        var action3 = () =>
        {
            var writer = TemplateWriter.Create(bufferWriter);
            TemplateRenderer.Render(ref writer, "{{ NullStringStaticProperty }}", in context);
        };
        await Assert.That(action3).ThrowsExactly<NullReferenceException>();
    }

    [Test]
    public async Task 静的プロパティ()
    {
        var context = new NullContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, "{{ NullObjectStaticProperty }}", in context);
        writer.Flush();

        await Assert.That(bufferWriter.WrittenMemory)
            .IsEmpty();
    }

    [Test]
    public async Task プロパティ_NullReferenceException()
    {
        var context = new NullContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var action1 = () =>
        {
            var writer = TemplateWriter.Create(bufferWriter);
            TemplateRenderer.Render(ref writer, "{{ NullBytesProperty }}", in context);
        };
        await Assert.That(action1).ThrowsExactly<NullReferenceException>();

        var action2 = () =>
        {
            var writer = TemplateWriter.Create(bufferWriter);
            TemplateRenderer.Render(ref writer, "{{ NullCharsProperty }}", in context);
        };
        await Assert.That(action2).ThrowsExactly<NullReferenceException>();

        var action3 = () =>
        {
            var writer = TemplateWriter.Create(bufferWriter);
            TemplateRenderer.Render(ref writer, "{{ NullStringProperty }}", in context);
        };
        await Assert.That(action3).ThrowsExactly<NullReferenceException>();
    }

    [Test]
    public async Task プロパティ()
    {
        var context = new NullContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, "{{ NullObjectProperty }}", in context);
        writer.Flush();

        await Assert.That(bufferWriter.WrittenMemory)
            .IsEmpty();
    }
}
