using System.Text;
using SimpleTextTemplate.Tests.Assertions;

namespace SimpleTextTemplate.Tests;

public sealed class TemplateIdentifierReaderTryReadTest
{
    [Test]
    [Arguments("A", "A")]
    [Arguments("A B", "A B")]
    [Arguments("A:", "A")]
    [Arguments("A::", "A")]
    public async Task 書式指定及びカルチャー指定なし_識別子名のみ返す(string input, string expectedIdentifier)
    {
        var utf8Input = Encoding.UTF8.GetBytes(input);
        var reader = new TemplateIdentifierReader(utf8Input);

        var result = reader.TryRead(out var value, out var format, out var culture);
        ReadOnlyMemory<byte> memory = value.ToArray();

        await Assert.That(result).IsTrue();
        await Assert.That(memory).IsUtf8SequenceEqualTo(expectedIdentifier);
        await Assert.That(format).IsNull();
        await Assert.That(culture).IsNull();
    }

    [Test]
    [Arguments("A:B", "A", "B")]
    [Arguments("A:BC", "A", "BC")]
    [Arguments("A:B C", "A", "B C")]
    [Arguments("A: B", "A", " B")]
    [Arguments("A B:C", "A B", "C")]
    public async Task 書式指定あり_識別子名と書式指定を返す(string input, string expectedIdentifier, string expectedFormat)
    {
        var utf8Input = Encoding.UTF8.GetBytes(input);
        var reader = new TemplateIdentifierReader(utf8Input);

        var result = reader.TryRead(out var value, out var format, out var culture);
        ReadOnlyMemory<byte> memory = value.ToArray();

        await Assert.That(result).IsTrue();
        await Assert.That(memory).IsUtf8SequenceEqualTo(expectedIdentifier);
        await Assert.That(format).IsEqualTo(expectedFormat);
        await Assert.That(culture).IsNull();
    }

    [Test]
    [Arguments("A::B", "A", "B")]
    [Arguments("A::BC", "A", "BC")]
    [Arguments("A::B C", "A", "B C")]
    [Arguments("A:: B", "A", " B")]
    [Arguments("A:::", "A", ":")]
    [Arguments("A B::C", "A B", "C")]
    public async Task カルチャー指定あり_識別子名とカルチャー指定を返す(string input, string expectedIdentifier, string expectedCulture)
    {
        var utf8Input = Encoding.UTF8.GetBytes(input);
        var reader = new TemplateIdentifierReader(utf8Input);

        var result = reader.TryRead(out var value, out var format, out var culture);
        ReadOnlyMemory<byte> memory = value.ToArray();

        await Assert.That(result).IsTrue();
        await Assert.That(memory).IsUtf8SequenceEqualTo(expectedIdentifier);
        await Assert.That(format).IsNull();
        await Assert.That(culture).IsEqualTo(expectedCulture);
    }

    [Test]
    [Arguments("A:B:C", "A", "B", "C")]
    [Arguments("A:B :C", "A", "B ", "C")]
    [Arguments("A: B:C", "A", " B", "C")]
    [Arguments("A:B: C", "A", "B", " C")]
    [Arguments("A : B : C", "A ", " B ", " C")]
    public async Task 書式指定及びカルチャー指定あり_識別子名と書式指定とカルチャー指定を返す(string input, string expectedIdentifier, string expectedFormat, string expectedCulture)
    {
        var utf8Input = Encoding.UTF8.GetBytes(input);
        var reader = new TemplateIdentifierReader(utf8Input);

        var result = reader.TryRead(out var value, out var format, out var culture);
        ReadOnlyMemory<byte> memory = value.ToArray();

        await Assert.That(result).IsTrue();
        await Assert.That(memory).IsUtf8SequenceEqualTo(expectedIdentifier);
        await Assert.That(format).IsEqualTo(expectedFormat);
        await Assert.That(culture).IsEqualTo(expectedCulture);
    }

    [Test]
    public async Task バイト列先頭がコロン_falseを返す()
    {
        var reader = new TemplateIdentifierReader(":A"u8);
        var result = reader.TryRead(out _, out _, out _);

        await Assert.That(result).IsFalse();
    }
}
