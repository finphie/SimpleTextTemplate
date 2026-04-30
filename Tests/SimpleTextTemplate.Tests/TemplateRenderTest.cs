using System.Buffers;
using System.Globalization;
using System.Text;
using SimpleTextTemplate.Tests.Assertions;

namespace SimpleTextTemplate.Tests;

public sealed class TemplateRenderTest
{
    [Test]
    [Arguments("{{A}}")]
    [Arguments("{{ A }}")]
    [Arguments("{{  A  }}")]
    public async Task 識別子_識別子を置換(string input)
    {
        var template = Template.Parse(Encoding.UTF8.GetBytes(input));

        var bufferWriter = new ArrayBufferWriter<byte>();
        var dic = Context.Create();
        dic.TryAdd("A"u8.ToArray(), "Test1"u8.ToArray());

        template.Render(bufferWriter, dic);
        await Assert.That(bufferWriter.WrittenMemory).IsUtf8SequenceEqualTo("Test1");
    }

    [Test]
    [Arguments("{{ A }}{{ B }}")]
    [Arguments("{{ AAA }}{{ BBB }}")]
    public async Task 識別子_識別子_識別子を置換(string input)
    {
        var template = Template.Parse(Encoding.UTF8.GetBytes(input));

        var bufferWriter = new ArrayBufferWriter<byte>();
        var dic = Context.Create();
        dic.TryAdd("A"u8.ToArray(), "Test1"u8.ToArray());
        dic.TryAdd("AAA"u8.ToArray(), "Test1"u8.ToArray());
        dic.TryAdd("B"u8.ToArray(), "Test2"u8.ToArray());
        dic.TryAdd("BBB"u8.ToArray(), "Test2"u8.ToArray());

        template.Render(bufferWriter, dic);
        await Assert.That(bufferWriter.WrittenMemory).IsUtf8SequenceEqualTo("Test1Test2");
    }

    [Test]
    [Arguments("z{{A}}z")]
    [Arguments("z{{ A }}z")]
    public async Task 文字列_識別子_文字列_識別子を置換(string input)
    {
        var template = Template.Parse(Encoding.UTF8.GetBytes(input));

        var bufferWriter = new ArrayBufferWriter<byte>();
        var dic = Context.Create();
        dic.TryAdd("A"u8.ToArray(), "Test1"u8.ToArray());

        template.Render(bufferWriter, dic);
        await Assert.That(bufferWriter.WrittenMemory).IsUtf8SequenceEqualTo("zTest1z");
    }

    [Test]
    [Arguments("{{ A }}z{{ B }}")]
    [Arguments("{{ AAA }}z{{ BBB }}")]
    public async Task 識別子_文字列_識別子_識別子を置換(string input)
    {
        var template = Template.Parse(Encoding.UTF8.GetBytes(input));

        var bufferWriter = new ArrayBufferWriter<byte>();
        var dic = Context.Create();
        dic.TryAdd("A"u8.ToArray(), "Test1"u8.ToArray());
        dic.TryAdd("AAA"u8.ToArray(), "Test1"u8.ToArray());
        dic.TryAdd("B"u8.ToArray(), "Test2"u8.ToArray());
        dic.TryAdd("BBB"u8.ToArray(), "Test2"u8.ToArray());

        template.Render(bufferWriter, dic);
        await Assert.That(bufferWriter.WrittenMemory).IsUtf8SequenceEqualTo("Test1zTest2");
    }

    [Test]
    [Arguments("x{{ A }}123{{ B }}x")]
    public async Task 文字列_識別子_文字列_識別子_文字列_識別子を置換(string input)
    {
        var template = Template.Parse(Encoding.UTF8.GetBytes(input));

        var bufferWriter = new ArrayBufferWriter<byte>();
        var dic = Context.Create();
        dic.TryAdd("A"u8.ToArray(), "Test1"u8.ToArray());
        dic.TryAdd("B"u8.ToArray(), "Test2"u8.ToArray());

        template.Render(bufferWriter, dic);
        await Assert.That(bufferWriter.WrittenMemory).IsUtf8SequenceEqualTo("xTest1123Test2x");
    }

