using System;
using System.Text;
using FluentAssertions;
using Xunit;
using static SimpleTextTemplate.SimpleTextTemplateException;

namespace SimpleTextTemplate.Tests
{
    public sealed class HtmlReaderTest
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
        public void ReadTest(string html, MemberSerializer<Block[]> expectedItems)
        {
            var utf8Html = GetBytes(html);
            var reader = new SimpleTextTemplateReader(utf8Html);
            var count = 0;
            Range range;

            while (reader.Read(out range) is var type && type != BlockType.None)
            {
                var actual = utf8Html[range];
                actual.Should().Equal(GetBytes(expectedItems.Value[count++].Value));
            }

            utf8Html[range].Should().BeEmpty();

            if (!string.IsNullOrEmpty(html))
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
        public void TryReadHtmlTest(string html, string expected)
        {
            var utf8Html = GetBytes(html);
            var reader = new SimpleTextTemplateReader(utf8Html);
            var success = reader.TryReadHtml(out var range);

            var actual = utf8Html[range];
            success.Should().BeTrue();
            actual.Should().Equal(GetBytes(expected));
        }

        [Theory]
        [InlineData("")]
        [InlineData("{{")]
        public void TryReadHtmlTest_Error(string html)
        {
            var utf8Html = GetBytes(html);
            var reader = new SimpleTextTemplateReader(utf8Html);
            var success = reader.TryReadHtml(out var range);

            var actual = utf8Html[range];
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
        public void ReadObjectTest(string html, string expected)
        {
            var utf8Html = GetBytes(html);
            var reader = new SimpleTextTemplateReader(utf8Html);
            reader.ReadIdentifier(out var range);

            var actual = utf8Html[range];
            actual.Should().Equal(GetBytes(expected));
        }

        [Theory]
        [InlineData("{{}}", ParserError.InvalidObjectFormat, 2)]
        [InlineData("", ParserError.ExpectedStartObject, 0)]
        [InlineData("{", ParserError.ExpectedStartObject, 0)]
        [InlineData("a", ParserError.ExpectedStartObject, 0)]
        [InlineData("ab", ParserError.ExpectedStartObject, 0)]
        [InlineData("abc", ParserError.ExpectedStartObject, 0)]
        [InlineData("z{{A}}z", ParserError.ExpectedStartObject, 0)]
        [InlineData("{A}", ParserError.ExpectedStartObject, 1)]
        [InlineData("{ A }", ParserError.ExpectedStartObject, 1)]
        [InlineData("{{{", ParserError.ExpectedStartObject, 2)]
        [InlineData("{{{A}}", ParserError.ExpectedStartObject, 2)]
        [InlineData("{{{ A}}", ParserError.ExpectedStartObject, 2)]
        [InlineData("{{", ParserError.ExpectedEndObject, 1)]
        [InlineData("{{ ", ParserError.ExpectedEndObject, 2)]
        [InlineData("{{ A", ParserError.ExpectedEndObject, 3)]
        [InlineData("{{A}}}", ParserError.ExpectedEndObject, 5)]
        [InlineData("{{A }}}", ParserError.ExpectedEndObject, 6)]
        public void ReadObjectTest_Error(string html, ParserError error, int position)
        {
            FluentActions.Invoking(Execute).Should().Throw<SimpleTextTemplateException>()
                .Where(x => x.Error == error && x.Position == position);

            Range Execute()
            {
                var utf8Html = GetBytes(html);
                var reader = new SimpleTextTemplateReader(utf8Html);
                reader.ReadIdentifier(out var range);

                return range;
            }
        }

        static MemberSerializer<T[]> CreateMembers<T>(params T[] values) => new(values);

        static byte[] GetBytes(string value) => Encoding.UTF8.GetBytes(value);

        public record Block(object Type, string Value);
    }
}