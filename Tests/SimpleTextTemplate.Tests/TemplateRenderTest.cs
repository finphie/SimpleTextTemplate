#if NET9_0_OR_GREATER
using System.Buffers;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using Shouldly;
using Xunit;

namespace SimpleTextTemplate.Tests;

public sealed class TemplateRenderTest
{
    [Theory]
    [InlineData("{{A}}")]
    [InlineData("{{ A }}")]
    [InlineData("{{  A  }}")]
    public void 識別子_識別子を置換(string input)
    {
        var template = Template.Parse(Encoding.UTF8.GetBytes(input));

        var bufferWriter = new ArrayBufferWriter<byte>();
        var dic = Context.Create();
        dic.TryAdd("A"u8.ToArray(), "Test1"u8.ToArray());

        template.Render(bufferWriter, dic);
        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .ShouldBe("Test1");
    }

    [Theory]
    [InlineData("{{ A }}{{ B }}")]
    [InlineData("{{ AAA }}{{ BBB }}")]
    public void 識別子_識別子_識別子を置換(string input)
    {
        var template = Template.Parse(Encoding.UTF8.GetBytes(input));

        var bufferWriter = new ArrayBufferWriter<byte>();
        var dic = Context.Create();
        dic.TryAdd("A"u8.ToArray(), "Test1"u8.ToArray());
        dic.TryAdd("AAA"u8.ToArray(), "Test1"u8.ToArray());
        dic.TryAdd("B"u8.ToArray(), "Test2"u8.ToArray());
        dic.TryAdd("BBB"u8.ToArray(), "Test2"u8.ToArray());

        template.Render(bufferWriter, dic);
        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .ShouldBe("Test1Test2");
    }

    [Theory]
    [InlineData("z{{A}}z")]
    [InlineData("z{{ A }}z")]
    public void 文字列_識別子_文字列_識別子を置換(string input)
    {
        var template = Template.Parse(Encoding.UTF8.GetBytes(input));

        var bufferWriter = new ArrayBufferWriter<byte>();
        var dic = Context.Create();
        dic.TryAdd("A"u8.ToArray(), "Test1"u8.ToArray());

        template.Render(bufferWriter, dic);
        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .ShouldBe("zTest1z");
    }

    [Theory]
    [InlineData("{{ A }}z{{ B }}")]
    [InlineData("{{ AAA }}z{{ BBB }}")]
    public void 識別子_文字列_識別子_識別子を置換(string input)
    {
        var template = Template.Parse(Encoding.UTF8.GetBytes(input));

        var bufferWriter = new ArrayBufferWriter<byte>();
        var dic = Context.Create();
        dic.TryAdd("A"u8.ToArray(), "Test1"u8.ToArray());
        dic.TryAdd("AAA"u8.ToArray(), "Test1"u8.ToArray());
        dic.TryAdd("B"u8.ToArray(), "Test2"u8.ToArray());
        dic.TryAdd("BBB"u8.ToArray(), "Test2"u8.ToArray());

        template.Render(bufferWriter, dic);
        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .ShouldBe("Test1zTest2");
    }

    [Theory]
    [InlineData("x{{ A }}123{{ B }}x")]
    public void 文字列_識別子_文字列_識別子_文字列_識別子を置換(string input)
    {
        var template = Template.Parse(Encoding.UTF8.GetBytes(input));

        var bufferWriter = new ArrayBufferWriter<byte>();
        var dic = Context.Create();
        dic.TryAdd("A"u8.ToArray(), "Test1"u8.ToArray());
        dic.TryAdd("B"u8.ToArray(), "Test2"u8.ToArray());

        template.Render(bufferWriter, dic);
        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .ShouldBe("xTest1123Test2x");
    }

    [Fact]
    public void Byte配列_識別子を置換()
    {
        var value = "abc"u8.ToArray();

        Execute("{{ A }}"u8, value, "abc");
        Execute("{{ A: }}"u8, value, "abc");
        Execute("{{ A:: }}"u8, value, "abc");
    }

    [Fact]
    public void String_識別子を置換()
    {
        const string Value = "abc";

        Execute("{{ A }}"u8, Value, "abc");
        Execute("{{ A: }}"u8, Value, "abc");
        Execute("{{ A:: }}"u8, Value, "abc");
    }

    [Fact]
    public void Char配列_識別子を置換()
    {
        var value = "abc".ToArray();

        Execute("{{ A }}"u8, value, "abc");
        Execute("{{ A: }}"u8, value, "abc");
        Execute("{{ A:: }}"u8, value, "abc");
    }

    [Fact]
    public void Int32_識別子を置換()
    {
        const int Value = 1234;

        Execute("{{ A }}"u8, Value, "1234");
        Execute("{{ A: }}"u8, Value, "1234");
        Execute("{{ A:: }}"u8, Value, "1234");
        Execute("{{ A:N3 }}"u8, Value, "1,234.000");
        Execute("{{ A:N3:es-ES }}"u8, Value, "1.234,000", CultureInfo.GetCultureInfo("ja-JP", true));
        Execute("{{ A:N3 }}"u8, Value, "1.234,000", CultureInfo.GetCultureInfo("es-ES", true));
    }

    [Fact]
    public void Double_識別子を置換()
    {
        const double Value = 1234.567;

        Execute("{{ A }}"u8, Value, "1234.567");
        Execute("{{ A: }}"u8, Value, "1234.567");
        Execute("{{ A:: }}"u8, Value, "1234.567");
        Execute("{{ A:F2 }}"u8, Value, "1234.57");
        Execute("{{ A:F3:es-ES }}"u8, Value, "1234,567", CultureInfo.GetCultureInfo("ja-JP", true));
        Execute("{{ A:F3 }}"u8, Value, "1234,567", CultureInfo.GetCultureInfo("es-ES", true));
    }

    [Fact]
    public void DateTimeOffset_識別子を置換()
    {
        var value = new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.FromHours(9));

        Execute("{{ A }}"u8, value, "01/01/2000 00:00:00 +09:00");
        Execute("{{ A: }}"u8, value, "01/01/2000 00:00:00 +09:00");
        Execute("{{ A:: }}"u8, value, "01/01/2000 00:00:00 +09:00");
        Execute("{{ A:d }}"u8, value, "01/01/2000");

        if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            Execute("{{ A:D:ja-JP }}"u8, value, "2000年1月1日土曜日", CultureInfo.GetCultureInfo("en-US", true));
        }
        else
        {
            Execute("{{ A:D:ja-JP }}"u8, value, "2000年1月1日 土曜日", CultureInfo.GetCultureInfo("en-US", true));
        }
    }

    static void Execute<T>(ReadOnlySpan<byte> source, T value, string expectedValue, CultureInfo? provider = null)
        where T : notnull
    {
        var template = Template.Parse(source);
        var bufferWriter = new ArrayBufferWriter<byte>();
        var dic = Context.Create();
        dic.TryAdd("A"u8.ToArray(), value);

        template.Render(bufferWriter, dic, provider);
        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .ShouldBe(expectedValue);
    }
}
#endif
