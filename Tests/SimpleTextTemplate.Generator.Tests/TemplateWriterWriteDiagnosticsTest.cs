using FluentAssertions;
using SimpleTextTemplate.Generator.Tests.Core;
using SimpleTextTemplate.Generator.Tests.Extensions;
using Xunit;
using static Microsoft.CodeAnalysis.DiagnosticSeverity;
using static SimpleTextTemplate.Generator.Tests.GeneratorRunner;
using static SimpleTextTemplate.Generator.Tests.SourceCode;

namespace SimpleTextTemplate.Generator.Tests;

public sealed class TemplateWriterWriteDiagnosticsTest
{
    [Fact]
    public void テンプレート文字列が定数ではない_STT1000()
    {
        const string SourceCode = """
            using System.Buffers;
            using SimpleTextTemplate;
            
            var bufferWriter = new ArrayBufferWriter<byte>();
            var writer = TemplateWriter.Create(bufferWriter);
            var x = "a";
            writer.Write(x);
            """;
        var (_, diagnostics) = Run(SourceCode);

        diagnostics.Should().HaveCount(1);

        diagnostics[0].Id.Should().Be("STT1000");
        diagnostics[0].Severity.Should().Be(Error);
        diagnostics[0].GetText().Should().Be("x");
    }

    [Fact]
    public void テンプレート文字列がnull_STT1000()
    {
        var sourceCode = Get(templateText: null);
        var (_, diagnostics) = Run(sourceCode);

        diagnostics.Should().HaveCount(1);

        diagnostics[0].Id.Should().Be("STT1000");
        diagnostics[0].Severity.Should().Be(Error);
        diagnostics[0].GetText().Should().Be("null");
    }

    [Fact]
    public void テンプレート文字列に識別子がありコンテキストの指定がない_STT1001()
    {
        const string SourceCode = """
            using System.Buffers;
            using SimpleTextTemplate;
            
            var bufferWriter = new ArrayBufferWriter<byte>();
            var writer = TemplateWriter.Create(bufferWriter);
            writer.Write("{{ x }}");
            """;
        var (_, diagnostics) = Run(SourceCode);

        diagnostics.Should().HaveCount(1);

        diagnostics[0].Id.Should().Be("STT1001");
        diagnostics[0].Severity.Should().Be(Error);
        diagnostics[0].GetText().Should().Be("writer.Write(\"{{ x }}\")");
    }

    [Fact]
    public void コンテキストに識別子が存在しない_STT1002()
    {
        var sourceCode = Get("{{ A }}", nameof(ByteArrayContextTestData));
        var (_, diagnostics) = Run(sourceCode);

        diagnostics.Should().HaveCount(1);

        diagnostics[0].Id.Should().Be("STT1002");
        diagnostics[0].Severity.Should().Be(Error);
        diagnostics[0].GetText().Should().Be("context");
    }

    [Fact]
    public void コンテキストに複数の識別子が存在しない_STT1002()
    {
        var sourceCode = Get("{{ A }}{{ B }}", nameof(ByteArrayContextTestData));
        var (_, diagnostics) = Run(sourceCode);

        diagnostics.Should().HaveCount(2);

        diagnostics[0].Id.Should().Be("STT1002");
        diagnostics[0].Severity.Should().Be(Error);
        diagnostics[0].GetText().Should().Be("context");

        diagnostics[1].Id.Should().Be("STT1002");
        diagnostics[1].Severity.Should().Be(Error);
        diagnostics[1].GetText().Should().Be("context");
    }

    [Fact]
    public void テンプレート文字列が不正な形式_STT1003()
    {
        var sourceCode = Get("{{");
        var (_, diagnostics) = Run(sourceCode);

        diagnostics.Should().HaveCount(1);

        diagnostics[0].Id.Should().Be("STT1003");
        diagnostics[0].Severity.Should().Be(Error);
        diagnostics[0].GetText().Should().Be("\"{{\"");
    }

