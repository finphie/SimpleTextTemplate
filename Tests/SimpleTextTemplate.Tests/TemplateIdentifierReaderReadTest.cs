using System.Text;
using FluentAssertions;
using Xunit;

namespace SimpleTextTemplate.Tests;

public sealed class TemplateIdentifierReaderReadTest
{
    [Theory]
    [InlineData("A", "A")]
    [InlineData("A B", "A B")]
    [InlineData("A:", "A")]
    [InlineData("A::", "A")]
    [InlineData("A:: ", "A")]
    public void 書式指定及びカルチャー指定なし_識別子名のみ返す(string input, string expectedIdentifier)
    {
        var utf8Input = Encoding.UTF8.GetBytes(input);
        var reader = new TemplateIdentifierReader(utf8Input);

        reader.Read(out var value, out var format, out var culture);

        value.ToArray().Should().Equal(Encoding.UTF8.GetBytes(expectedIdentifier));
        format.Should().BeNull();
        culture.Should().BeNull();
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

        reader.Read(out var value, out var format, out var culture);

        value.ToArray().Should().Equal(Encoding.UTF8.GetBytes(expectedIdentifier));
        format.Should().Be(expectedFormat);
        culture.Should().BeNull();
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

        reader.Read(out var value, out var format, out var culture);

        value.ToArray().Should().Equal(Encoding.UTF8.GetBytes(expectedIdentifier));
        format.Should().BeNull();
        culture.Should().Be(expectedCulture);
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

        reader.Read(out var value, out var format, out var culture);

        value.ToArray().Should().Equal(Encoding.UTF8.GetBytes(expectedIdentifier));
        format.Should().Be(expectedFormat);
        culture.Should().Be(expectedCulture);
    }

    [Fact]
    public void バイト列先頭がコロン_TemplateException()
    {
        FluentActions.Invoking(() =>
        {
            var reader = new TemplateIdentifierReader(":A"u8);
            reader.Read(out _, out _, out _);
        })
        .Should()
        .Throw<TemplateException>();
    }
}
