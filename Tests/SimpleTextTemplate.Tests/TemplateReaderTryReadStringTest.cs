using System.Text;
using SimpleTextTemplate.Tests.Assertions;

namespace SimpleTextTemplate.Tests;

public sealed class TemplateReaderTryReadStringTest
{
    [Test]
    [Arguments("{", "{", 1)]
    [Arguments("a", "a", 1)]
    [Arguments("ab", "ab", 2)]
    [Arguments("abc", "abc", 3)]
    [Arguments("{ A }", "{ A }", 5)]
    [Arguments("z{{ A }}z", "z", 1)]
    [Arguments("z{{", "z", 1)]
    public async Task 文字列_文字列の範囲とtrueを返す(string input, string template, int expectedConsumed)
    {
        var utf8Input = Encoding.UTF8.GetBytes(input);
        var reader = new TemplateReader(utf8Input);

        var result = reader.TryReadString(out var value);
        var consumed = reader.Consumed;
        ReadOnlyMemory<byte> memory = value.ToArray();

        await Assert.That(result).IsTrue();
        await Assert.That(consumed).IsEqualTo((nuint)expectedConsumed);
        await Assert.That(memory).IsUtf8SequenceEqualTo(template);
    }

    [Test]
    [Arguments("")]
    [Arguments("{{")]
    public async Task 空または識別子開始タグ_falseを返す(string input)
    {
        var utf8Input = Encoding.UTF8.GetBytes(input);
        var reader = new TemplateReader(utf8Input);

        var result = reader.TryReadString(out var value);
        var consumed = reader.Consumed;
        var length = value.Length;

        await Assert.That(result).IsFalse();
        await Assert.That(consumed).IsEqualTo((nuint)0);
        await Assert.That(length).IsZero();
    }
}
