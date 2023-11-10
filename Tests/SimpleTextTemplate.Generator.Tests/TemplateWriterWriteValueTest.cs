using System.Buffers;
using System.Globalization;
using System.Text;
using FluentAssertions;
using Xunit;

namespace SimpleTextTemplate.Generator.Tests;

public sealed class TemplateWriterWriteValueTest
{
    enum Test
    {
        A
    }

    [Fact]
    public void Int32_バッファーライターに書き込み()
    {
        var value = 123;
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = new TemplateWriter<ArrayBufferWriter<byte>>(ref bufferWriter))
        {
            writer.WriteValue(value);
        }

        bufferWriter.WrittenSpan.ToArray()
            .Should()
            .Equal(Encoding.UTF8.GetBytes(value.ToString(CultureInfo.InvariantCulture)));
    }

    [Fact]
    public void Double_バッファーライターに書き込み()
    {
        var value = 1.23;
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = new TemplateWriter<ArrayBufferWriter<byte>>(ref bufferWriter))
        {
            writer.WriteValue(value);
        }

        bufferWriter.WrittenSpan.ToArray()
            .Should()
            .Equal(Encoding.UTF8.GetBytes(value.ToString(CultureInfo.InvariantCulture)));
    }

    [Fact]
    public void DateTimeOffset_バッファーライターに書き込み()
    {
        var value = new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.FromHours(9));
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = new TemplateWriter<ArrayBufferWriter<byte>>(ref bufferWriter))
        {
            writer.WriteValue(value);
        }

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan.ToArray())
            .Should()
            .Be(value.ToString(CultureInfo.InvariantCulture));
    }

    [Fact]
    public void IUtf8SpanFormattable_バッファーライターに書き込み()
    {
        var value = 11_111_111_111_111_111_111;
        var bufferWriter = new ArrayBufferWriter<byte>();
        var count = 0;

        using (var writer = new TemplateWriter<ArrayBufferWriter<byte>>(ref bufferWriter))
        {
            for (; count < 20; count++)
            {
                writer.WriteValue(value);
            }
        }

        var array = bufferWriter.WrittenSpan.ToArray();
        array.Should().OnlyContain(static x => x == (byte)'1');
        array.Should().HaveCount(20 * count);
    }

    [Fact]
    public void ISpanFormattable_バッファーライターに書き込み()
    {
        var value = new A(11_111_111_111_111_111_111);
        var bufferWriter = new ArrayBufferWriter<byte>();
        var count = 0;

        using (var writer = new TemplateWriter<ArrayBufferWriter<byte>>(ref bufferWriter))
        {
            for (; count < 20; count++)
            {
                writer.WriteValue(value);
            }
        }

        var array = bufferWriter.WrittenSpan.ToArray();
        array.Should().OnlyContain(static x => x == (byte)'1');
        array.Should().HaveCount(20 * count);
    }

    sealed record A(ulong Value) : ISpanFormattable
    {
        public string ToString(string? format, IFormatProvider? formatProvider)
            => Value.ToString(format, formatProvider);

        public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
            => Value.TryFormat(destination, out charsWritten, format, provider);
    }
}
