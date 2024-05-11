using System.Buffers;
using System.Text;
using FluentAssertions;
using SimpleTextTemplate.Generator.Tests.Core;
using Xunit;

namespace SimpleTextTemplate.Generator.Execute.Tests;

public sealed class TemplateWriterWriteCharArrayTest
{
    [Fact]
    public void 静的フィールド()
    {
        var context = new CharArrayContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = TemplateWriter.Create(bufferWriter))
        {
            writer.Write("{{ CharsStaticField }}", in context);
        }

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("_CharsStaticField");
    }

    [Fact]
    public void フィールド()
    {
        var context = new CharArrayContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = TemplateWriter.Create(bufferWriter))
        {
            writer.Write("{{ CharsField }}", in context);
        }

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("_CharsField");
    }

    [Fact]
    public void 静的プロパティ()
    {
        var context = new CharArrayContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = TemplateWriter.Create(bufferWriter))
        {
            writer.Write("{{ CharsStaticProperty }}", in context);
        }

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("_CharsStaticProperty");
    }

    [Fact]
    public void プロパティ()
    {
        var context = new CharArrayContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = TemplateWriter.Create(bufferWriter))
        {
            writer.Write("{{ CharsProperty }}", in context);
        }

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("_CharsProperty");
    }

    [Fact]
    public void 静的ReadOnlySpanプロパティ()
    {
        var context = new CharArrayContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = TemplateWriter.Create(bufferWriter))
        {
            writer.Write("{{ CharsSpanStaticProperty }}", in context);
        }

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("_CharsSpanStaticProperty");
    }

    [Fact]
    public void ReadOnlySpanプロパティ()
    {
        var context = new CharArrayContextTestData();
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = TemplateWriter.Create(bufferWriter))
        {
            writer.Write("{{ CharsSpanProperty }}", in context);
        }

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("_CharsSpanProperty");
    }
}
