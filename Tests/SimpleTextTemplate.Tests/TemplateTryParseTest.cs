using System.Text;
using Shouldly;
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
    public void 文字列_trueを返す(string input) => Execute(input);

    [Theory]
    [InlineData("{{ A }}{{ B }}")]
    [InlineData("{{ AAA }}{{ BBB }}")]
    public void 識別子_識別子_trueを返す(string input) => Execute(input);

    [Theory]
    [InlineData("z{{A}}z")]
    [InlineData("z{{ A }}z")]
    public void 文字列_識別子_文字列_trueを返す(string input) => Execute(input);

    [Theory]
    [InlineData("{{ AAA }}z{{ BBB }}")]
    [InlineData("{{ A }}123{{ B }}")]
    public void 識別子_文字列_識別子_trueを返す(string input) => Execute(input);

    [Theory]
    [InlineData("x{{ A }}123{{ B }}x")]
    public void 文字列_識別子_文字列_識別子_文字列_trueを返す(string input) => Execute(input);

    [Theory]
    [InlineData("{{ 1::ja-JP }}")]
    [InlineData("{{ 1:N:ja-JP }}")]
    public void カルチャー指定あり_trueを返す(string input) => Execute(input);

    [Theory]
    [InlineData("{{ A: }}")]
    [InlineData("{{ A:: }}")]
    [InlineData("{{ A:N: }}")]
    public void 書式指定またはカルチャー指定が空_trueを返す(string input) => Execute(input);

    [Theory]
    [InlineData("{{", 2)]
    [InlineData("{{ ", 3)]
    [InlineData("{{ }", 3)]
    public void 識別子終了タグなし_falseを返す(string input, int expectedConsumed)
    {
        Template.TryParse(Encoding.UTF8.GetBytes(input), out _, out var consumed).ShouldBeFalse();
        consumed.ShouldBe((nuint)expectedConsumed);
    }

    [Theory]
    [InlineData("{{ : }}")]
    [InlineData("{{ :: }}")]
    public void 識別子が空_falseを返す(string input)
    {
        Template.TryParse(Encoding.UTF8.GetBytes(input), out _, out var consumed).ShouldBeFalse();
        consumed.ShouldBe((nuint)input.Length);
    }

    [Fact]
    public void 無効なカルチャー_falseを返す()
    {
        var input = "{{ A::B }}"u8;
        Template.TryParse(input, out _, out var consumed).ShouldBeFalse();
        consumed.ShouldBe((nuint)input.Length);
    }

    static void Execute(string input)
    {
        Template.TryParse(Encoding.UTF8.GetBytes(input), out _, out var consumed).ShouldBeTrue();
        consumed.ShouldBe((nuint)input.Length);
    }
}
