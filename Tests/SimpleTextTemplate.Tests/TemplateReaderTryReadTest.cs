using System.Text;
using SimpleTextTemplate.Tests.Assertions;
using static SimpleTextTemplate.BlockType;

namespace SimpleTextTemplate.Tests;

public sealed class TemplateReaderTryReadTest
{
    [Test]
    public async Task 文字列_文字列の範囲を返す()
    {
        await Execute("{"u8.ToArray(), (Raw, "{", 1));
        await Execute("a"u8.ToArray(), (Raw, "a", 1));
        await Execute("ab"u8.ToArray(), (Raw, "ab", 2));
        await Execute("abc"u8.ToArray(), (Raw, "abc", 3));
        await Execute("{ A }"u8.ToArray(), (Raw, "{ A }", 5));
        await Execute("}}"u8.ToArray(), (Raw, "}}", 2));
    }

    [Test]
    public async Task 識別子_識別子_識別子の範囲を返す()
    {
        await Execute("{{ A }}{{ B }}"u8.ToArray(), (Identifier, "A", 7), (Identifier, "B", 14));
        await Execute("{{ AAA }}{{ BBB }}"u8.ToArray(), (Identifier, "AAA", 9), (Identifier, "BBB", 18));
    }

    [Test]
    public async Task 文字列_識別子_文字列_文字列または識別子の範囲を返す()
    {
        await Execute("z{{A}}z"u8.ToArray(), (Raw, "z", 1), (Identifier, "A", 6), (Raw, "z", 7));
        await Execute("z{{ A }}z"u8.ToArray(), (Raw, "z", 1), (Identifier, "A", 8), (Raw, "z", 9));
    }

    [Test]
    public async Task 識別子_文字列_識別子_文字列または識別子の範囲を返す()
    {
        await Execute("{{ AAA }}z{{ BBB }}"u8.ToArray(), (Identifier, "AAA", 9), (Raw, "z", 10), (Identifier, "BBB", 19));
        await Execute("{{ A }}123{{ B }}"u8.ToArray(), (Identifier, "A", 7), (Raw, "123", 10), (Identifier, "B", 17));
    }

    [Test]
    public Task 文字列_識別子_文字列_識別子_文字列_文字列または識別子の範囲を返す()
        => Execute("x{{ A }}123{{ B }}x"u8.ToArray(), (Raw, "x", 1), (Identifier, "A", 8), (Raw, "123", 11), (Identifier, "B", 18), (Raw, "x", 19));

    [Test]
    public async Task 末尾_ブロックタイプEndとfalseを返す()
    {
        var reader = new TemplateReader([]);

        var result = reader.TryRead(out var value);
        var consumed = reader.Consumed;
        var length = value.Length;

        await Assert.That(result).IsEqualTo(End);
        await Assert.That(consumed).IsEqualTo((nuint)0);
        await Assert.That(length).IsZero();
    }

    [Test]
    [Arguments("{{", 2)]
    [Arguments("{{ ", 3)]
    [Arguments("{{ }", 3)]
    public async Task 識別子終了タグなし_ブロックタイプNoneとfalseを返す(string input, int expectedConsumed)
    {
        var utf8Input = Encoding.UTF8.GetBytes(input);
        var reader = new TemplateReader(utf8Input);

        var result = reader.TryRead(out var value);
        var consumed = reader.Consumed;
        var length = value.Length;

        await Assert.That(result).IsEqualTo(None);
        await Assert.That(consumed).IsEqualTo((nuint)expectedConsumed);
        await Assert.That(length).IsZero();
    }

    static async Task Execute(ReadOnlyMemory<byte> buffer, params (BlockType Type, string ExpectedValue, nuint Consumed)[] blocks)
    {
        foreach (var (actual, expected) in Get(buffer, blocks.Length).Zip(blocks))
        {
            var (result, memory, consumed) = actual;
            var (type, expectedValue, expectedConsumed) = expected;

            await Assert.That(result).IsEqualTo(type);
            await Assert.That(consumed).IsEqualTo(expectedConsumed);
            await Assert.That(memory).IsUtf8SequenceEqualTo(expectedValue);
        }

        static IEnumerable<(BlockType, ReadOnlyMemory<byte>, nuint)> Get(ReadOnlyMemory<byte> buffer, int count)
        {
            var reader = new TemplateReader(buffer.Span);
            var list = new List<(BlockType, ReadOnlyMemory<byte>, nuint)>();

            for (var i = 0; i < count; i++)
            {
                var result = reader.TryRead(out var value);
                list.Add((result, value.ToArray(), reader.Consumed));
            }

            return list;
        }
    }
}