    [Fact]
    public void テンプレート文字列に識別子名の宣言が存在しない_STT1003()
    {
        var sourceCode = Get("{{}}");
        var (_, diagnostics) = Run(sourceCode);

        diagnostics.Should().HaveCount(1);

        diagnostics[0].Id.Should().Be("STT1003");
        diagnostics[0].Severity.Should().Be(Error);
        diagnostics[0].GetText().Should().Be("\"{{}}\"");
    }

    [Fact]
    public void テンプレート文字列の識別子名宣言が空白_STT1003()
    {
        var sourceCode = Get("{{ }}");
        var (_, diagnostics) = Run(sourceCode);

        diagnostics.Should().HaveCount(1);

        diagnostics[0].Id.Should().Be("STT1003");
        diagnostics[0].Severity.Should().Be(Error);
        diagnostics[0].GetText().Should().Be("\"{{ }}\"");
    }

    [Fact]
    public void 文字列定数識別子に対して書式指定_STT1004()
    {
        var sourceCode = Get(
            [
                "{{ StringConstantField:A }}",
                "{{ StringConstantField::ja-JP }}",
                "{{ StringConstantField:A:ja-JP }}"
            ],
            nameof(StringContextTestData));
        var (_, diagnostics) = Run(sourceCode);

        diagnostics.Should().HaveCount(3);

        diagnostics[0].Id.Should().Be("STT1004");
        diagnostics[0].Severity.Should().Be(Error);
        diagnostics[0].GetText().Should().Be("\"{{ StringConstantField:A }}\"");

        diagnostics[1].Id.Should().Be("STT1004");
        diagnostics[1].Severity.Should().Be(Error);
        diagnostics[1].GetText().Should().Be("\"{{ StringConstantField::ja-JP }}\"");

        diagnostics[2].Id.Should().Be("STT1004");
        diagnostics[2].Severity.Should().Be(Error);
        diagnostics[2].GetText().Should().Be("\"{{ StringConstantField:A:ja-JP }}\"");
    }

    [Fact]
    public void 列挙型識別子に対して書式指定_STT1005()
    {
        var sourceCode = Get(
            [
                "{{ EnumStaticField::ja-JP }}",
                "{{ EnumStaticField:A:ja-JP }}"
            ],
            nameof(EnumContextTestData));
        var (_, diagnostics) = Run(sourceCode);

        diagnostics.Should().HaveCount(2);

        diagnostics[0].Id.Should().Be("STT1005");
        diagnostics[0].Severity.Should().Be(Error);
        diagnostics[0].GetText().Should().Be("\"{{ EnumStaticField::ja-JP }}\"");

        diagnostics[1].Id.Should().Be("STT1005");
        diagnostics[1].Severity.Should().Be(Error);
        diagnostics[1].GetText().Should().Be("\"{{ EnumStaticField:A:ja-JP }}\"");
    }

    [Fact]
    public void IFormattable_ISpanFormattable_IUtf8Formattableを実装していない識別子に対して書式指定_STT1006()
    {
        var sourceCode = Get(
            [
                "{{ BytesStaticField:A }}",
                "{{ BytesStaticField::ja-JP }}",
                "{{ BytesStaticField:A:ja-JP }}"
            ],
            nameof(ByteArrayContextTestData));
        var (_, diagnostics) = Run(sourceCode);

        diagnostics.Should().HaveCount(3);

        diagnostics[0].Id.Should().Be("STT1006");
        diagnostics[0].Severity.Should().Be(Error);
        diagnostics[0].GetText().Should().Be("\"{{ BytesStaticField:A }}\"");

        diagnostics[1].Id.Should().Be("STT1006");
        diagnostics[1].Severity.Should().Be(Error);
        diagnostics[1].GetText().Should().Be("\"{{ BytesStaticField::ja-JP }}\"");

        diagnostics[2].Id.Should().Be("STT1006");
        diagnostics[2].Severity.Should().Be(Error);
        diagnostics[2].GetText().Should().Be("\"{{ BytesStaticField:A:ja-JP }}\"");
    }
}
