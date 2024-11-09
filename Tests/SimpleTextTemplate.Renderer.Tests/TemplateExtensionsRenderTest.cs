using System.Buffers;
using System.Globalization;
using System.Text;
using FluentAssertions;
using SimpleTextTemplate.Contexts;
using Utf8Utility;
using Xunit;

namespace SimpleTextTemplate.Renderer.Tests;

public sealed class TemplateExtensionsRenderTest
{
    [Theory]
    [InlineData("{{A}}")]
    [InlineData("{{ A }}")]
    [InlineData("{{  A  }}")]
    public void 識別子_識別子を置換(string input)
    {
        var template = Template.Parse(Encoding.UTF8.GetBytes(input));

        var bufferWriter = new ArrayBufferWriter<byte>();
        var dic = new Utf8ArrayDictionary<object>();
        dic.TryAdd(new("A"u8), "Test1"u8.ToArray());

        template.Render(bufferWriter, Context.Create(dic));
        bufferWriter.WrittenSpan.ToArray()
            .Should()
            .Equal("Test1"u8.ToArray());
    }

    [Theory]
    [InlineData("{{ A }}{{ B }}")]
    [InlineData("{{ AAA }}{{ BBB }}")]
    public void 識別子_識別子_識別子を置換(string input)
    {
        var template = Template.Parse(Encoding.UTF8.GetBytes(input));

        var bufferWriter = new ArrayBufferWriter<byte>();
        var dic = new Utf8ArrayDictionary<object>();
        dic.TryAdd(new("A"u8), "Test1"u8.ToArray());
        dic.TryAdd(new("AAA"u8), "Test1"u8.ToArray());
        dic.TryAdd(new("B"u8), "Test2"u8.ToArray());
        dic.TryAdd(new("BBB"u8), "Test2"u8.ToArray());

        template.Render(bufferWriter, Context.Create(dic));
        bufferWriter.WrittenSpan.ToArray()
            .Should()
            .Equal("Test1Test2"u8.ToArray());
    }

    [Theory]
    [InlineData("z{{A}}z")]
    [InlineData("z{{ A }}z")]
    public void 文字列_識別子_文字列_識別子を置換(string input)
    {
        var template = Template.Parse(Encoding.UTF8.GetBytes(input));

        var bufferWriter = new ArrayBufferWriter<byte>();
        var dic = new Utf8ArrayDictionary<object>();
        dic.TryAdd(new("A"u8), "Test1"u8.ToArray());

        template.Render(bufferWriter, Context.Create(dic));
        bufferWriter.WrittenSpan.ToArray()
            .Should()
            .Equal("zTest1z"u8.ToArray());
    }

    [Theory]
    [InlineData("{{ A }}z{{ B }}")]
    [InlineData("{{ AAA }}z{{ BBB }}")]
    public void 識別子_文字列_識別子_識別子を置換(string input)
    {
        var template = Template.Parse(Encoding.UTF8.GetBytes(input));

        var bufferWriter = new ArrayBufferWriter<byte>();
        var dic = new Utf8ArrayDictionary<object>();
        dic.TryAdd(new("A"u8), "Test1"u8.ToArray());
        dic.TryAdd(new("AAA"u8), "Test1"u8.ToArray());
        dic.TryAdd(new("B"u8), "Test2"u8.ToArray());
        dic.TryAdd(new("BBB"u8), "Test2"u8.ToArray());

        template.Render(bufferWriter, Context.Create(dic));
        bufferWriter.WrittenSpan.ToArray()
            .Should()
            .Equal("Test1zTest2"u8.ToArray());
    }

    [Theory]
    [InlineData("x{{ A }}123{{ B }}x")]
    public void 文字列_識別子_文字列_識別子_文字列_識別子を置換(string input)
    {
        var template = Template.Parse(Encoding.UTF8.GetBytes(input));

        var bufferWriter = new ArrayBufferWriter<byte>();
        var dic = new Utf8ArrayDictionary<object>();
        dic.TryAdd(new("A"u8), "Test1"u8.ToArray());
        dic.TryAdd(new("B"u8), "Test2"u8.ToArray());

        template.Render(bufferWriter, Context.Create(dic));
        bufferWriter.WrittenSpan.ToArray()
            .Should()
            .Equal("xTest1123Test2x"u8.ToArray());
    }

