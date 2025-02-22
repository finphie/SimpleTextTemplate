﻿using System.Text;
using Shouldly;
using Xunit;

namespace SimpleTextTemplate.Tests;

public sealed class TemplateReaderTryReadIdentifierTest
{
    [Theory]
    [InlineData("{{A}}", "A", 5)]
    [InlineData("{{AB}}", "AB", 6)]
    [InlineData("{{ A }}", "A", 7)]
    [InlineData("{{   A   }}", "A", 11)]
    [InlineData("{{ ABC }}", "ABC", 9)]
    [InlineData("{{ A B }}", "A B", 9)]
    [InlineData("{{{A}}", "{A", 6)]
    [InlineData("{{{ A}}", "{ A", 7)]
    [InlineData("{{A}}}", "A", 5)]
    [InlineData("{{A }}}", "A", 6)]
    public void 識別子_識別子の範囲とtrueを返す(string input, string identifier, int consumed)
    {
        var utf8Input = Encoding.UTF8.GetBytes(input);
        var reader = new TemplateReader(utf8Input);

        reader.TryReadIdentifier(out var value).ShouldBeTrue();
        reader.Consumed.ShouldBe((nuint)consumed);

        Encoding.UTF8.GetString(value)
            .ShouldBe(identifier);
    }

    [Theory]
    [InlineData("{{}}", 2)]
    [InlineData("", 0)]
    [InlineData("{", 0)]
    [InlineData("a", 0)]
    [InlineData("ab", 0)]
    [InlineData("abc", 0)]
    [InlineData("z{{A}}z", 0)]
    [InlineData("{A}", 0)]
    [InlineData("}}", 0)]
    [InlineData("{ A }", 0)]
    [InlineData("{{", 2)]
    [InlineData("{{ ", 3)]
    [InlineData("{{ A", 3)]
    [InlineData("{{{", 2)]
    public void 識別子以外_falseを返す(string input, int consumed)
    {
        var utf8Input = Encoding.UTF8.GetBytes(input);
        var reader = new TemplateReader(utf8Input);

        reader.TryReadIdentifier(out var value).ShouldBeFalse();
        reader.Consumed.ShouldBe((nuint)consumed);

        value.ToArray().ShouldBeEmpty();
    }
}
