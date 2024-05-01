using System.Buffers;
using System.Text;
using FluentAssertions;
using SimpleTextTemplate.Generator.Tests.Core;
using Xunit;

namespace SimpleTextTemplate.Generator.Execute.Tests;

public sealed class TemplateWriterWriteStringTest
{
    [Fact]
    public void 定数()
    {
        var context = new StringContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = TemplateWriter.Create(bufferWriter))
        {
            writer.Write("{{ StringConstantField }}", in context);
        }

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("_StringConstantField");
    }

    [Fact]
    public void 静的フィールド()
    {
        var context = new StringContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = TemplateWriter.Create(bufferWriter))
        {
            writer.Write("{{ StringStaticField }}", in context);
        }

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("_StringStaticField");
    }

    [Fact]
    public void フィールド()
    {
        var context = new StringContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = TemplateWriter.Create(bufferWriter))
        {
            writer.Write("{{ StringField }}", in context);
        }

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("_StringField");
    }

    [Fact]
    public void 静的プロパティ()
    {
        var context = new StringContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = TemplateWriter.Create(bufferWriter))
        {
            writer.Write("{{ StringStaticProperty }}", in context);
        }

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("_StringStaticProperty");
    }

    [Fact]
    public void プロパティ()
    {
        var context = new StringContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = TemplateWriter.Create(bufferWriter))
        {
            writer.Write("{{ StringProperty }}", in context);
        }

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("_StringProperty");
    }
}
