using System.Text;
using FluentAssertions;
using Xunit;
using static SimpleTextTemplate.BlockType;

namespace SimpleTextTemplate.Tests;

public sealed class TemplateReaderReadTest
{
    [Fact]
    public void テンプレート文字列_文字列の範囲を返す()
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
    public void テンプレート文字列_識別子_テンプレート文字列_テンプレート文字列または識別子の範囲を返す()
    {
        Execute("z{{A}}z"u8, (Raw, "z", 1), (Identifier, "A", 6), (Raw, "z", 7));
        Execute("z{{ A }}z"u8, (Raw, "z", 1), (Identifier, "A", 8), (Raw, "z", 9));
    }

    [Fact]
    public void 識別子_テンプレート文字列_識別子_テンプレート文字列または識別子の範囲を返す()
    {
        Execute("{{ AAA }}z{{ BBB }}"u8, (Identifier, "AAA", 9), (Raw, "z", 10), (Identifier, "BBB", 19));
        Execute("{{ A }}123{{ B }}"u8, (Identifier, "A", 7), (Raw, "123", 10), (Identifier, "B", 17));
    }

    [Fact]
    public void テンプレート文字列_識別子_テンプレート文字列_識別子_テンプレート文字列_テンプレート文字列または識別子の範囲を返す()
        => Execute("x{{ A }}123{{ B }}x"u8, (Raw, "x", 1), (Identifier, "A", 8), (Raw, "123", 11), (Identifier, "B", 18), (Raw, "x", 19));

    static void Execute(ReadOnlySpan<byte> buffer, params (BlockType Type, string Value, nuint Consumed)[] blocks)
    {
        var reader = new TemplateReader(buffer);

        foreach (var (type, value, consumed) in blocks)
        {
            reader.Read(out var range).Should().Be(type);
            reader.Consumed.Should().Be(consumed);

            buffer.Slice(range.Start, range.Length).ToArray()
               .Should()
               .Equal(Encoding.UTF8.GetBytes(value));
        }
    }
}
