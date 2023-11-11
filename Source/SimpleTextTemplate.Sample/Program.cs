using System.Diagnostics.CodeAnalysis;
using System.Text;
using CommunityToolkit.HighPerformance.Buffers;
using SimpleTextTemplate;

var context = new SampleContext("Hello, World", new(2000, 1, 1, 0, 0, 0, TimeSpan.Zero));

var bufferWriter = new ArrayPoolBufferWriter<byte>();
var template = new TemplateWriter<ArrayPoolBufferWriter<byte>>(ref bufferWriter);

template.Write("{{ DateTime }}_{{ Identifier }}!!!", context);
template.Dispose();

Console.WriteLine(Encoding.UTF8.GetString(bufferWriter.WrittenSpan));

bufferWriter.Dispose();

/// <summary>
/// コンテキスト
/// </summary>
/// <param name="Identifier">文字列</param>
/// <param name="DateTime">時刻</param>
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "サンプルプロジェクト")]
readonly record struct SampleContext(string Identifier, [property: Identifier("o")] DateTimeOffset DateTime);
