using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;
using SimpleTextTemplate;

var bufferWriter = new ArrayBufferWriter<byte>();
var symbols = new Dictionary<string, byte[]>
{
    { "Identifier", Encoding.UTF8.GetBytes("Hello, World!") }
};

TemplateEx.GeneratePageTemplate(bufferWriter, Context.Create(symbols));

Console.WriteLine(Encoding.UTF8.GetString(bufferWriter.WrittenSpan));