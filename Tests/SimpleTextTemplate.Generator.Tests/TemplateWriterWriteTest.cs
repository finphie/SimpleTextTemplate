using System.Buffers;
using System.Globalization;
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
        const string SourceCode = """
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
        var (compilation, diagnostics) = Run(SourceCode);
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
        interceptInfoList[0].Methods[0].Text.Should().Be("\"A\"u8");
        interceptInfoList[0].Methods[0].Format.Should().BeNull();
        interceptInfoList[0].Methods[0].Provider.Should().BeNull();

        interceptInfoList[1].Methods[0].Name.Should().Be(WriteConstantLiteral);
        interceptInfoList[1].Methods[0].Text.Should().Be("\"B\"u8");
        interceptInfoList[1].Methods[0].Format.Should().BeNull();
        interceptInfoList[1].Methods[0].Provider.Should().BeNull();
    }

    [Fact]
    public void 文字列定数()
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
        interceptInfoList[0].Methods[0].Text.Should().Be("\"A\"u8");
        interceptInfoList[0].Methods[0].Format.Should().BeNull();
        interceptInfoList[0].Methods[0].Provider.Should().BeNull();

        interceptInfoList[0].Methods[1].Name.Should().Be(WriteString);
        interceptInfoList[0].Methods[1].Text.Should().Be("global::SimpleTextTemplate.Generator.Tests.Core.StringContextTestData.@StringStaticField");
        interceptInfoList[0].Methods[1].Format.Should().BeNull();
        interceptInfoList[0].Methods[1].Provider.Should().BeNull();

        interceptInfoList[0].Methods[2].Name.Should().Be(WriteConstantLiteral);
        interceptInfoList[0].Methods[2].Text.Should().Be("\"B\"u8");
        interceptInfoList[0].Methods[2].Format.Should().BeNull();
        interceptInfoList[0].Methods[2].Provider.Should().BeNull();

        interceptInfoList[1].Methods[0].Name.Should().Be(WriteConstantLiteral);
        interceptInfoList[1].Methods[0].Text.Should().Be($"\"A{StringContextTestData.StringConstantField}{StringContextTestData.StringConstantField}B\"u8");
        interceptInfoList[1].Methods[0].Format.Should().BeNull();
        interceptInfoList[1].Methods[0].Provider.Should().BeNull();
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
        interceptInfoList[0].Methods[0].Text.Should().Be("global::SimpleTextTemplate.Generator.Tests.Core.ByteArrayContextTestData.@BytesStaticField");
        interceptInfoList[0].Methods[0].Format.Should().BeNull();
        interceptInfoList[0].Methods[0].Provider.Should().BeNull();

        interceptInfoList[1].Methods[0].Name.Should().Be(WriteLiteral);
        interceptInfoList[1].Methods[0].Text.Should().Be("global::SimpleTextTemplate.Generator.Tests.Core.ByteArrayContextTestData.@BytesStaticField");
        interceptInfoList[1].Methods[0].Format.Should().BeNull();
        interceptInfoList[1].Methods[0].Provider.Should().BeNull();

        // BytesField
        interceptInfoList[0].Methods[1].Name.Should().Be(WriteLiteral);
        interceptInfoList[0].Methods[1].Text.Should().Be("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@BytesField");
        interceptInfoList[0].Methods[1].Format.Should().BeNull();
        interceptInfoList[0].Methods[1].Provider.Should().BeNull();

        interceptInfoList[1].Methods[1].Name.Should().Be(WriteLiteral);
        interceptInfoList[1].Methods[1].Text.Should().Be("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@BytesField");
        interceptInfoList[1].Methods[1].Format.Should().BeNull();
        interceptInfoList[1].Methods[1].Provider.Should().BeNull();

        // BytesSpanStaticProperty
        interceptInfoList[0].Methods[2].Name.Should().Be(WriteLiteral);
        interceptInfoList[0].Methods[2].Text.Should().Be("global::SimpleTextTemplate.Generator.Tests.Core.ByteArrayContextTestData.@BytesSpanStaticProperty");
        interceptInfoList[0].Methods[2].Format.Should().BeNull();
        interceptInfoList[0].Methods[2].Provider.Should().BeNull();

        interceptInfoList[1].Methods[2].Name.Should().Be(WriteLiteral);
        interceptInfoList[1].Methods[2].Text.Should().Be("global::SimpleTextTemplate.Generator.Tests.Core.ByteArrayContextTestData.@BytesSpanStaticProperty");
        interceptInfoList[1].Methods[2].Format.Should().BeNull();
        interceptInfoList[1].Methods[2].Provider.Should().BeNull();

        // BytesSpanProperty
        interceptInfoList[0].Methods[3].Name.Should().Be(WriteLiteral);
        interceptInfoList[0].Methods[3].Text.Should().Be("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@BytesSpanProperty");
        interceptInfoList[0].Methods[3].Format.Should().BeNull();
        interceptInfoList[0].Methods[3].Provider.Should().BeNull();

        interceptInfoList[1].Methods[3].Name.Should().Be(WriteLiteral);
        interceptInfoList[1].Methods[3].Text.Should().Be("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@BytesSpanProperty");
        interceptInfoList[1].Methods[3].Format.Should().BeNull();
        interceptInfoList[1].Methods[3].Provider.Should().BeNull();

        // BytesStaticProperty
        interceptInfoList[0].Methods[4].Name.Should().Be(WriteLiteral);
        interceptInfoList[0].Methods[4].Text.Should().Be("global::SimpleTextTemplate.Generator.Tests.Core.ByteArrayContextTestData.@BytesStaticProperty");
        interceptInfoList[0].Methods[4].Format.Should().BeNull();
        interceptInfoList[0].Methods[4].Provider.Should().BeNull();

        interceptInfoList[1].Methods[4].Name.Should().Be(WriteLiteral);
        interceptInfoList[1].Methods[4].Text.Should().Be("global::SimpleTextTemplate.Generator.Tests.Core.ByteArrayContextTestData.@BytesStaticProperty");
        interceptInfoList[1].Methods[4].Format.Should().BeNull();
        interceptInfoList[1].Methods[4].Provider.Should().BeNull();

        // BytesProperty
        interceptInfoList[0].Methods[5].Name.Should().Be(WriteLiteral);
        interceptInfoList[0].Methods[5].Text.Should().Be("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@BytesProperty");
        interceptInfoList[0].Methods[5].Format.Should().BeNull();
        interceptInfoList[0].Methods[5].Provider.Should().BeNull();

        interceptInfoList[1].Methods[5].Name.Should().Be(WriteLiteral);
        interceptInfoList[1].Methods[5].Text.Should().Be("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@BytesProperty");
        interceptInfoList[1].Methods[5].Format.Should().BeNull();
        interceptInfoList[1].Methods[5].Provider.Should().BeNull();
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

        // CharsStaticField
        interceptInfoList[0].Methods[0].Name.Should().Be(WriteString);
        interceptInfoList[0].Methods[0].Text.Should().Be("global::SimpleTextTemplate.Generator.Tests.Core.CharArrayContextTestData.@CharsStaticField");
        interceptInfoList[0].Methods[0].Format.Should().BeNull();
        interceptInfoList[0].Methods[0].Provider.Should().BeNull();

        interceptInfoList[1].Methods[0].Name.Should().Be(WriteString);
        interceptInfoList[1].Methods[0].Text.Should().Be("global::SimpleTextTemplate.Generator.Tests.Core.CharArrayContextTestData.@CharsStaticField");
        interceptInfoList[1].Methods[0].Format.Should().BeNull();
        interceptInfoList[1].Methods[0].Provider.Should().BeNull();

        // CharsField
        interceptInfoList[0].Methods[1].Name.Should().Be(WriteString);
        interceptInfoList[0].Methods[1].Text.Should().Be("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@CharsField");
        interceptInfoList[0].Methods[1].Format.Should().BeNull();
        interceptInfoList[0].Methods[1].Provider.Should().BeNull();

        interceptInfoList[1].Methods[1].Name.Should().Be(WriteString);
        interceptInfoList[1].Methods[1].Text.Should().Be("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@CharsField");
        interceptInfoList[1].Methods[1].Format.Should().BeNull();
        interceptInfoList[1].Methods[1].Provider.Should().BeNull();

        // CharsSpanStaticProperty
        interceptInfoList[0].Methods[2].Name.Should().Be(WriteString);
        interceptInfoList[0].Methods[2].Text.Should().Be("global::SimpleTextTemplate.Generator.Tests.Core.CharArrayContextTestData.@CharsSpanStaticProperty");
        interceptInfoList[0].Methods[2].Format.Should().BeNull();
        interceptInfoList[0].Methods[2].Provider.Should().BeNull();

        interceptInfoList[1].Methods[2].Name.Should().Be(WriteString);
        interceptInfoList[1].Methods[2].Text.Should().Be("global::SimpleTextTemplate.Generator.Tests.Core.CharArrayContextTestData.@CharsSpanStaticProperty");
        interceptInfoList[1].Methods[2].Format.Should().BeNull();
        interceptInfoList[1].Methods[2].Provider.Should().BeNull();

        // CharsSpanProperty
        interceptInfoList[0].Methods[3].Name.Should().Be(WriteString);
        interceptInfoList[0].Methods[3].Text.Should().Be("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@CharsSpanProperty");
        interceptInfoList[0].Methods[3].Format.Should().BeNull();
        interceptInfoList[0].Methods[3].Provider.Should().BeNull();

        interceptInfoList[1].Methods[3].Name.Should().Be(WriteString);
        interceptInfoList[1].Methods[3].Text.Should().Be("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@CharsSpanProperty");
        interceptInfoList[1].Methods[3].Format.Should().BeNull();
        interceptInfoList[1].Methods[3].Provider.Should().BeNull();

        // CharsStaticProperty
        interceptInfoList[0].Methods[4].Name.Should().Be(WriteString);
        interceptInfoList[0].Methods[4].Text.Should().Be("global::SimpleTextTemplate.Generator.Tests.Core.CharArrayContextTestData.@CharsStaticProperty");
        interceptInfoList[0].Methods[4].Format.Should().BeNull();
        interceptInfoList[0].Methods[4].Provider.Should().BeNull();

        interceptInfoList[1].Methods[4].Name.Should().Be(WriteString);
        interceptInfoList[1].Methods[4].Text.Should().Be("global::SimpleTextTemplate.Generator.Tests.Core.CharArrayContextTestData.@CharsStaticProperty");
        interceptInfoList[1].Methods[4].Format.Should().BeNull();
        interceptInfoList[1].Methods[4].Provider.Should().BeNull();

        // CharsProperty
        interceptInfoList[0].Methods[5].Name.Should().Be(WriteString);
        interceptInfoList[0].Methods[5].Text.Should().Be("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@CharsProperty");
        interceptInfoList[0].Methods[5].Format.Should().BeNull();
        interceptInfoList[0].Methods[5].Provider.Should().BeNull();

        interceptInfoList[1].Methods[5].Name.Should().Be(WriteString);
        interceptInfoList[1].Methods[5].Text.Should().Be("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@CharsProperty");
        interceptInfoList[1].Methods[5].Format.Should().BeNull();
        interceptInfoList[1].Methods[5].Provider.Should().BeNull();
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
        interceptInfoList[0].Methods[0].Text.Should().Be("\"_StringConstantField\"u8");
        interceptInfoList[0].Methods[0].Format.Should().BeNull();
        interceptInfoList[0].Methods[0].Provider.Should().BeNull();

        interceptInfoList[1].Methods[0].Name.Should().Be(WriteConstantLiteral);
        interceptInfoList[1].Methods[0].Text.Should().Be("\"_StringConstantField\"u8");
        interceptInfoList[1].Methods[0].Format.Should().BeNull();
        interceptInfoList[1].Methods[0].Provider.Should().BeNull();

        // StringStaticField
        interceptInfoList[0].Methods[1].Name.Should().Be(WriteString);
        interceptInfoList[0].Methods[1].Text.Should().Be("global::SimpleTextTemplate.Generator.Tests.Core.StringContextTestData.@StringStaticField");
        interceptInfoList[0].Methods[1].Format.Should().BeNull();
        interceptInfoList[0].Methods[1].Provider.Should().BeNull();

        interceptInfoList[1].Methods[1].Name.Should().Be(WriteString);
        interceptInfoList[1].Methods[1].Text.Should().Be("global::SimpleTextTemplate.Generator.Tests.Core.StringContextTestData.@StringStaticField");
        interceptInfoList[1].Methods[1].Format.Should().BeNull();
        interceptInfoList[1].Methods[1].Provider.Should().BeNull();

        // StringField
        interceptInfoList[0].Methods[2].Name.Should().Be(WriteString);
        interceptInfoList[0].Methods[2].Text.Should().Be("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@StringField");
        interceptInfoList[0].Methods[2].Format.Should().BeNull();
        interceptInfoList[0].Methods[2].Provider.Should().BeNull();

        interceptInfoList[1].Methods[2].Name.Should().Be(WriteString);
        interceptInfoList[1].Methods[2].Text.Should().Be("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@StringField");
        interceptInfoList[1].Methods[2].Format.Should().BeNull();
        interceptInfoList[1].Methods[2].Provider.Should().BeNull();

        // StringStaticProperty
        interceptInfoList[0].Methods[3].Name.Should().Be(WriteString);
        interceptInfoList[0].Methods[3].Text.Should().Be("global::SimpleTextTemplate.Generator.Tests.Core.StringContextTestData.@StringStaticProperty");
        interceptInfoList[0].Methods[3].Format.Should().BeNull();
        interceptInfoList[0].Methods[3].Provider.Should().BeNull();

        interceptInfoList[1].Methods[3].Name.Should().Be(WriteString);
        interceptInfoList[1].Methods[3].Text.Should().Be("global::SimpleTextTemplate.Generator.Tests.Core.StringContextTestData.@StringStaticProperty");
        interceptInfoList[1].Methods[3].Format.Should().BeNull();
        interceptInfoList[1].Methods[3].Provider.Should().BeNull();

        // StringProperty
        interceptInfoList[0].Methods[4].Name.Should().Be(WriteString);
        interceptInfoList[0].Methods[4].Text.Should().Be("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@StringProperty");
        interceptInfoList[0].Methods[4].Format.Should().BeNull();
        interceptInfoList[0].Methods[4].Provider.Should().BeNull();

        interceptInfoList[1].Methods[4].Name.Should().Be(WriteString);
        interceptInfoList[1].Methods[4].Text.Should().Be("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@StringProperty");
        interceptInfoList[1].Methods[4].Format.Should().BeNull();
        interceptInfoList[1].Methods[4].Provider.Should().BeNull();
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
        interceptInfoList[0].Methods[0].Text.Should().Be("global::SimpleTextTemplate.Generator.Tests.Core.EnumContextTestData.@EnumStaticField");
        interceptInfoList[0].Methods[0].Format.Should().Be("default");
        interceptInfoList[0].Methods[0].Provider.Should().BeNull();

        interceptInfoList[1].Methods[0].Name.Should().Be(WriteEnum);
        interceptInfoList[1].Methods[0].Text.Should().Be("global::SimpleTextTemplate.Generator.Tests.Core.EnumContextTestData.@EnumStaticField");
        interceptInfoList[1].Methods[0].Format.Should().Be("\"D\"");
        interceptInfoList[1].Methods[0].Provider.Should().BeNull();

        // EnumField
        interceptInfoList[0].Methods[1].Name.Should().Be(WriteEnum);
        interceptInfoList[0].Methods[1].Text.Should().Be("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@EnumField");
        interceptInfoList[0].Methods[1].Format.Should().Be("default");
        interceptInfoList[0].Methods[1].Provider.Should().BeNull();

        interceptInfoList[1].Methods[1].Name.Should().Be(WriteEnum);
        interceptInfoList[1].Methods[1].Text.Should().Be("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@EnumField");
        interceptInfoList[1].Methods[1].Format.Should().Be("\"D\"");
        interceptInfoList[1].Methods[1].Provider.Should().BeNull();

        // EnumStaticProperty
        interceptInfoList[0].Methods[2].Name.Should().Be(WriteEnum);
        interceptInfoList[0].Methods[2].Text.Should().Be("global::SimpleTextTemplate.Generator.Tests.Core.EnumContextTestData.@EnumStaticProperty");
        interceptInfoList[0].Methods[2].Format.Should().Be("default");
        interceptInfoList[0].Methods[2].Provider.Should().BeNull();

        interceptInfoList[1].Methods[2].Name.Should().Be(WriteEnum);
        interceptInfoList[1].Methods[2].Text.Should().Be("global::SimpleTextTemplate.Generator.Tests.Core.EnumContextTestData.@EnumStaticProperty");
        interceptInfoList[1].Methods[2].Format.Should().Be("\"D\"");
        interceptInfoList[1].Methods[2].Provider.Should().BeNull();

        // EnumProperty
        interceptInfoList[0].Methods[3].Name.Should().Be(WriteEnum);
        interceptInfoList[0].Methods[3].Text.Should().Be("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@EnumProperty");
        interceptInfoList[0].Methods[3].Format.Should().Be("default");
        interceptInfoList[0].Methods[3].Provider.Should().BeNull();

        interceptInfoList[1].Methods[3].Name.Should().Be(WriteEnum);
        interceptInfoList[1].Methods[3].Text.Should().Be("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@EnumProperty");
        interceptInfoList[1].Methods[3].Format.Should().Be("\"D\"");
        interceptInfoList[1].Methods[3].Provider.Should().BeNull();
    }

    [Fact]
    public void Int型()
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
        interceptInfoList[0].Methods[0].Text.Should().Be("\"1234\"u8");
        interceptInfoList[0].Methods[0].Format.Should().BeNull();
        interceptInfoList[0].Methods[0].Provider.Should().BeNull();

        interceptInfoList[1].Methods[0].Name.Should().Be(WriteConstantLiteral);
        interceptInfoList[1].Methods[0].Text.Should().Be("\"1,234.000\"u8");
        interceptInfoList[1].Methods[0].Format.Should().BeNull();
        interceptInfoList[1].Methods[0].Provider.Should().BeNull();

        interceptInfoList[2].Methods[0].Name.Should().Be(WriteConstantLiteral);
        interceptInfoList[2].Methods[0].Text.Should().Be("\"1.234,000\"u8");
        interceptInfoList[2].Methods[0].Format.Should().BeNull();
        interceptInfoList[2].Methods[0].Provider.Should().BeNull();

        // IntStaticField
        interceptInfoList[0].Methods[1].Name.Should().Be(WriteValue);
        interceptInfoList[0].Methods[1].Text.Should().Be("global::SimpleTextTemplate.Generator.Tests.Core.IntContextTestData.@IntStaticField");
        interceptInfoList[0].Methods[1].Format.Should().Be("default");
        interceptInfoList[0].Methods[1].Provider.Should().Be("global::System.Globalization.CultureInfo.InvariantCulture");

        interceptInfoList[1].Methods[1].Name.Should().Be(WriteValue);
        interceptInfoList[1].Methods[1].Text.Should().Be("global::SimpleTextTemplate.Generator.Tests.Core.IntContextTestData.@IntStaticField");
        interceptInfoList[1].Methods[1].Format.Should().Be("\"N3\"");
        interceptInfoList[1].Methods[1].Provider.Should().Be("global::System.Globalization.CultureInfo.InvariantCulture");

        // IntField
        interceptInfoList[0].Methods[2].Name.Should().Be(WriteValue);
        interceptInfoList[0].Methods[2].Text.Should().Be("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@IntField");
        interceptInfoList[0].Methods[2].Format.Should().Be("default");
        interceptInfoList[0].Methods[2].Provider.Should().Be("global::System.Globalization.CultureInfo.InvariantCulture");

        interceptInfoList[1].Methods[2].Name.Should().Be(WriteValue);
        interceptInfoList[1].Methods[2].Text.Should().Be("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@IntField");
        interceptInfoList[1].Methods[2].Format.Should().Be("\"N3\"");
        interceptInfoList[1].Methods[2].Provider.Should().Be("global::System.Globalization.CultureInfo.InvariantCulture");

        // IntStaticProperty
        interceptInfoList[0].Methods[3].Name.Should().Be(WriteValue);
        interceptInfoList[0].Methods[3].Text.Should().Be("global::SimpleTextTemplate.Generator.Tests.Core.IntContextTestData.@IntStaticProperty");
        interceptInfoList[0].Methods[3].Format.Should().Be("default");
        interceptInfoList[0].Methods[3].Provider.Should().Be("global::System.Globalization.CultureInfo.InvariantCulture");

        interceptInfoList[1].Methods[3].Name.Should().Be(WriteValue);
        interceptInfoList[1].Methods[3].Text.Should().Be("global::SimpleTextTemplate.Generator.Tests.Core.IntContextTestData.@IntStaticProperty");
        interceptInfoList[1].Methods[3].Format.Should().Be("\"N3\"");
        interceptInfoList[1].Methods[3].Provider.Should().Be("global::System.Globalization.CultureInfo.InvariantCulture");

        // IntProperty
        interceptInfoList[0].Methods[4].Name.Should().Be(WriteValue);
        interceptInfoList[0].Methods[4].Text.Should().Be("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@IntProperty");
        interceptInfoList[0].Methods[4].Format.Should().Be("default");
        interceptInfoList[0].Methods[4].Provider.Should().Be("global::System.Globalization.CultureInfo.InvariantCulture");

        interceptInfoList[1].Methods[4].Name.Should().Be(WriteValue);
        interceptInfoList[1].Methods[4].Text.Should().Be("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@IntProperty");
        interceptInfoList[1].Methods[4].Format.Should().Be("\"N3\"");
        interceptInfoList[1].Methods[4].Provider.Should().Be("esES");
    }

    [Fact]
    public void 定数Int型_特定カルチャー指定()
    {
        var sourceCode = Get(
            [
                "{{ IntConstantField }}",
                "{{ IntConstantField:N3 }}",
                "{{ IntConstantField:N3:es-ES }}"
            ],
            nameof(IntContextTestData),
            CultureInfo.GetCultureInfo("ja-JP").ToExpressionString());
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.Should().BeEmpty();
        interceptInfoList.Should().HaveCount(3);
        interceptInfoList[0].Methods.Should().HaveCount(1);
        interceptInfoList[1].Methods.Should().HaveCount(1);
        interceptInfoList[2].Methods.Should().HaveCount(1);

        interceptInfoList[0].Methods[0].Name.Should().Be(WriteValue);
        interceptInfoList[0].Methods[0].Text.Should().Be("global::SimpleTextTemplate.Generator.Tests.Core.IntContextTestData.@IntConstantField");
        interceptInfoList[0].Methods[0].Format.Should().Be("default");
        interceptInfoList[0].Methods[0].Provider.Should().Be("provider");

        interceptInfoList[1].Methods[0].Name.Should().Be(WriteValue);
        interceptInfoList[1].Methods[0].Text.Should().Be("global::SimpleTextTemplate.Generator.Tests.Core.IntContextTestData.@IntConstantField");
        interceptInfoList[1].Methods[0].Format.Should().Be("\"N3\"");
        interceptInfoList[1].Methods[0].Provider.Should().Be("provider");

        interceptInfoList[2].Methods[0].Name.Should().Be(WriteConstantLiteral);
        interceptInfoList[2].Methods[0].Text.Should().Be("\"1.234,000\"u8");
        interceptInfoList[2].Methods[0].Format.Should().BeNull();
        interceptInfoList[2].Methods[0].Provider.Should().BeNull();
    }

    [Fact]
    public void DateTimeOffset型()
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
        interceptInfoList[0].Methods[0].Text.Should().Be("global::SimpleTextTemplate.Generator.Tests.Core.DateTimeOffsetContextTestData.@DateTimeOffsetStaticField");
        interceptInfoList[0].Methods[0].Format.Should().Be("default");
        interceptInfoList[0].Methods[0].Provider.Should().Be("global::System.Globalization.CultureInfo.InvariantCulture");

        interceptInfoList[1].Methods[0].Name.Should().Be(WriteValue);
        interceptInfoList[1].Methods[0].Text.Should().Be("global::SimpleTextTemplate.Generator.Tests.Core.DateTimeOffsetContextTestData.@DateTimeOffsetStaticField");
        interceptInfoList[1].Methods[0].Format.Should().Be("\"o\"");
        interceptInfoList[1].Methods[0].Provider.Should().Be("global::System.Globalization.CultureInfo.InvariantCulture");

        // DateTimeOffsetField
        interceptInfoList[0].Methods[1].Name.Should().Be(WriteValue);
        interceptInfoList[0].Methods[1].Text.Should().Be("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@DateTimeOffsetField");
        interceptInfoList[0].Methods[1].Format.Should().Be("default");
        interceptInfoList[0].Methods[1].Provider.Should().Be("global::System.Globalization.CultureInfo.InvariantCulture");

        interceptInfoList[1].Methods[1].Name.Should().Be(WriteValue);
        interceptInfoList[1].Methods[1].Text.Should().Be("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@DateTimeOffsetField");
        interceptInfoList[1].Methods[1].Format.Should().Be("\"D\"");
        interceptInfoList[1].Methods[1].Provider.Should().Be("jaJP");

        // DateTimeOffsetStaticProperty
        interceptInfoList[0].Methods[2].Name.Should().Be(WriteValue);
        interceptInfoList[0].Methods[2].Text.Should().Be("global::SimpleTextTemplate.Generator.Tests.Core.DateTimeOffsetContextTestData.@DateTimeOffsetStaticProperty");
        interceptInfoList[0].Methods[2].Format.Should().Be("default");
        interceptInfoList[0].Methods[2].Provider.Should().Be("global::System.Globalization.CultureInfo.InvariantCulture");

        interceptInfoList[1].Methods[2].Name.Should().Be(WriteValue);
        interceptInfoList[1].Methods[2].Text.Should().Be("global::SimpleTextTemplate.Generator.Tests.Core.DateTimeOffsetContextTestData.@DateTimeOffsetStaticProperty");
        interceptInfoList[1].Methods[2].Format.Should().Be("default");
        interceptInfoList[1].Methods[2].Provider.Should().Be("jaJP");

        // DateTimeOffsetProperty
        interceptInfoList[0].Methods[3].Name.Should().Be(WriteValue);
        interceptInfoList[0].Methods[3].Text.Should().Be("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@DateTimeOffsetProperty");
        interceptInfoList[0].Methods[3].Format.Should().Be("default");
        interceptInfoList[0].Methods[3].Provider.Should().Be("global::System.Globalization.CultureInfo.InvariantCulture");

        interceptInfoList[1].Methods[3].Name.Should().Be(WriteValue);
        interceptInfoList[1].Methods[3].Text.Should().Be("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@DateTimeOffsetProperty");
        interceptInfoList[1].Methods[3].Format.Should().Be("default");
        interceptInfoList[1].Methods[3].Provider.Should().Be("global::System.Globalization.CultureInfo.InvariantCulture");
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

    [Fact]
    public void InvariantCulture()
    {
        var sourceCode = Get("{{ DateTimeOffsetStaticField }}{{ DateTimeOffsetStaticField::ja-JP }}", nameof(DateTimeOffsetContextTestData), CultureInfo.InvariantCulture.ToExpressionString());
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.Should().BeEmpty();
        interceptInfoList.Should().HaveCount(1);
        interceptInfoList[0].Methods.Should().HaveCount(2);

        interceptInfoList[0].Methods[0].Name.Should().Be(WriteValue);
        interceptInfoList[0].Methods[0].Provider.Should().Be("global::System.Globalization.CultureInfo.InvariantCulture");

        interceptInfoList[0].Methods[1].Name.Should().Be(WriteValue);
        interceptInfoList[0].Methods[1].Provider.Should().Be("jaJP");
    }

    [Fact]
    public void InvariantInfo()
    {
        var sourceCode = Get("{{ DateTimeOffsetStaticField }}{{ DateTimeOffsetStaticField::ja-JP }}", nameof(DateTimeOffsetContextTestData), DateTimeFormatInfo.InvariantInfo.ToExpressionString());
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.Should().BeEmpty();
        interceptInfoList.Should().HaveCount(1);
        interceptInfoList[0].Methods.Should().HaveCount(2);

        interceptInfoList[0].Methods[0].Name.Should().Be(WriteValue);
        interceptInfoList[0].Methods[0].Provider.Should().Be("global::System.Globalization.CultureInfo.InvariantCulture");

        interceptInfoList[0].Methods[1].Name.Should().Be(WriteValue);
        interceptInfoList[0].Methods[1].Provider.Should().Be("jaJP");
    }

    [Fact]
    public void CurrentCulture()
    {
        var sourceCode = Get("{{ DateTimeOffsetStaticField }}{{ DateTimeOffsetStaticField::ja-JP }}", nameof(DateTimeOffsetContextTestData), CultureInfo.CurrentCulture.ToExpressionString());
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.Should().BeEmpty();
        interceptInfoList.Should().HaveCount(1);
        interceptInfoList[0].Methods.Should().HaveCount(2);

        interceptInfoList[0].Methods[0].Name.Should().Be(WriteValue);
        interceptInfoList[0].Methods[0].Provider.Should().Be("provider");

        interceptInfoList[0].Methods[1].Name.Should().Be(WriteValue);
        interceptInfoList[0].Methods[1].Provider.Should().Be("jaJP");
    }

    [Fact]
    public void 特定カルチャー指定()
    {
        var sourceCode = Get("{{ DateTimeOffsetStaticField }}{{ DateTimeOffsetStaticField::ja-JP }}", nameof(DateTimeOffsetContextTestData), CultureInfo.GetCultureInfo("en-US", true).ToExpressionString());
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.Should().BeEmpty();
        interceptInfoList.Should().HaveCount(1);
        interceptInfoList[0].Methods.Should().HaveCount(2);

        interceptInfoList[0].Methods[0].Name.Should().Be(WriteValue);
        interceptInfoList[0].Methods[0].Provider.Should().Be("provider");

        interceptInfoList[0].Methods[1].Name.Should().Be(WriteValue);
        interceptInfoList[0].Methods[1].Provider.Should().Be("jaJP");
    }

    [Fact]
    public void カルチャーにnullを指定()
    {
        var sourceCode = Get("{{ DateTimeOffsetStaticField }}{{ DateTimeOffsetStaticField::ja-JP }}", nameof(DateTimeOffsetContextTestData), "null");
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.Should().BeEmpty();
        interceptInfoList.Should().HaveCount(1);
        interceptInfoList[0].Methods.Should().HaveCount(2);

        interceptInfoList[0].Methods[0].Name.Should().Be(WriteValue);
        interceptInfoList[0].Methods[0].Provider.Should().Be("global::System.Globalization.CultureInfo.InvariantCulture");

        interceptInfoList[0].Methods[1].Name.Should().Be(WriteValue);
        interceptInfoList[0].Methods[1].Provider.Should().Be("jaJP");
    }

    [Fact]
    public void テンプレート文字列の書式指定を省略()
    {
        var sourceCode = Get(
            [
                "{{DateTimeOffsetStaticField:}}{{ DateTimeOffsetStaticField: }}",
                "{{ DateTimeOffsetStaticField::ja-JP }}"
            ],
            nameof(DateTimeOffsetContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.Should().BeEmpty();
        interceptInfoList.Should().HaveCount(2);
        interceptInfoList[0].Methods.Should().HaveCount(2);
        interceptInfoList[1].Methods.Should().HaveCount(1);

        interceptInfoList[0].Methods[0].Name.Should().Be(WriteValue);
        interceptInfoList[0].Methods[0].Format.Should().Be("default");
        interceptInfoList[0].Methods[0].Provider.Should().Be("global::System.Globalization.CultureInfo.InvariantCulture");

        interceptInfoList[0].Methods[1].Name.Should().Be(WriteValue);
        interceptInfoList[0].Methods[1].Format.Should().Be("default");
        interceptInfoList[0].Methods[1].Provider.Should().Be("global::System.Globalization.CultureInfo.InvariantCulture");

        interceptInfoList[1].Methods[0].Name.Should().Be(WriteValue);
        interceptInfoList[1].Methods[0].Format.Should().Be("default");
        interceptInfoList[1].Methods[0].Provider.Should().Be("jaJP");
    }

    [Fact]
    public void テンプレート文字列のカルチャー指定を省略()
    {
        var sourceCode = Get(
            [
                "{{DateTimeOffsetStaticField:o:}}{{ DateTimeOffsetStaticField:o: }}",
                "{{ DateTimeOffsetStaticField:o:  }}",
            ],
            nameof(DateTimeOffsetContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.Should().BeEmpty();
        interceptInfoList.Should().HaveCount(2);
        interceptInfoList[0].Methods.Should().HaveCount(2);
        interceptInfoList[1].Methods.Should().HaveCount(1);

        interceptInfoList[0].Methods[0].Name.Should().Be(WriteValue);
        interceptInfoList[0].Methods[0].Format.Should().Be("\"o\"");
        interceptInfoList[0].Methods[0].Provider.Should().Be("global::System.Globalization.CultureInfo.InvariantCulture");

        interceptInfoList[0].Methods[1].Name.Should().Be(WriteValue);
        interceptInfoList[0].Methods[1].Format.Should().Be("\"o\"");
        interceptInfoList[0].Methods[1].Provider.Should().Be("global::System.Globalization.CultureInfo.InvariantCulture");

        interceptInfoList[1].Methods[0].Name.Should().Be(WriteValue);
        interceptInfoList[1].Methods[0].Format.Should().Be("\"o\"");
        interceptInfoList[1].Methods[0].Provider.Should().Be("global::System.Globalization.CultureInfo.InvariantCulture");
    }

    [Fact]
    public void テンプレート文字列の書式及びカルチャー指定を省略()
    {
        var sourceCode = Get(
            [
                "{{ DateTimeOffsetStaticField:: }}{{DateTimeOffsetStaticField::}}",
                "{{ DateTimeOffsetStaticField::  }}",
            ],
            nameof(DateTimeOffsetContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.Should().BeEmpty();
        interceptInfoList.Should().HaveCount(2);
        interceptInfoList[0].Methods.Should().HaveCount(2);
        interceptInfoList[1].Methods.Should().HaveCount(1);

        interceptInfoList[0].Methods[0].Name.Should().Be(WriteValue);
        interceptInfoList[0].Methods[0].Format.Should().Be("default");
        interceptInfoList[0].Methods[0].Provider.Should().Be("global::System.Globalization.CultureInfo.InvariantCulture");

        interceptInfoList[0].Methods[1].Name.Should().Be(WriteValue);
        interceptInfoList[0].Methods[1].Format.Should().Be("default");
        interceptInfoList[0].Methods[1].Provider.Should().Be("global::System.Globalization.CultureInfo.InvariantCulture");

        interceptInfoList[1].Methods[0].Name.Should().Be(WriteValue);
        interceptInfoList[1].Methods[0].Format.Should().Be("default");
        interceptInfoList[1].Methods[0].Provider.Should().Be("global::System.Globalization.CultureInfo.InvariantCulture");
    }

    [Fact]
    public void IFormattableを実装していない識別子()
    {
        var sourceCode = Get("{{ NonFormattableStaticField }}{{ NonFormattableField }}{{ NonFormattableStaticProperty }}{{ NonFormattableProperty }}", nameof(NonFormattableContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.Should().BeEmpty();
        interceptInfoList.Should().HaveCount(1);
        interceptInfoList[0].Methods.Should().HaveCount(4);

        interceptInfoList[0].Methods[0].Name.Should().Be(WriteValue);
        interceptInfoList[0].Methods[0].Text.Should().Be("global::SimpleTextTemplate.Generator.Tests.Core.NonFormattableContextTestData.@NonFormattableStaticField");
        interceptInfoList[0].Methods[0].Format.Should().Be("default");
        interceptInfoList[0].Methods[0].Provider.Should().Be("global::System.Globalization.CultureInfo.InvariantCulture");

        interceptInfoList[0].Methods[1].Name.Should().Be(WriteValue);
        interceptInfoList[0].Methods[1].Text.Should().Be("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@NonFormattableField");
        interceptInfoList[0].Methods[1].Format.Should().Be("default");
        interceptInfoList[0].Methods[1].Provider.Should().Be("global::System.Globalization.CultureInfo.InvariantCulture");

        interceptInfoList[0].Methods[2].Name.Should().Be(WriteValue);
        interceptInfoList[0].Methods[2].Text.Should().Be("global::SimpleTextTemplate.Generator.Tests.Core.NonFormattableContextTestData.@NonFormattableStaticProperty");
        interceptInfoList[0].Methods[2].Format.Should().Be("default");
        interceptInfoList[0].Methods[2].Provider.Should().Be("global::System.Globalization.CultureInfo.InvariantCulture");

        interceptInfoList[0].Methods[3].Name.Should().Be(WriteValue);
        interceptInfoList[0].Methods[3].Text.Should().Be("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@NonFormattableProperty");
        interceptInfoList[0].Methods[3].Format.Should().Be("default");
        interceptInfoList[0].Methods[3].Provider.Should().Be("global::System.Globalization.CultureInfo.InvariantCulture");
    }

    [Fact]
    public void 定数識別子がnullまたはEmpty()
    {
        var sourceCode = Get(
            [
                "{{ NullStringConstantField }}{{ NullObjectConstantField }}{{ NullStringStaticField }}{{ NullObjectStaticField }}",
                "{{ EmptyStringConstantField }}",
                "A{{ NullStringConstantField }}B{{ EmptyStringConstantField }}C"
            ],
            nameof(NullContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.Should().BeEmpty();
        interceptInfoList.Should().HaveCount(3);
        interceptInfoList[0].Methods.Should().HaveCount(2);
        interceptInfoList[1].Methods.Should().HaveCount(0);
        interceptInfoList[2].Methods.Should().HaveCount(1);

        interceptInfoList[0].Methods[0].Name.Should().Be(WriteString);
        interceptInfoList[0].Methods[0].Text.Should().Be("global::SimpleTextTemplate.Generator.Tests.Core.NullContextTestData.@NullStringStaticField");
        interceptInfoList[0].Methods[0].Format.Should().BeNull();
        interceptInfoList[0].Methods[0].Provider.Should().BeNull();

        interceptInfoList[0].Methods[1].Name.Should().Be(WriteValue);
        interceptInfoList[0].Methods[1].Text.Should().Be("global::SimpleTextTemplate.Generator.Tests.Core.NullContextTestData.@NullObjectStaticField");
        interceptInfoList[0].Methods[1].Format.Should().Be("default");
        interceptInfoList[0].Methods[1].Provider.Should().Be("global::System.Globalization.CultureInfo.InvariantCulture");

        interceptInfoList[2].Methods[0].Name.Should().Be(WriteConstantLiteral);
        interceptInfoList[2].Methods[0].Text.Should().Be("\"ABC\"u8");
        interceptInfoList[2].Methods[0].Format.Should().BeNull();
        interceptInfoList[2].Methods[0].Provider.Should().BeNull();
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
