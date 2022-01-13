using System.Text;
using CommunityToolkit.HighPerformance.Buffers;
using SimpleTextTemplate;
using SimpleTextTemplate.Contexts;
using Utf8Utility;

#pragma warning disable SA1516 // Elements should be separated by blank line

var symbols = new Utf8ArrayDictionary<Utf8Array>();
symbols.TryAdd((Utf8Array)"Identifier", (Utf8Array)"Hello, World!");

using var bufferWriter = new ArrayPoolBufferWriter<byte>();
ZTemplate.GeneratePageTemplate(bufferWriter, Context.Create(symbols));

Console.WriteLine(Encoding.UTF8.GetString(bufferWriter.WrittenSpan));

#pragma warning restore SA1516 // Elements should be separated by blank line
