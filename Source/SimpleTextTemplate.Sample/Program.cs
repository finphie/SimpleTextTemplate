using System.Text;
using CommunityToolkit.HighPerformance.Buffers;
using SimpleTextTemplate.Sample;

var context = new SampleContext(Encoding.UTF8.GetBytes("Hello, World!"));

using var bufferWriter = new ArrayPoolBufferWriter<byte>();
ZTemplate.Render(bufferWriter, context);

Console.WriteLine(Encoding.UTF8.GetString(bufferWriter.WrittenSpan));