    [Fact]
    public void Utf8Array_識別子を置換()
    {
        var value = new Utf8Array("abc"u8);

        Execute("{{ A }}"u8, value, "abc"u8);
        Execute("{{ A: }}"u8, value, "abc"u8);
        Execute("{{ A:: }}"u8, value, "abc"u8);
    }

    [Fact]
    public void Byte配列_識別子を置換()
    {
        var value = "abc"u8.ToArray();

        Execute("{{ A }}"u8, value, "abc"u8);
        Execute("{{ A: }}"u8, value, "abc"u8);
        Execute("{{ A:: }}"u8, value, "abc"u8);
    }

    [Fact]
    public void String_識別子を置換()
    {
        const string Value = "abc";

        Execute("{{ A }}"u8, Value, "abc"u8);
        Execute("{{ A: }}"u8, Value, "abc"u8);
        Execute("{{ A:: }}"u8, Value, "abc"u8);
    }

    [Fact]
    public void Char配列_識別子を置換()
    {
        var value = "abc".ToArray();

        Execute("{{ A }}"u8, value, "abc"u8);
        Execute("{{ A: }}"u8, value, "abc"u8);
        Execute("{{ A:: }}"u8, value, "abc"u8);
    }

    [Fact]
    public void Int32_識別子を置換()
    {
        const int Value = 1234;

        Execute("{{ A }}"u8, Value, "1234"u8);
        Execute("{{ A: }}"u8, Value, "1234"u8);
        Execute("{{ A:: }}"u8, Value, "1234"u8);
        Execute("{{ A:N3 }}"u8, Value, "1,234.000"u8);
        Execute("{{ A:N3:es-ES }}"u8, Value, "1.234,000"u8, CultureInfo.GetCultureInfo("ja-JP", true));
        Execute("{{ A:N3 }}"u8, Value, "1.234,000"u8, CultureInfo.GetCultureInfo("es-ES", true));
    }

    [Fact]
    public void Double_識別子を置換()
    {
        const double Value = 1234.567;

        Execute("{{ A }}"u8, Value, "1234.567"u8);
        Execute("{{ A: }}"u8, Value, "1234.567"u8);
        Execute("{{ A:: }}"u8, Value, "1234.567"u8);
        Execute("{{ A:F2 }}"u8, Value, "1234.57"u8);
        Execute("{{ A:F3:es-ES }}"u8, Value, "1234,567"u8, CultureInfo.GetCultureInfo("ja-JP", true));
        Execute("{{ A:F3 }}"u8, Value, "1234,567"u8, CultureInfo.GetCultureInfo("es-ES", true));
    }

    [Fact]
    public void DateTimeOffset_識別子を置換()
    {
        var value = new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.FromHours(9));

        Execute("{{ A }}"u8, value, "01/01/2000 00:00:00 +09:00"u8);
        Execute("{{ A: }}"u8, value, "01/01/2000 00:00:00 +09:00"u8);
        Execute("{{ A:: }}"u8, value, "01/01/2000 00:00:00 +09:00"u8);
        Execute("{{ A:d }}"u8, value, "01/01/2000"u8);
        Execute("{{ A:D:ja-JP }}"u8, value, "2000年1月1日土曜日"u8, CultureInfo.GetCultureInfo("en-US", true));
    }

    static void Execute<T>(ReadOnlySpan<byte> source, T value, ReadOnlySpan<byte> expectedValue, CultureInfo? provider = null)
        where T : notnull
    {
        var template = Template.Parse(source);
        var bufferWriter = new ArrayBufferWriter<byte>();
        var dic = new Utf8ArrayDictionary<object>();
        dic.TryAdd(new("A"u8), value);

        template.Render(bufferWriter, Context.Create(dic), provider);
        bufferWriter.WrittenSpan.ToArray()
            .Should()
            .Equal(expectedValue.ToArray());
    }
}
