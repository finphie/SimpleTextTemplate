using System.Text;
using SimpleTextTemplate.Tests.Assertions;

namespace SimpleTextTemplate.Tests;

public sealed class TemplateReaderTryReadIdentifierTest
{
    [Test]
    [Arguments("{{A}}", "A", 5)]
    [Arguments("{{AB}}", "AB", 6)]
    [Arguments("{{ A }}", "A", 7)]
    [Arguments("{{   A   }}", "A", 11)]
    [Arguments("{{ ABC }}", "ABC", 9)]
    [Arguments("{{ A B }}", "A B", 9)]
    [Arguments("{{{A}}", "{A", 6)]
    [Arguments("{{{ A}}", "{ A", 7)]
    [Arguments("{{A}}}", "A", 5)]
    [Arguments("{{A }}}", "A", 6)]
    public async Task 識別子_識別子の範囲とtrueを返す(string input, string expectedIdentifier, int expectedConsumed)
    {
        var utf8Input = Encoding.UTF8.GetBytes(input);
        var reader = new TemplateReader(utf8Input);

        var result = reader.TryReadIdentifier(out var value);
        var consumed = reader.Consumed;
        ReadOnlyMemory<byte> memory = value.ToArray();

        await Assert.That(result).IsTrue();
        await Assert.That(consumed).IsEqualTo((nuint)expectedConsumed);
        await Assert.That(memory).IsUtf8SequenceEqualTo(expectedIdentifier);
    }

    [Test]
    [Arguments("{{}}", 2)]
    [Arguments("", 0)]
    [Arguments("{", 0)]
    [Arguments("a", 0)]
    [Arguments("ab", 0)]
    [Arguments("abc", 0)]
    [Arguments("z{{A}}z", 0)]
    [Arguments("{A}", 0)]
    [Arguments("}}", 0)]
    [Arguments("{ A }", 0)]
    [Arguments("{{", 2)]
    [Arguments("{{ ", 3)]
    [Arguments("{{ A", 3)]
    [Arguments("{{{", 2)]
    public async Task 識別子以外_falseを返す(string input, int expectedConsumed)
    {
        var utf8Input = Encoding.UTF8.GetBytes(input);
        var reader = new TemplateReader(utf8Input);

        var result = reader.TryReadIdentifier(out var value);
        var consumed = reader.Consumed;
        var length = value.Length;

        await Assert.That(result).IsFalse();
        await Assert.That(consumed).IsEqualTo((nuint)expectedConsumed);
        await Assert.That(length).IsZero();
    }
}
