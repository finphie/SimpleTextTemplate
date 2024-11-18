using System.Text;
using CommunityToolkit.HighPerformance.Buffers;
using SimpleTextTemplate;

var context = new SampleContext("Hello, World", new(2000, 1, 1, 0, 0, 0, TimeSpan.Zero));

var bufferWriter = new ArrayPoolBufferWriter<byte>();
var writer = TemplateWriter.Create(bufferWriter);

TemplateRenderer.Render(ref writer, "{{ DateTime }}_{{ Identifier }}!!!", context);
writer.Flush();

Console.WriteLine(Encoding.UTF8.GetString(bufferWriter.WrittenSpan));

bufferWriter.Dispose();

/// <summary>
/// コンテキスト
/// </summary>
/// <param name="Identifier">文字列</param>
/// <param name="DateTime">時刻</param>
readonly record struct SampleContext(string Identifier, DateTimeOffset DateTime);
