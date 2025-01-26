using System.Buffers;
using System.Text;
using Shouldly;
using SimpleTextTemplate.Generator.Tests.Core;
using Xunit;

namespace SimpleTextTemplate.Generator.Execute.Tests;

public sealed class TemplateRendererRenderRefStructTest
{
    [Fact]
    public void 静的フィールド()
    {
        var context = new RefStructTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, "{{ BytesStaticField }}", in context);
        writer.Flush();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .ShouldBe("_BytesStaticField");
    }

    [Fact]
    public void フィールド()
    {
        var context = new RefStructTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, "{{ BytesField }}", in context);
        writer.Flush();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .ShouldBe("_BytesField");
    }

    [Fact]
    public void ReadOnlySpanフィールド()
    {
        var context = new RefStructTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, "{{ BytesSpanField }}", in context);
        writer.Flush();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .ShouldBe("_BytesSpanField");
    }

    [Fact]
    public void 静的プロパティ()
    {
        var context = new RefStructTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, "{{ BytesStaticProperty }}", in context);
        writer.Flush();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .ShouldBe("_BytesStaticProperty");
    }

    [Fact]
    public void プロパティ()
    {
        var context = new RefStructTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, "{{ BytesProperty }}", in context);
        writer.Flush();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .ShouldBe("_BytesProperty");
    }
}
