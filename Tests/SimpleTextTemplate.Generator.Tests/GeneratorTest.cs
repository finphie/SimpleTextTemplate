using CommunityToolkit.HighPerformance.Buffers;
using FluentAssertions;
using Xunit;

namespace SimpleTextTemplate.Generator.Tests;

public sealed class GeneratorTest
{
    static readonly byte[] Identifier = "Hello, World!"u8.ToArray();

    [Fact]
    public void Test()
    {
        var context = new TestContext(Identifier);

        using var bufferWriter = new ArrayPoolBufferWriter<byte>();
        ZTemplate.Render(bufferWriter, context);

        bufferWriter.WrittenSpan.ToArray().Should().Equal(Identifier);
    }

    [Fact]
    public void FileTest()
    {
        var context = new TestContext(Identifier);

        using var bufferWriter = new ArrayPoolBufferWriter<byte>();
        ZTemplate.FileRender(bufferWriter, context);

        bufferWriter.WrittenSpan[..^1].ToArray().Should().Equal(Identifier);
    }
}
