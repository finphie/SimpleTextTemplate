using System.Text;
using CommunityToolkit.HighPerformance.Buffers;
using FluentAssertions;
using Xunit;

namespace SimpleTextTemplate.Generator.Tests;

public sealed class GeneratorTest
{
    const string Identifier = "Hello, World!";

    [Fact]
    public void Test()
    {
        var identifier = Encoding.UTF8.GetBytes(Identifier);
        var context = new TestContext(identifier);

        using var bufferWriter = new ArrayPoolBufferWriter<byte>();
        ZTemplate.Render(bufferWriter, context);

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan).Should().Be(Identifier);
    }

    [Fact]
    public void FileTest()
    {
        var identifier = Encoding.UTF8.GetBytes(Identifier);
        var context = new TestContext(identifier);

        using var bufferWriter = new ArrayPoolBufferWriter<byte>();
        ZTemplate.FileRender(bufferWriter, context);

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan[..^1]).Should().Be(Identifier);
    }
}
