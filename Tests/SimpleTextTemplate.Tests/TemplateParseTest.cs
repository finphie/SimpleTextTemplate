using System.Globalization;
using System.Text;
using FluentAssertions;
using Xunit;

namespace SimpleTextTemplate.Tests;

public sealed class TemplateParseTest
{
    [Theory]
    [InlineData("")]
    [InlineData("{")]
    [InlineData("a")]
    [InlineData("ab")]
    [InlineData("abc")]
    [InlineData("{ A }")]
    [InlineData("}}")]
    public void 文字列_Templateを返す(string input) => Execute(input);

    [Theory]
    [InlineData("{{ A }}{{ B }}")]
    [InlineData("{{ AAA }}{{ BBB }}")]
    public void 識別子_識別子_Templateを返す(string input) => Execute(input);

    [Theory]
    [InlineData("z{{A}}z")]
    [InlineData("z{{ A }}z")]
    public void 文字列_識別子_文字列_Templateを返す(string input) => Execute(input);

    [Theory]
    [InlineData("{{ AAA }}z{{ BBB }}")]
    [InlineData("{{ A }}123{{ B }}")]
    public void 識別子_文字列_識別子_Templateを返す(string input) => Execute(input);

    [Theory]
    [InlineData("x{{ A }}123{{ B }}x")]
    public void 文字列_識別子_文字列_識別子_文字列_Templateを返す(string input) => Execute(input);

    [Theory]
    [InlineData("{{ 1:N }}")]
    public void 書式指定あり_Templateを返す(string input) => Execute(input);

    [Theory]
    [InlineData("{{ 1::ja-JP }}")]
    [InlineData("{{ 1:N:ja-JP }}")]
    public void カルチャー指定あり_Templateを返す(string input) => Execute(input);

    [Theory]
    [InlineData("{{ A: }}")]
    [InlineData("{{ A:: }}")]
    [InlineData("{{ A:N: }}")]
    public void 書式指定またはカルチャー指定が空_Templateを返す(string input) => Execute(input);

    [Theory]
    [InlineData("{{", 2)]
    [InlineData("{{ ", 3)]
    [InlineData("{{ }", 3)]
    public void 識別子終了タグなし_TemplateException(string input, int position)
    {
        // nuint型とint型との比較の際、例外が発生する。
        // そのため、int型をあらかじめUIntPtr型でキャストしておく。
        var positionPtr = (UIntPtr)position;

        Encoding.UTF8.GetBytes(input).Invoking(static x => Template.Parse(x))
            .Should()
            .Throw<TemplateException>()
            .Where(x => x.Position == positionPtr);
    }

    [Theory]
    [InlineData("{{ : }}")]
    [InlineData("{{ :: }}")]
    public void 識別子が空_TemplateException(string input)
    {
        Encoding.UTF8.GetBytes(input).Invoking(static x => Template.Parse(x))
            .Should()
            .Throw<TemplateException>();
    }

    [Fact]
    public void 無効なカルチャー_TemplateException()
    {
        FluentActions.Invoking(() => Template.Parse("{{ A::B }}"u8))
            .Should()
            .Throw<CultureNotFoundException>();
    }

    static void Execute(string input) => Template.Parse(Encoding.UTF8.GetBytes(input));
}
