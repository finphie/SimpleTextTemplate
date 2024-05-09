using System.Buffers;
using FluentAssertions;
using SimpleTextTemplate.Generator.Tests.Core;
using SimpleTextTemplate.Generator.Tests.Extensions;
using Xunit;
using static SimpleTextTemplate.Generator.Tests.Constants;
using static SimpleTextTemplate.Generator.Tests.GeneratorRunner;
using static SimpleTextTemplate.Generator.Tests.SourceCode;

namespace SimpleTextTemplate.Generator.Tests;

public sealed class TemplateWriterWriteTest
{
    [Fact]
    public void 空白()
    {
        var sourceCode = Get(string.Empty);
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.Should().BeEmpty();
        interceptInfoList.Should().HaveCount(1);
        interceptInfoList[0].Methods.Should().HaveCount(0);
    }

    [Fact]
    public void StringEmpty()
    {
        var sourceCode = $$$"""
            using System.Buffers;
            using SimpleTextTemplate;
            using static System.String;
            using S = System.String;
            
            var bufferWriter = new ArrayBufferWriter<byte>();
            var writer = TemplateWriter.Create(bufferWriter);
            writer.Write(string.Empty);
            writer.Write(System.String.Empty);
            writer.Write(global::System.String.Empty);
            writer.Write(S.Empty);
            writer.Write(Empty);
            """;
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.Should().BeEmpty();
        interceptInfoList.Should().HaveCount(5);

        interceptInfoList[0].Methods.Should().HaveCount(0);
        interceptInfoList[1].Methods.Should().HaveCount(0);
        interceptInfoList[2].Methods.Should().HaveCount(0);
        interceptInfoList[3].Methods.Should().HaveCount(0);
        interceptInfoList[4].Methods.Should().HaveCount(0);
    }

    [Fact]
    public void 識別子なし()
    {
        var sourceCode = Get(
            [
                "A",
                "B"
            ]);
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.Should().BeEmpty();
        interceptInfoList.Should().HaveCount(2);
        interceptInfoList[0].Methods.Should().HaveCount(1);
        interceptInfoList[1].Methods.Should().HaveCount(1);

        interceptInfoList[0].Methods[0].Name.Should().Be(WriteConstantLiteral);
        interceptInfoList[1].Methods[0].Name.Should().Be(WriteConstantLiteral);
    }

