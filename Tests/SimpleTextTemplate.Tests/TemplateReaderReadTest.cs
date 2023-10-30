using System.Text;
using FluentAssertions;
using Xunit;
using static SimpleTextTemplate.BlockType;

namespace SimpleTextTemplate.Tests;

public sealed class TemplateReaderReadTest
{
    [Fact]
    public void 文字列_文字列の範囲を返す()
    {
        Execute("{"u8, (Raw, "{", 1));
        Execute("a"u8, (Raw, "a", 1));
        Execute("ab"u8, (Raw, "ab", 2));
        Execute("abc"u8, (Raw, "abc", 3));
        Execute("{ A }"u8, (Raw, "{ A }", 5));
        Execute("}}"u8, (Raw, "}}", 2));
    }

    [Fact]
    public void 識別子_識別子_識別子の範囲を返す()
    {
        Execute("{{ A }}{{ B }}"u8, (Identifier, "A", 7), (Identifier, "B", 14));
        Execute("{{ AAA }}{{ BBB }}"u8, (Identifier, "AAA", 9), (Identifier, "BBB", 18));
    }

    [Fact]
    public void 文字列_識別子_文字列_文字列または識別子の範囲を返す()
    {
        Execute("z{{A}}z"u8, (Raw, "z", 1), (Identifier, "A", 6), (Raw, "z", 7));
        Execute("z{{ A }}z"u8, (Raw, "z", 1), (Identifier, "A", 8), (Raw, "z", 9));
    }

    [Fact]
    public void 識別子_文字列_識別子_文字列または識別子の範囲を返す()
    {
        Execute("{{ AAA }}z{{ BBB }}"u8, (Identifier, "AAA", 9), (Raw, "z", 10), (Identifier, "BBB", 19));
        Execute("{{ A }}123{{ B }}"u8, (Identifier, "A", 7), (Raw, "123", 10), (Identifier, "B", 17));
    }

    [Fact]
    public void 文字列_識別子_文字列_識別子_文字列_文字列または識別子の範囲を返す()
        => Execute("x{{ A }}123{{ B }}x"u8, (Raw, "x", 1), (Identifier, "A", 8), (Raw, "123", 11), (Identifier, "B", 18), (Raw, "x", 19));

    [Fact]
    public void 末尾_ブロックタイプEndを返す()
    {
        var reader = new TemplateReader([]);

        reader.Read(out var value).Should().Be(End);
        reader.Consumed.Should().Be(0);

        value.ToArray().Should().BeEmpty();
    }

    [Theory]
    [InlineData("{{", 2)]
    [InlineData("{{ ", 3)]
    [InlineData("{{ }", 3)]
    public void 識別子終了タグなし_TemplateException(string input, int position)
    {
        // nuint型とint型との比較の際、例外が発生する。
        // そのため、int型をあらかじめUIntPtr型でキャストしておく。
        var positionPtr = (UIntPtr)position;

        var utf8Input = Encoding.UTF8.GetBytes(input);
        utf8Input.Invoking(static array =>
        {
            var reader = new TemplateReader(array);
            reader.Read(out _);
        })
        .Should()
        .Throw<TemplateException>()
        .Where(x => x.Position == positionPtr);
    }

    static void Execute(ReadOnlySpan<byte> buffer, params (BlockType Type, string ExpectedValue, nuint Consumed)[] blocks)
    {
        var reader = new TemplateReader(buffer);

        foreach (var (type, expectedValue, consumed) in blocks)
        {
            reader.Read(out var value).Should().Be(type);
            reader.Consumed.Should().Be(consumed);

            value.ToArray()
                .Should()
                .Equal(Encoding.UTF8.GetBytes(expectedValue));
        }
    }
}
