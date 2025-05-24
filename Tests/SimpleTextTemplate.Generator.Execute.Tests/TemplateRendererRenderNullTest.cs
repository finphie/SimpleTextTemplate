using System.Buffers;
using System.Text;
using Shouldly;
using SimpleTextTemplate.Generator.Tests.Core;
using Xunit;

namespace SimpleTextTemplate.Generator.Execute.Tests;

public sealed class TemplateRendererRenderNullTest
{
    [Fact]
    public void 定数()
    {
        var context = new NullContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, "{{ NullStringConstantField }}", in context);
        TemplateRenderer.Render(ref writer, "{{ NullObjectConstantField }}", in context);
        writer.Flush();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .ShouldBeEmpty();
    }

    [Fact]
    public void 静的フィールド_NullReferenceException()
    {
        var context = new NullContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        Should.Throw<NullReferenceException>(() =>
        {
            var writer = TemplateWriter.Create(bufferWriter);
            TemplateRenderer.Render(ref writer, "{{ NullBytesStaticField }}", in context);
        });

        Should.Throw<NullReferenceException>(() =>
        {
            var writer = TemplateWriter.Create(bufferWriter);
            TemplateRenderer.Render(ref writer, "{{ NullCharsStaticField }}", in context);
        });

        Should.Throw<NullReferenceException>(() =>
        {
            var writer = TemplateWriter.Create(bufferWriter);
            TemplateRenderer.Render(ref writer, "{{ NullStringStaticField }}", in context);
        });
    }

    [Fact]
    public void 静的フィールド()
    {
        var context = new NullContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, "{{ NullObjectStaticField }}", in context);
        writer.Flush();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .ShouldBeEmpty();
    }

    [Fact]
    public void フィールド_NullReferenceException()
    {
        var context = new NullContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        Should.Throw<NullReferenceException>(() =>
        {
            var writer = TemplateWriter.Create(bufferWriter);
            TemplateRenderer.Render(ref writer, "{{ NullBytesField }}", in context);
        });

        Should.Throw<NullReferenceException>(() =>
        {
            var writer = TemplateWriter.Create(bufferWriter);
            TemplateRenderer.Render(ref writer, "{{ NullCharsField }}", in context);
        });

        Should.Throw<NullReferenceException>(() =>
        {
            var writer = TemplateWriter.Create(bufferWriter);
            TemplateRenderer.Render(ref writer, "{{ NullStringField }}", in context);
        });
    }

    [Fact]
    public void フィールド()
    {
        var context = new NullContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, "{{ NullObjectField }}", in context);
        writer.Flush();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .ShouldBeEmpty();
    }

    [Fact]
    public void 静的プロパティ_NullReferenceException()
    {
        var context = new NullContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        Should.Throw<NullReferenceException>(() =>
        {
            var writer = TemplateWriter.Create(bufferWriter);
            TemplateRenderer.Render(ref writer, "{{ NullBytesStaticProperty }}", in context);
        });

        Should.Throw<NullReferenceException>(() =>
        {
            var writer = TemplateWriter.Create(bufferWriter);
            TemplateRenderer.Render(ref writer, "{{ NullCharsStaticProperty }}", in context);
        });

        Should.Throw<NullReferenceException>(() =>
        {
            var writer = TemplateWriter.Create(bufferWriter);
            TemplateRenderer.Render(ref writer, "{{ NullStringStaticProperty }}", in context);
        });
    }

    [Fact]
    public void 静的プロパティ()
    {
        var context = new NullContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, "{{ NullObjectStaticProperty }}", in context);
        writer.Flush();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .ShouldBeEmpty();
    }

    [Fact]
    public void プロパティ_NullReferenceException()
    {
        var context = new NullContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        Should.Throw<NullReferenceException>(() =>
        {
            var writer = TemplateWriter.Create(bufferWriter);
            TemplateRenderer.Render(ref writer, "{{ NullBytesProperty }}", in context);
        });

        Should.Throw<NullReferenceException>(() =>
        {
            var writer = TemplateWriter.Create(bufferWriter);
            TemplateRenderer.Render(ref writer, "{{ NullCharsProperty }}", in context);
        });

        Should.Throw<NullReferenceException>(() =>
        {
            var writer = TemplateWriter.Create(bufferWriter);
            TemplateRenderer.Render(ref writer, "{{ NullStringProperty }}", in context);
        });
    }

    [Fact]
    public void プロパティ()
    {
        var context = new NullContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, "{{ NullObjectProperty }}", in context);
        writer.Flush();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .ShouldBeEmpty();
    }
}
