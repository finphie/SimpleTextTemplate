using System.Buffers;
using System.Globalization;
using SimpleTextTemplate.Tests.Assertions;

namespace SimpleTextTemplate.Writer.Tests;

public sealed class TemplateWriterWriteValueTest
{
    [Test]
    public async Task Int32_バッファーライターに書き込み()
    {
        const int Value = 1234;
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        writer.WriteValue(Value, default, CultureInfo.InvariantCulture);
        writer.WriteValue(Value, "N3", CultureInfo.InvariantCulture);
        writer.WriteValue(Value, "N3", CultureInfo.GetCultureInfo("es-ES", true));
        writer.Flush();

        await Assert.That(bufferWriter.WrittenMemory)
            .IsUtf8SequenceEqualTo("12341,234.0001.234,000");
    }

    [Test]
    public async Task Double_バッファーライターに書き込み()
    {
        const double Value = 1234.567;
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        writer.WriteValue(Value, default, CultureInfo.InvariantCulture);
        writer.WriteValue(Value, "F2", CultureInfo.InvariantCulture);
        writer.WriteValue(Value, "F3", CultureInfo.GetCultureInfo("es-ES", true));
        writer.Flush();

        await Assert.That(bufferWriter.WrittenMemory)
            .IsUtf8SequenceEqualTo("1234.5671234.571234,567");
    }

    [Test]
    public async Task DateTimeOffset_バッファーライターに書き込み()
    {
        var value = new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.FromHours(9));
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        writer.WriteValue(value, default, CultureInfo.InvariantCulture);
        writer.WriteValue(value, "d", CultureInfo.InvariantCulture);
        writer.WriteValue(value, "D", CultureInfo.GetCultureInfo("ja-JP", true));
        writer.Flush();

        var expected = !OperatingSystem.IsMacOS()
            ? "01/01/2000 00:00:00 +09:0001/01/20002000年1月1日土曜日"
            : "01/01/2000 00:00:00 +09:0001/01/20002000年1月1日 土曜日";

        await Assert.That(bufferWriter.WrittenMemory)
            .IsUtf8SequenceEqualTo(expected);
    }

    [Test]
    public async Task IUtf8SpanFormattable_バッファーライターに書き込み()
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

        await Assert.That(bufferWriter.WrittenMemory)
            .Count()
            .IsEqualTo(20 * count)
            .And
            .All(static x => x == (byte)'1');
    }

    [Test]
    public async Task ISpanFormattable_バッファーライターに書き込み()
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

        await Assert.That(bufferWriter.WrittenMemory)
            .IsUtf8SequenceEqualTo(string.Concat(Enumerable.Repeat(value.ToString(null, CultureInfo.InvariantCulture), 20)));
    }

    [Test]
    public async Task IFormattable_バッファーライターに書き込み()
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

        await Assert.That(bufferWriter.WrittenMemory)
            .IsUtf8SequenceEqualTo(string.Concat(Enumerable.Repeat(value.ToString(null, CultureInfo.InvariantCulture), 20)));
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
