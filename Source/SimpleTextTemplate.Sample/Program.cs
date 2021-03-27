using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Toolkit.HighPerformance.Buffers;
using SimpleTextTemplate;
using SimpleTextTemplate.Contexts;

#pragma warning disable SA1516 // Elements should be separated by blank line

var symbols = new Dictionary<string, byte[]>
{
    { "Identifier", Encoding.UTF8.GetBytes("Hello, World!") }
};

using var bufferWriter = new ArrayPoolBufferWriter<byte>();
ZTemplate.GeneratePageTemplate(bufferWriter, Context.Create(symbols));

Console.WriteLine(Encoding.UTF8.GetString(bufferWriter.WrittenSpan));

#pragma warning restore SA1516 // Elements should be separated by blank line