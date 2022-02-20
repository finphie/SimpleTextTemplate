using System.Text;
using CommunityToolkit.HighPerformance.Buffers;
using SimpleTextTemplate.Sample;

#pragma warning disable SA1516 // Elements should be separated by blank line

var context = new SampleContext(Encoding.UTF8.GetBytes("Hello, World!"));

using var bufferWriter = new ArrayPoolBufferWriter<byte>();
ZTemplate.Render(bufferWriter, context);

Console.WriteLine(Encoding.UTF8.GetString(bufferWriter.WrittenSpan));

#pragma warning restore SA1516 // Elements should be separated by blank line
