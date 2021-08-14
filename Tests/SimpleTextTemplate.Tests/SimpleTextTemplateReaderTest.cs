using System.Text;
using FluentAssertions;
using Xunit;
using static SimpleTextTemplate.TemplateException;

namespace SimpleTextTemplate.Tests;

public sealed class SimpleTextTemplateReaderTest
{
    public static TheoryData<string, MemberSerializer<Block[]>> TestData => new()
    {
        {
            string.Empty,
            CreateMembers(new Block(BlockType.None, string.Empty))
        },
        {
            "{",
            CreateMembers(new Block(BlockType.Raw, "{"))
        },
        {
            "a",
            CreateMembers(new Block(BlockType.Raw, "a"))
        },
        {
            "ab",
            CreateMembers(new Block(BlockType.Raw, "ab"))
        },
        {
            "abc",
            CreateMembers(new Block(BlockType.Raw, "abc"))
        },
        {
            "{ A }",
            CreateMembers(new Block(BlockType.Raw, "{ A }"))
        },
        {
            "z{{A}}z",
            CreateMembers<Block>(new(BlockType.Raw, "z"), new(BlockType.Identifier, "A"), new(BlockType.Raw, "z"))
        },
        {
            "z{{ A }}z",
            CreateMembers<Block>(new(BlockType.Raw, "z"), new(BlockType.Identifier, "A"), new(BlockType.Raw, "z"))
        },
        {
            "}}",
            CreateMembers(new Block(BlockType.Raw, "}}"))
        },
        {
            "{{ A }}123{{ B }}",
            CreateMembers<Block>(new(BlockType.Identifier, "A"), new(BlockType.Raw, "123"), new(BlockType.Identifier, "B"))
        },
        {
            "x{{ A }}123{{ B }}x",
            CreateMembers<Block>(new(BlockType.Raw, "x"), new(BlockType.Identifier, "A"), new(BlockType.Raw, "123"), new(BlockType.Identifier, "B"), new(BlockType.Raw, "x"))
        },
        {
            "{{ A }}{{ B }}",
            CreateMembers<Block>(new(BlockType.Identifier, "A"), new(BlockType.Identifier, "B"))
        },
        {
            "{{ AAA }}{{ BBB }}",
            CreateMembers<Block>(new(BlockType.Identifier, "AAA"), new(BlockType.Identifier, "BBB"))
        },
        {
            "{{ AAA }}z{{ BBB }}",
            CreateMembers<Block>(new(BlockType.Identifier, "AAA"), new(BlockType.Raw, "z"), new(BlockType.Identifier, "BBB"))
        }
    };

    [Theory]
    [MemberData(nameof(TestData))]
    public void ReadTest(string input, MemberSerializer<Block[]> expectedItems)
    {
        var utf8Bytes = GetBytes(input);
        var reader = new TemplateReader(utf8Bytes);
        var count = 0;
        TextRange range;

        while (reader.Read(out range) is var type && type != BlockType.None)
        {
            var actual = utf8Bytes.AsSpan(range.Start, range.Length).ToArray();
            actual.Should().Equal(GetBytes(expectedItems.Value[count++].Value));
        }

        utf8Bytes.AsSpan(range.Start, range.Length).ToArray().Should().BeEmpty();

        if (!string.IsNullOrEmpty(input))
        {
            count.Should().Be(expectedItems.Value.Length);
        }
    }

    [Theory]
    [InlineData("{", "{")]
    [InlineData("a", "a")]
    [InlineData("ab", "ab")]
    [InlineData("abc", "abc")]
    [InlineData("{ A }", "{ A }")]
    [InlineData("z{{ A }}z", "z")]
    [InlineData("z{{", "z")]
    public void TryReadTemplateTest(string input, string expected)
    {
        var utf8Bytes = GetBytes(input);
        var reader = new TemplateReader(utf8Bytes);
        var success = reader.TryReadTemplate(out var range);

        var actual = utf8Bytes.AsSpan(range.Start, range.Length).ToArray();
        success.Should().BeTrue();
        actual.Should().Equal(GetBytes(expected));
    }

    [Theory]
    [InlineData("")]
    [InlineData("{{")]
    public void TryReadTemplateTest_Error(string input)
    {
        var utf8Bytes = GetBytes(input);
        var reader = new TemplateReader(utf8Bytes);
        var success = reader.TryReadTemplate(out var range);

        var actual = utf8Bytes.AsSpan(range.Start, range.Length).ToArray();
        success.Should().BeFalse();
        actual.Should().BeEmpty();
    }

    [Theory]
    [InlineData("{{A}}", "A")]
    [InlineData("{{AB}}", "AB")]
    [InlineData("{{ A }}", "A")]
    [InlineData("{{   A   }}", "A")]
    [InlineData("{{ ABC }}", "ABC")]
    [InlineData("{{ A B }}", "A B")]
    public void ReadIdentifierTest(string input, string expected)
    {
        var utf8Bytes = GetBytes(input);
        var reader = new TemplateReader(utf8Bytes);
        reader.ReadIdentifier(out var range);

        var actual = utf8Bytes.AsSpan(range.Start, range.Length).ToArray();
        actual.Should().Equal(GetBytes(expected));
    }

    [Theory]
    [InlineData("{{}}", ParserError.InvalidIdentifierFormat, 2)]
    [InlineData("", ParserError.ExpectedStartToken, 0)]
    [InlineData("{", ParserError.ExpectedStartToken, 0)]
    [InlineData("a", ParserError.ExpectedStartToken, 0)]
    [InlineData("ab", ParserError.ExpectedStartToken, 0)]
    [InlineData("abc", ParserError.ExpectedStartToken, 0)]
    [InlineData("z{{A}}z", ParserError.ExpectedStartToken, 0)]
    [InlineData("{A}", ParserError.ExpectedStartToken, 1)]
    [InlineData("{ A }", ParserError.ExpectedStartToken, 1)]
    [InlineData("{{{", ParserError.ExpectedStartToken, 2)]
    [InlineData("{{{A}}", ParserError.ExpectedStartToken, 2)]
    [InlineData("{{{ A}}", ParserError.ExpectedStartToken, 2)]
    [InlineData("{{", ParserError.ExpectedEndToken, 1)]
    [InlineData("{{ ", ParserError.ExpectedEndToken, 2)]
    [InlineData("{{ A", ParserError.ExpectedEndToken, 3)]
    [InlineData("{{A}}}", ParserError.ExpectedEndToken, 5)]
    [InlineData("{{A }}}", ParserError.ExpectedEndToken, 6)]
    public void ReadIdentifierTest_Error(string input, ParserError error, int position)
    {
        FluentActions.Invoking(Execute).Should().Throw<TemplateException>()
            .Where(x => x.Error == error && x.Position == position);

        TextRange Execute()
        {
            var utf8Bytes = GetBytes(input);
            var reader = new TemplateReader(utf8Bytes);
            reader.ReadIdentifier(out var range);

            return range;
        }
    }

    static MemberSerializer<T[]> CreateMembers<T>(params T[] values) => new(values);

    static byte[] GetBytes(string value) => Encoding.UTF8.GetBytes(value);

    public record Block(object Type, string Value);
}