using System.Text;
using FluentAssertions;
using Xunit;

namespace SimpleTextTemplate.Tests;

public sealed class TemplateTryParseTest
{
    [Theory]
    [InlineData("")]
    [InlineData("{")]
    [InlineData("a")]
    [InlineData("ab")]
    [InlineData("abc")]
    [InlineData("{ A }")]
    [InlineData("}}")]
    public void 文字列_trueを返す(string input) => Execute(input, input.Length);

    [Theory]
    [InlineData("{{ A }}{{ B }}", 14)]
    [InlineData("{{ AAA }}{{ BBB }}", 18)]
    public void 識別子_識別子_trueを返す(string input, int consumed) => Execute(input, consumed);

    [Theory]
    [InlineData("z{{A}}z", 7)]
    [InlineData("z{{ A }}z", 9)]
    public void 文字列_識別子_文字列_trueを返す(string input, int consumed) => Execute(input, consumed);

    [Theory]
    [InlineData("{{ AAA }}z{{ BBB }}", 19)]
    [InlineData("{{ A }}123{{ B }}", 17)]
    public void 識別子_文字列_識別子_trueを返す(string input, int consumed) => Execute(input, consumed);

    [Theory]
    [InlineData("x{{ A }}123{{ B }}x", 19)]
    public void 文字列_識別子_文字列_識別子_文字列_trueを返す(string input, int consumed) => Execute(input, consumed);

    [Theory]
    [InlineData("{{", 2)]
    [InlineData("{{ ", 3)]
    [InlineData("{{ }", 3)]
    public void 識別子終了タグなし_falseを返す(string input, int expectedConsumed)
    {
        Template.TryParse(Encoding.UTF8.GetBytes(input), out _, out var consumed).Should().BeFalse();
        ((int)consumed).Should().Be(expectedConsumed);
    }

    static void Execute(string input, int expectedConsumed)
    {
        Template.TryParse(Encoding.UTF8.GetBytes(input), out _, out var consumed).Should().BeTrue();
        ((int)consumed).Should().Be(expectedConsumed);
    }
}
