using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;
using SimpleTextTemplate;

#pragma warning disable SA1516 // Elements should be separated by blank line

var symbols = new Dictionary<string, byte[]>
{
    { "Identifier", Encoding.UTF8.GetBytes("Hello, World!") }
};

var bufferWriter = new ArrayBufferWriter<byte>();
TemplateEx.GeneratePageTemplate(bufferWriter, Context.Create(symbols));

Console.WriteLine(Encoding.UTF8.GetString(bufferWriter.WrittenSpan));

#pragma warning restore SA1516 // Elements should be separated by blank line