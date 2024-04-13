using System.Buffers;
using System.Text;
using FluentAssertions;
using SimpleTextTemplate.Contexts;
using Utf8Utility;
using Xunit;

namespace SimpleTextTemplate.Renderer.Tests;

public sealed class TemplateExtensionsRenderTest
{
    [Theory]
    [InlineData("{{A}}")]
    [InlineData("{{ A }}")]
    [InlineData("{{  A  }}")]
    public void 識別子_識別子を置換(string input)
    {
        var template = Template.Parse(Encoding.UTF8.GetBytes(input));

        var bufferWriter = new ArrayBufferWriter<byte>();
        var dic = new Utf8ArrayDictionary<object>();
        dic.TryAdd(new("A"u8), new Utf8Array("Test1"u8));

        template.Render(bufferWriter, Context.Create(dic));
        bufferWriter.WrittenSpan.ToArray()
            .Should()
            .Equal("Test1"u8.ToArray());
    }

    [Theory]
    [InlineData("{{ A }}{{ B }}")]
    [InlineData("{{ AAA }}{{ BBB }}")]
    public void 識別子_識別子_識別子を置換(string input)
    {
        var template = Template.Parse(Encoding.UTF8.GetBytes(input));

        var bufferWriter = new ArrayBufferWriter<byte>();
        var dic = new Utf8ArrayDictionary<object>();
        dic.TryAdd(new("A"u8), new Utf8Array("Test1"u8));
        dic.TryAdd(new("AAA"u8), new Utf8Array("Test1"u8));
        dic.TryAdd(new("B"u8), new Utf8Array("Test2"u8));
        dic.TryAdd(new("BBB"u8), new Utf8Array("Test2"u8));

        template.Render(bufferWriter, Context.Create(dic));
        bufferWriter.WrittenSpan.ToArray()
            .Should()
            .Equal("Test1Test2"u8.ToArray());
    }

    [Theory]
    [InlineData("z{{A}}z")]
    [InlineData("z{{ A }}z")]
    public void 文字列_識別子_文字列_識別子を置換(string input)
    {
        var template = Template.Parse(Encoding.UTF8.GetBytes(input));

        var bufferWriter = new ArrayBufferWriter<byte>();
        var dic = new Utf8ArrayDictionary<object>();
        dic.TryAdd(new("A"u8), new Utf8Array("Test1"u8));

        template.Render(bufferWriter, Context.Create(dic));
        bufferWriter.WrittenSpan.ToArray()
            .Should()
            .Equal("zTest1z"u8.ToArray());
    }

    [Theory]
    [InlineData("{{ A }}z{{ B }}")]
    [InlineData("{{ AAA }}z{{ BBB }}")]
    public void 識別子_文字列_識別子_識別子を置換(string input)
    {
        var template = Template.Parse(Encoding.UTF8.GetBytes(input));

        var bufferWriter = new ArrayBufferWriter<byte>();
        var dic = new Utf8ArrayDictionary<object>();
        dic.TryAdd(new("A"u8), new Utf8Array("Test1"u8));
        dic.TryAdd(new("AAA"u8), new Utf8Array("Test1"u8));
        dic.TryAdd(new("B"u8), new Utf8Array("Test2"u8));
        dic.TryAdd(new("BBB"u8), new Utf8Array("Test2"u8));

        template.Render(bufferWriter, Context.Create(dic));
        bufferWriter.WrittenSpan.ToArray()
            .Should()
            .Equal("Test1zTest2"u8.ToArray());
    }

    [Theory]
    [InlineData("x{{ A }}123{{ B }}x")]
    public void 文字列_識別子_文字列_識別子_文字列_識別子を置換(string input)
    {
        var template = Template.Parse(Encoding.UTF8.GetBytes(input));

        var bufferWriter = new ArrayBufferWriter<byte>();
        var dic = new Utf8ArrayDictionary<object>();
        dic.TryAdd(new("A"u8), new Utf8Array("Test1"u8));
        dic.TryAdd(new("B"u8), new Utf8Array("Test2"u8));

        template.Render(bufferWriter, Context.Create(dic));
        bufferWriter.WrittenSpan.ToArray()
            .Should()
            .Equal("xTest1123Test2x"u8.ToArray());
    }
}
