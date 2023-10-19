using System.Text;
using FluentAssertions;
using Xunit;

namespace SimpleTextTemplate.Tests;

public sealed class TemplateReaderTryReadStringTest
{
    [Theory]
    [InlineData("{", "{", 1)]
    [InlineData("a", "a", 1)]
    [InlineData("ab", "ab", 2)]
    [InlineData("abc", "abc", 3)]
    [InlineData("{ A }", "{ A }", 5)]
    [InlineData("z{{ A }}z", "z", 1)]
    [InlineData("z{{", "z", 1)]
    public void 文字列_文字列の範囲とtrueを返す(string input, string template, int consumed)
    {
        var utf8Input = Encoding.UTF8.GetBytes(input);
        var reader = new TemplateReader(utf8Input);

        reader.TryReadString(out var range).Should().BeTrue();
        reader.Consumed.Should().Be((nuint)consumed);

        utf8Input.AsSpan(range.Start, range.Length).ToArray()
            .Should()
            .Equal(Encoding.UTF8.GetBytes(template));
    }

    [Theory]
    [InlineData("")]
    [InlineData("{{")]
    public void 空または識別子開始タグ_falseを返す(string input)
    {
        var utf8Input = Encoding.UTF8.GetBytes(input);
        var reader = new TemplateReader(utf8Input);

        reader.TryReadString(out var range).Should().BeFalse();
        reader.Consumed.Should().Be(0);

        range.Should().Be(default(TextRange));
    }
}
