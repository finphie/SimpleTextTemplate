using System.Text;
using FluentAssertions;
using Microsoft.Toolkit.HighPerformance.Buffers;
using SimpleTextTemplate.Contexts;
using Utf8Utility;
using Xunit;

namespace SimpleTextTemplate.Generator.Tests
{
    public sealed class GeneratorTest
    {
        [Fact]
        public void GeneratePageTemplateTest()
        {
            var key = (Utf8String)"Identifier";
            var message = (Utf8String)"Hello, World!";

            var symbols = new Utf8StringDictionary<Utf8String>();
            symbols.Add(key, message);

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
}