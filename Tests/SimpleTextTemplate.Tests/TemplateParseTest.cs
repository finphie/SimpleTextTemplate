﻿using System.Text;
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
    [InlineData("{{", 2)]
    [InlineData("{{ ", 3)]
    [InlineData("{{ }", 3)]
    public void 識別子終了タグなし_falseを返す(string input, int position)
    {
        // nuint型とint型との比較の際、例外が発生する。
        // そのため、int型をあらかじめUIntPtr型でキャストしておく。
        var positionPtr = (UIntPtr)position;

        Encoding.UTF8.GetBytes(input).Invoking(Template.Parse)
            .Should()
            .Throw<TemplateException>()
            .Where(x => x.Position == positionPtr);
    }

    static void Execute(string input) => Template.Parse(Encoding.UTF8.GetBytes(input));
}
