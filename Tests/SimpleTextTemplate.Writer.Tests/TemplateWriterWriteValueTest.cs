using System.Buffers;
using System.Globalization;
using System.Text;
using Shouldly;
using Xunit;

namespace SimpleTextTemplate.Writer.Tests;

public sealed class TemplateWriterWriteValueTest
{
    [Fact]
    public void Int32_バッファーライターに書き込み()
    {
        const int Value = 1234;
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        writer.WriteValue(Value, default, CultureInfo.InvariantCulture);
        writer.WriteValue(Value, "N3", CultureInfo.InvariantCulture);
        writer.WriteValue(Value, "N3", CultureInfo.GetCultureInfo("es-ES", true));
        writer.Flush();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .ShouldBe("12341,234.0001.234,000");
    }

    [Fact]
    public void Double_バッファーライターに書き込み()
    {
        const double Value = 1234.567;
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        writer.WriteValue(Value, default, CultureInfo.InvariantCulture);
        writer.WriteValue(Value, "F2", CultureInfo.InvariantCulture);
        writer.WriteValue(Value, "F3", CultureInfo.GetCultureInfo("es-ES", true));
        writer.Flush();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .ShouldBe("1234.5671234.571234,567");
    }

    [Fact]
    public void DateTimeOffset_バッファーライターに書き込み()
    {
        var value = new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.FromHours(9));
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        writer.WriteValue(value, default, CultureInfo.InvariantCulture);
        writer.WriteValue(value, "d", CultureInfo.InvariantCulture);
        writer.WriteValue(value, "D", CultureInfo.GetCultureInfo("ja-JP", true));
        writer.Flush();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .ShouldBe("01/01/2000 00:00:00 +09:0001/01/20002000年1月1日土曜日");
    }

    [Fact]
    public void IUtf8SpanFormattable_バッファーライターに書き込み()
    {
        const ulong Value = 11_111_111_111_111_111_111;
        var bufferWriter = new ArrayBufferWriter<byte>();
        var count = 0;

        var writer = TemplateWriter.Create(bufferWriter);

        for (; count < 20; count++)
        {
            writer.WriteValue(Value, default, CultureInfo.InvariantCulture);
        }

        writer.Flush();

        var array = bufferWriter.WrittenSpan.ToArray();
        array.ShouldAllBe(static x => x == (byte)'1');
        array.Length.ShouldBe(20 * count);
    }

    [Fact]
    public void ISpanFormattable_バッファーライターに書き込み()
    {
        var value = new SpanFormattableRecord(new(2000, 1, 1, 0, 0, 0, TimeSpan.FromHours(9)));
        var bufferWriter = new ArrayBufferWriter<byte>();
        var count = 0;

        var writer = TemplateWriter.Create(bufferWriter);

        for (; count < 20; count++)
        {
            writer.WriteValue(value, default, CultureInfo.InvariantCulture);
        }

        writer.Flush();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .ShouldBe(string.Concat(Enumerable.Repeat(value.ToString(null, CultureInfo.InvariantCulture), 20)));
    }

    [Fact]
    public void IFormattable_バッファーライターに書き込み()
    {
        var value = new FormattableRecord(new(2000, 1, 1, 0, 0, 0, TimeSpan.FromHours(9)));
        var bufferWriter = new ArrayBufferWriter<byte>();
        var count = 0;

        var writer = TemplateWriter.Create(bufferWriter);

        for (; count < 20; count++)
        {
            writer.WriteValue(value, default, CultureInfo.InvariantCulture);
        }

        writer.Flush();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .ShouldBe(string.Concat(Enumerable.Repeat(value.ToString(null, CultureInfo.InvariantCulture), 20)));
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
