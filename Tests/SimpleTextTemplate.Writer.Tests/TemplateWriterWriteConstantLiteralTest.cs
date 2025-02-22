﻿using System.Buffers;
using System.Text;
using Shouldly;
using Xunit;

namespace SimpleTextTemplate.Writer.Tests;

public sealed class TemplateWriterWriteConstantLiteralTest
{
    [Theory]
    [InlineData("0")]
    [InlineData("01")]
    [InlineData("012")]
    [InlineData("0123")]
    [InlineData("01234")]
    [InlineData("012345")]
    [InlineData("0123456")]
    [InlineData("01234567")]
    [InlineData("012345678")]
    [InlineData("0123456789")]
    public void 文字列_バッファーライターに書き込み(string value)
    {
        var utf8Value = Encoding.UTF8.GetBytes(value);
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        writer.WriteConstantLiteral(utf8Value);
        writer.Flush();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .ShouldBe(value);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(9)]
    [InlineData(10)]
    public void 指定された長さの文字列を複数回追加_バッファーライターに書き込み(int length)
    {
        var value = new string('a', length);
        var utf8Value = Encoding.UTF8.GetBytes(value);
        var bufferWriter = new ArrayBufferWriter<byte>();
        var count = 0;

        var writer = TemplateWriter.Create(bufferWriter);

        for (; count < 10; count++)
        {
            writer.WriteConstantLiteral(utf8Value);
        }

        writer.Flush();

        var array = bufferWriter.WrittenSpan.ToArray();
        array.ShouldAllBe(static x => x == (byte)'a');
        array.Length.ShouldBe(value.Length * count);
    }
}
