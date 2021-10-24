using System.Text;
using FluentAssertions;
using Microsoft.Toolkit.HighPerformance.Buffers;
using SimpleTextTemplate.Contexts;
using Utf8Utility;
using Xunit;

namespace SimpleTextTemplate.Generator.Tests;

public sealed class GeneratorTest
{
    [Fact]
    public void GeneratePageTemplateTest()
    {
        var key = (Utf8Array)"Identifier";
        var message = (Utf8Array)"Hello, World!";

        var symbols = new Utf8ArrayDictionary<Utf8Array>();
        symbols.TryAdd(key, message);

        using var bufferWriter = new ArrayPoolBufferWriter<byte>();
        ZTemplate.GeneratePageTemplate(bufferWriter, Context.Create(symbols));

        var expected = @$"<html>

<body>
  {message}
</body>

</html>";
        bufferWriter.WrittenSpan.ToArray().Should().Equal(Encoding.UTF8.GetBytes(expected));
    }
}