    [Fact]
    public void 定数()
    {
        var sourceCode = Get(
            [
                "A{{ StringStaticField }}B",
                "A{{ StringConstantField }}{{ StringConstantField }}B"
            ],
            nameof(StringContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.Should().BeEmpty();
        interceptInfoList.Should().HaveCount(2);
        interceptInfoList[0].Methods.Should().HaveCount(3);
        interceptInfoList[1].Methods.Should().HaveCount(1);

        interceptInfoList[0].Methods[0].Name.Should().Be(WriteConstantLiteral);
        interceptInfoList[0].Methods[1].Name.Should().Be(WriteString);
        interceptInfoList[0].Methods[2].Name.Should().Be(WriteConstantLiteral);
        interceptInfoList[1].Methods[0].Name.Should().Be(WriteConstantLiteral);
        interceptInfoList[1].Methods[0].Text.Should().Be($"\"A{StringContextTestData.StringConstantField}{StringContextTestData.StringConstantField}B\"u8");
    }

    [Fact]
    public void Byte配列()
    {
        var sourceCode = Get(
            [
                "{{ BytesStaticField }}{{ BytesField }}{{ BytesSpanStaticProperty }}{{ BytesSpanProperty }}{{ BytesStaticProperty }}{{ BytesProperty }}",
                "{{ BytesStaticField }}{{ BytesField }}{{ BytesSpanStaticProperty }}{{ BytesSpanProperty }}{{ BytesStaticProperty }}{{ BytesProperty }}"
            ],
            nameof(ByteArrayContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.Should().BeEmpty();
        interceptInfoList.Should().HaveCount(2);
        interceptInfoList[0].Methods.Should().HaveCount(6);
        interceptInfoList[1].Methods.Should().HaveCount(6);

        // BytesStaticField
        interceptInfoList[0].Methods[0].Name.Should().Be(WriteLiteral);
        interceptInfoList[1].Methods[0].Name.Should().Be(WriteLiteral);

        // BytesField
        interceptInfoList[0].Methods[1].Name.Should().Be(WriteLiteral);
        interceptInfoList[1].Methods[1].Name.Should().Be(WriteLiteral);

        // BytesSpanStaticProperty
        interceptInfoList[0].Methods[2].Name.Should().Be(WriteLiteral);
        interceptInfoList[1].Methods[2].Name.Should().Be(WriteLiteral);

        // BytesSpanProperty
        interceptInfoList[0].Methods[3].Name.Should().Be(WriteLiteral);
        interceptInfoList[1].Methods[3].Name.Should().Be(WriteLiteral);

        // BytesStaticProperty
        interceptInfoList[0].Methods[4].Name.Should().Be(WriteLiteral);
        interceptInfoList[1].Methods[4].Name.Should().Be(WriteLiteral);

        // BytesProperty
        interceptInfoList[0].Methods[5].Name.Should().Be(WriteLiteral);
        interceptInfoList[1].Methods[5].Name.Should().Be(WriteLiteral);
    }

    [Fact]
    public void Char配列()
    {
        var sourceCode = Get(
            [
                "{{ CharsStaticField }}{{ CharsField }}{{ CharsSpanStaticProperty }}{{ CharsSpanProperty }}{{ CharsStaticProperty }}{{ CharsProperty }}",
                "{{ CharsStaticField }}{{ CharsField }}{{ CharsSpanStaticProperty }}{{ CharsSpanProperty }}{{ CharsStaticProperty }}{{ CharsProperty }}"
            ],
            nameof(CharArrayContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.Should().BeEmpty();
        interceptInfoList.Should().HaveCount(2);
        interceptInfoList[0].Methods.Should().HaveCount(6);
        interceptInfoList[1].Methods.Should().HaveCount(6);

        // BytesStaticField
        interceptInfoList[0].Methods[0].Name.Should().Be(WriteString);
        interceptInfoList[1].Methods[0].Name.Should().Be(WriteString);

        // BytesField
        interceptInfoList[0].Methods[1].Name.Should().Be(WriteString);
        interceptInfoList[1].Methods[1].Name.Should().Be(WriteString);

        // BytesSpanStaticProperty
        interceptInfoList[0].Methods[2].Name.Should().Be(WriteString);
        interceptInfoList[1].Methods[2].Name.Should().Be(WriteString);

        // BytesSpanProperty
        interceptInfoList[0].Methods[3].Name.Should().Be(WriteString);
        interceptInfoList[1].Methods[3].Name.Should().Be(WriteString);

        // BytesStaticProperty
        interceptInfoList[0].Methods[4].Name.Should().Be(WriteString);
        interceptInfoList[1].Methods[4].Name.Should().Be(WriteString);

        // BytesProperty
        interceptInfoList[0].Methods[5].Name.Should().Be(WriteString);
        interceptInfoList[1].Methods[5].Name.Should().Be(WriteString);
    }

    [Fact]
    public void 文字列()
    {
        var sourceCode = Get(
            [
                "{{ StringConstantField }}{{ StringStaticField }}{{ StringField }}{{ StringStaticProperty }}{{ StringProperty }}",
                "{{ StringConstantField }}{{ StringStaticField }}{{ StringField }}{{ StringStaticProperty }}{{ StringProperty }}"
            ],
            nameof(StringContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.Should().BeEmpty();
        interceptInfoList.Should().HaveCount(2);
        interceptInfoList[0].Methods.Should().HaveCount(5);
        interceptInfoList[1].Methods.Should().HaveCount(5);

        // StringConstantField
        interceptInfoList[0].Methods[0].Name.Should().Be(WriteConstantLiteral);
        interceptInfoList[1].Methods[0].Name.Should().Be(WriteConstantLiteral);

        // StringStaticField
        interceptInfoList[0].Methods[1].Name.Should().Be(WriteString);
        interceptInfoList[1].Methods[1].Name.Should().Be(WriteString);

        // StringField
        interceptInfoList[0].Methods[2].Name.Should().Be(WriteString);
        interceptInfoList[1].Methods[2].Name.Should().Be(WriteString);

        // StringStaticProperty
        interceptInfoList[0].Methods[3].Name.Should().Be(WriteString);
        interceptInfoList[1].Methods[3].Name.Should().Be(WriteString);

        // StringProperty
        interceptInfoList[0].Methods[4].Name.Should().Be(WriteString);
        interceptInfoList[1].Methods[4].Name.Should().Be(WriteString);
    }

    [Fact]
    public void Enum()
    {
        var sourceCode = Get(
            [
                "{{ EnumStaticField }}{{ EnumField }}{{ EnumStaticProperty }}{{ EnumProperty }}",
                "{{ EnumStaticField:D }}{{ EnumField:D }}{{ EnumStaticProperty:D }}{{ EnumProperty:D }}"
            ],
            nameof(EnumContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.Should().BeEmpty();
        interceptInfoList.Should().HaveCount(2);
        interceptInfoList[0].Methods.Should().HaveCount(4);
        interceptInfoList[1].Methods.Should().HaveCount(4);

        // EnumStaticField
        interceptInfoList[0].Methods[0].Name.Should().Be(WriteEnum);
        interceptInfoList[1].Methods[0].Name.Should().Be(WriteEnum);

        // EnumField
        interceptInfoList[0].Methods[1].Name.Should().Be(WriteEnum);
        interceptInfoList[1].Methods[1].Name.Should().Be(WriteEnum);

        // EnumStaticProperty
        interceptInfoList[0].Methods[2].Name.Should().Be(WriteEnum);
        interceptInfoList[1].Methods[2].Name.Should().Be(WriteEnum);

        // EnumProperty
        interceptInfoList[0].Methods[3].Name.Should().Be(WriteEnum);
        interceptInfoList[1].Methods[3].Name.Should().Be(WriteEnum);
    }

    [Fact]
    public void 数字()
    {
        var sourceCode = Get(
            [
                "{{ IntConstantField }}{{ IntStaticField }}{{ IntField }}{{ IntStaticProperty }}{{ IntProperty }}",
                "{{ IntConstantField:N3 }}{{ IntStaticField:N3 }}{{ IntField:N3 }}{{ IntStaticProperty:N3 }}{{ IntProperty:N3:es-ES }}",
                "{{ IntConstantField:N3:es-ES }}"
            ],
            nameof(IntContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.Should().BeEmpty();
        interceptInfoList.Should().HaveCount(3);
        interceptInfoList[0].Methods.Should().HaveCount(5);
        interceptInfoList[1].Methods.Should().HaveCount(5);
        interceptInfoList[2].Methods.Should().HaveCount(1);

        // IntConstantField
        interceptInfoList[0].Methods[0].Name.Should().Be(WriteConstantLiteral);
        interceptInfoList[1].Methods[0].Name.Should().Be(WriteConstantLiteral);
        interceptInfoList[2].Methods[0].Name.Should().Be(WriteConstantLiteral);

        // IntStaticField
        interceptInfoList[0].Methods[1].Name.Should().Be(WriteValue);
        interceptInfoList[1].Methods[1].Name.Should().Be(WriteValue);

        // IntField
        interceptInfoList[0].Methods[2].Name.Should().Be(WriteValue);
        interceptInfoList[1].Methods[2].Name.Should().Be(WriteValue);

        // IntStaticProperty
        interceptInfoList[0].Methods[3].Name.Should().Be(WriteValue);
        interceptInfoList[1].Methods[3].Name.Should().Be(WriteValue);

        // IntProperty
        interceptInfoList[0].Methods[4].Name.Should().Be(WriteValue);
        interceptInfoList[1].Methods[4].Name.Should().Be(WriteValue);
    }

    [Fact]
    public void DateTimeOffset()
    {
        var sourceCode = Get(
            [
                "{{ DateTimeOffsetStaticField }}{{ DateTimeOffsetField }}{{ DateTimeOffsetStaticProperty }}{{ DateTimeOffsetProperty }}",
                "{{ DateTimeOffsetStaticField:o }}{{ DateTimeOffsetField:D:ja-JP }}{{ DateTimeOffsetStaticProperty::ja-JP }}{{ DateTimeOffsetProperty }}"
            ],
            nameof(DateTimeOffsetContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.Should().BeEmpty();
        interceptInfoList.Should().HaveCount(2);
        interceptInfoList[0].Methods.Should().HaveCount(4);
        interceptInfoList[1].Methods.Should().HaveCount(4);

        // DateTimeOffsetStaticField
        interceptInfoList[0].Methods[0].Name.Should().Be(WriteValue);
        interceptInfoList[1].Methods[0].Name.Should().Be(WriteValue);

        // DateTimeOffsetField
        interceptInfoList[0].Methods[1].Name.Should().Be(WriteValue);
        interceptInfoList[1].Methods[1].Name.Should().Be(WriteValue);

        // DateTimeOffsetStaticProperty
        interceptInfoList[0].Methods[2].Name.Should().Be(WriteValue);
        interceptInfoList[1].Methods[2].Name.Should().Be(WriteValue);

        // DateTimeOffsetProperty
        interceptInfoList[0].Methods[2].Name.Should().Be(WriteValue);
        interceptInfoList[1].Methods[2].Name.Should().Be(WriteValue);
    }

    [Fact]
    public void トップレベルステートメント()
    {
        var sourceCode = $$$"""
            using System.Buffers;
            using SimpleTextTemplate;
            using SimpleTextTemplate.Generator.Tests.Core;
            
            var bufferWriter = new ArrayBufferWriter<byte>();
            var writer = TemplateWriter.Create(bufferWriter);
            var context = new {{{nameof(ByteArrayContextTestData)}}}();
            writer.Write("{{ BytesStaticField }}", in context);
            """;
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.Should().BeEmpty();
        interceptInfoList.Should().HaveCount(1);
        interceptInfoList[0].Methods.Should().HaveCount(1);
        interceptInfoList[0].Methods[0].Name.Should().Be(WriteLiteral);
    }
}

file static class Constants
{
    public const string WriteConstantLiteral = nameof(TemplateWriter<IBufferWriter<byte>>.WriteConstantLiteral);
    public const string WriteLiteral = nameof(TemplateWriter<IBufferWriter<byte>>.WriteLiteral);
    public const string WriteString = nameof(TemplateWriter<IBufferWriter<byte>>.WriteString);
    public const string WriteEnum = nameof(TemplateWriter<IBufferWriter<byte>>.WriteEnum);
    public const string WriteValue = nameof(TemplateWriter<IBufferWriter<byte>>.WriteValue);
}
