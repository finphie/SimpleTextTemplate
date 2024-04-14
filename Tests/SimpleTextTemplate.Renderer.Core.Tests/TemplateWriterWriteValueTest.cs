using System.Buffers;
using System.Globalization;
using System.Text;
using FluentAssertions;
using Xunit;

namespace SimpleTextTemplate.Renderer.Core.Tests;

public sealed class TemplateWriterWriteValueTest
{
    enum Test
    {
        A
    }

    [Fact]
    public void Int32_バッファーライターに書き込み()
    {
        var value = 1234;
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = new TemplateWriter<ArrayBufferWriter<byte>>(ref bufferWriter))
        {
            writer.WriteValue(value, default, CultureInfo.InvariantCulture);
            writer.WriteValue(value, "N3", CultureInfo.InvariantCulture);
            writer.WriteValue(value, "N3", new CultureInfo("es-ES"));
        }

        bufferWriter.WrittenSpan.ToArray()
            .Should()
            .Equal("12341,234.0001.234,000"u8.ToArray());
    }

    [Fact]
    public void Double_バッファーライターに書き込み()
    {
        var value = 1234.567;
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = new TemplateWriter<ArrayBufferWriter<byte>>(ref bufferWriter))
        {
            writer.WriteValue(value, default, CultureInfo.InvariantCulture);
            writer.WriteValue(value, "F2", CultureInfo.InvariantCulture);
            writer.WriteValue(value, "F3", new CultureInfo("es-ES"));
        }

        bufferWriter.WrittenSpan.ToArray()
            .Should()
            .Equal("1234.5671234.571234,567"u8.ToArray());
    }

    [Fact]
    public void DateTimeOffset_バッファーライターに書き込み()
    {
        var value = new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.FromHours(9));
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = new TemplateWriter<ArrayBufferWriter<byte>>(ref bufferWriter))
        {
            writer.WriteValue(value, default, CultureInfo.InvariantCulture);
            writer.WriteValue(value, "d", CultureInfo.InvariantCulture);
            writer.WriteValue(value, "D", new CultureInfo("ja-JP"));
        }

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("01/01/2000 00:00:00 +09:0001/01/20002000年1月1日");
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
                writer.WriteValue(value, default, CultureInfo.InvariantCulture);
            }
        }

        var array = bufferWriter.WrittenSpan.ToArray();
        array.Should().OnlyContain(static x => x == (byte)'1');
        array.Should().HaveCount(20 * count);
    }

    [Fact]
    public void ISpanFormattable_バッファーライターに書き込み()
    {
        var value = new SpanFormattableRecord(new(2000, 1, 1, 0, 0, 0, TimeSpan.FromHours(9)));
        var bufferWriter = new ArrayBufferWriter<byte>();
        var count = 0;

        using (var writer = new TemplateWriter<ArrayBufferWriter<byte>>(ref bufferWriter))
        {
            for (; count < 20; count++)
            {
                writer.WriteValue(value, default, CultureInfo.InvariantCulture);
            }
        }

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be(string.Concat(Enumerable.Repeat(value.ToString(null, CultureInfo.InvariantCulture), 20)));
    }

    [Fact]
    public void IFormattable_バッファーライターに書き込み()
    {
        var value = new FormattableRecord(new(2000, 1, 1, 0, 0, 0, TimeSpan.FromHours(9)));
        var bufferWriter = new ArrayBufferWriter<byte>();
        var count = 0;

        using (var writer = new TemplateWriter<ArrayBufferWriter<byte>>(ref bufferWriter))
        {
            for (; count < 20; count++)
            {
                writer.WriteValue(value, default, CultureInfo.InvariantCulture);
            }
        }

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be(string.Concat(Enumerable.Repeat(value.ToString(null, CultureInfo.InvariantCulture), 20)));
    }

    sealed record FormattableRecord(DateTimeOffset Value) : IFormattable
    {
        public string ToString(string? format, IFormatProvider? formatProvider)
            => Value.ToString(format, formatProvider);
    }

    sealed record SpanFormattableRecord(DateTimeOffset Value) : ISpanFormattable
    {
        public string ToString(string? format, IFormatProvider? formatProvider)
            => Value.ToString(format, formatProvider);

        public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
            => Value.TryFormat(destination, out charsWritten, format, provider);
    }
}
