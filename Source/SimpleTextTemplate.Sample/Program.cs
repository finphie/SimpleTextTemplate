using System;
using System.Text;
using Microsoft.Toolkit.HighPerformance.Buffers;
using SimpleTextTemplate;
using SimpleTextTemplate.Contexts;
using Utf8Utility;

#pragma warning disable SA1516 // Elements should be separated by blank line

var symbols = new Utf8StringDictionary<Utf8String>();
symbols.Add((Utf8String)"Identifier", (Utf8String)"Hello, World!");

using var bufferWriter = new ArrayPoolBufferWriter<byte>();
ZTemplate.GeneratePageTemplate(bufferWriter, Context.Create(symbols));

Console.WriteLine(Encoding.UTF8.GetString(bufferWriter.WrittenSpan));

#pragma warning restore SA1516 // Elements should be separated by blank line