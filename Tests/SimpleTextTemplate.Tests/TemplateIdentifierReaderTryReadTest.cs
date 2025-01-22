using System.Text;
using Shouldly;
using Xunit;

namespace SimpleTextTemplate.Tests;

public sealed class TemplateIdentifierReaderTryReadTest
{
    [Theory]
    [InlineData("A", "A")]
    [InlineData("A B", "A B")]
    [InlineData("A:", "A")]
    [InlineData("A::", "A")]
    public void 書式指定及びカルチャー指定なし_識別子名のみ返す(string input, string expectedIdentifier)
    {
        var utf8Input = Encoding.UTF8.GetBytes(input);
        var reader = new TemplateIdentifierReader(utf8Input);

        reader.TryRead(out var value, out var format, out var culture).ShouldBeTrue();

        Encoding.UTF8.GetString(value).ShouldBe(expectedIdentifier);
        format.ShouldBeNull();
        culture.ShouldBeNull();
    }

    [Theory]
    [InlineData("A:B", "A", "B")]
    [InlineData("A:BC", "A", "BC")]
    [InlineData("A:B C", "A", "B C")]
    [InlineData("A: B", "A", " B")]
    [InlineData("A B:C", "A B", "C")]
    public void 書式指定あり_識別子名と書式指定を返す(string input, string expectedIdentifier, string expectedFormat)
    {
        var utf8Input = Encoding.UTF8.GetBytes(input);
        var reader = new TemplateIdentifierReader(utf8Input);

        reader.TryRead(out var value, out var format, out var culture).ShouldBeTrue();

        Encoding.UTF8.GetString(value).ShouldBe(expectedIdentifier);
        format.ShouldBe(expectedFormat);
        culture.ShouldBeNull();
    }

    [Theory]
    [InlineData("A::B", "A", "B")]
    [InlineData("A::BC", "A", "BC")]
    [InlineData("A::B C", "A", "B C")]
    [InlineData("A:: B", "A", " B")]
    [InlineData("A:::", "A", ":")]
    [InlineData("A B::C", "A B", "C")]
    public void カルチャー指定あり_識別子名とカルチャー指定を返す(string input, string expectedIdentifier, string expectedCulture)
    {
        var utf8Input = Encoding.UTF8.GetBytes(input);
        var reader = new TemplateIdentifierReader(utf8Input);

        reader.TryRead(out var value, out var format, out var culture).ShouldBeTrue();

        Encoding.UTF8.GetString(value).ShouldBe(expectedIdentifier);
        format.ShouldBeNull();
        culture.ShouldBe(expectedCulture);
    }

    [Theory]
    [InlineData("A:B:C", "A", "B", "C")]
    [InlineData("A:B :C", "A", "B ", "C")]
    [InlineData("A: B:C", "A", " B", "C")]
    [InlineData("A:B: C", "A", "B", " C")]
    [InlineData("A : B : C", "A ", " B ", " C")]
    public void 書式指定及びカルチャー指定あり_識別子名と書式指定とカルチャー指定を返す(string input, string expectedIdentifier, string expectedFormat, string expectedCulture)
    {
        var utf8Input = Encoding.UTF8.GetBytes(input);
        var reader = new TemplateIdentifierReader(utf8Input);

        reader.TryRead(out var value, out var format, out var culture).ShouldBeTrue();

        Encoding.UTF8.GetString(value).ShouldBe(expectedIdentifier);
        format.ShouldBe(expectedFormat);
        culture.ShouldBe(expectedCulture);
    }

    [Fact]
    public void バイト列先頭がコロン_TemplateException()
    {
        var reader = new TemplateIdentifierReader(":A"u8);
        reader.TryRead(out _, out _, out _).ShouldBeFalse();
    }
}