    [Test]
    public async Task Byte配列_識別子を置換()
    {
        var value = "abc"u8.ToArray();

        await Execute("{{ A }}"u8.ToArray(), value, "abc");
        await Execute("{{ A: }}"u8.ToArray(), value, "abc");
        await Execute("{{ A:: }}"u8.ToArray(), value, "abc");
    }

    [Test]
    public async Task String_識別子を置換()
    {
        const string Value = "abc";

        await Execute("{{ A }}"u8.ToArray(), Value, "abc");
        await Execute("{{ A: }}"u8.ToArray(), Value, "abc");
        await Execute("{{ A:: }}"u8.ToArray(), Value, "abc");
    }

    [Test]
    public async Task Char配列_識別子を置換()
    {
        var value = "abc".ToArray();

        await Execute("{{ A }}"u8.ToArray(), value, "abc");
        await Execute("{{ A: }}"u8.ToArray(), value, "abc");
        await Execute("{{ A:: }}"u8.ToArray(), value, "abc");
    }

    [Test]
    public async Task Int32_識別子を置換()
    {
        const int Value = 1234;

        await Execute("{{ A }}"u8.ToArray(), Value, "1234");
        await Execute("{{ A: }}"u8.ToArray(), Value, "1234");
        await Execute("{{ A:: }}"u8.ToArray(), Value, "1234");
        await Execute("{{ A:N3 }}"u8.ToArray(), Value, "1,234.000");
        await Execute("{{ A:N3:es-ES }}"u8.ToArray(), Value, "1.234,000", CultureInfo.GetCultureInfo("ja-JP", true));
        await Execute("{{ A:N3 }}"u8.ToArray(), Value, "1.234,000", CultureInfo.GetCultureInfo("es-ES", true));
    }

    [Test]
    public async Task Double_識別子を置換()
    {
        const double Value = 1234.567;

        await Execute("{{ A }}"u8.ToArray(), Value, "1234.567");
        await Execute("{{ A: }}"u8.ToArray(), Value, "1234.567");
        await Execute("{{ A:: }}"u8.ToArray(), Value, "1234.567");
        await Execute("{{ A:F2 }}"u8.ToArray(), Value, "1234.57");
        await Execute("{{ A:F3:es-ES }}"u8.ToArray(), Value, "1234,567", CultureInfo.GetCultureInfo("ja-JP", true));
        await Execute("{{ A:F3 }}"u8.ToArray(), Value, "1234,567", CultureInfo.GetCultureInfo("es-ES", true));
    }

    [Test]
    public async Task DateTimeOffset_識別子を置換()
    {
        var value = new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.FromHours(9));

        await Execute("{{ A }}"u8.ToArray(), value, "01/01/2000 00:00:00 +09:00");
        await Execute("{{ A: }}"u8.ToArray(), value, "01/01/2000 00:00:00 +09:00");
        await Execute("{{ A:: }}"u8.ToArray(), value, "01/01/2000 00:00:00 +09:00");
        await Execute("{{ A:d }}"u8.ToArray(), value, "01/01/2000");

        var expected = !OperatingSystem.IsMacOS()
            ? "2000年1月1日土曜日"
            : "2000年1月1日 土曜日";
        await Execute("{{ A:D:ja-JP }}"u8.ToArray(), value, expected, CultureInfo.GetCultureInfo("en-US", true));
    }

    static async Task Execute<T>(ReadOnlyMemory<byte> source, T value, string expectedValue, CultureInfo? provider = null)
        where T : notnull
    {
        var template = Template.Parse(source.Span);
        var bufferWriter = new ArrayBufferWriter<byte>();
        var dic = Context.Create();
        dic.TryAdd("A"u8.ToArray(), value);

        template.Render(bufferWriter, dic, provider);
        await Assert.That(bufferWriter.WrittenMemory)
            .IsUtf8SequenceEqualTo(expectedValue);
    }
}
