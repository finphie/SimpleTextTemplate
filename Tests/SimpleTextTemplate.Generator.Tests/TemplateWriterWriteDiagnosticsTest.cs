using Shouldly;
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
            TemplateRenderer.Render(ref writer, x);
            """;
        var (_, diagnostics) = Run(SourceCode);

        diagnostics.Count.ShouldBe(1);

        diagnostics[0].Id.ShouldBe("STT1000");
        diagnostics[0].Severity.ShouldBe(Error);
        diagnostics[0].GetText().ShouldBe("x");
    }

    [Fact]
    public void テンプレート文字列がnull_STT1000()
    {
        var sourceCode = Get(templateText: null);
        var (_, diagnostics) = Run(sourceCode);

        diagnostics.Count.ShouldBe(1);

        diagnostics[0].Id.ShouldBe("STT1000");
        diagnostics[0].Severity.ShouldBe(Error);
        diagnostics[0].GetText().ShouldBe("null");
    }

    [Fact]
    public void テンプレート文字列に識別子がありコンテキストの指定がない_STT1001()
    {
        const string SourceCode = """
            using System.Buffers;
            using SimpleTextTemplate;

            var bufferWriter = new ArrayBufferWriter<byte>();
            var writer = TemplateWriter.Create(bufferWriter);
            TemplateRenderer.Render(ref writer, "{{ x }}");
            """;
        var (_, diagnostics) = Run(SourceCode);

        diagnostics.Count.ShouldBe(1);

        diagnostics[0].Id.ShouldBe("STT1001");
        diagnostics[0].Severity.ShouldBe(Error);
        diagnostics[0].GetText().ShouldBe("TemplateRenderer.Render(ref writer, \"{{ x }}\")");
    }

    [Fact]
    public void コンテキストに識別子が存在しない_STT1002()
    {
        var sourceCode = Get("{{ A }}", nameof(ByteArrayContextTestData));
        var (_, diagnostics) = Run(sourceCode);

        diagnostics.Count.ShouldBe(1);

        diagnostics[0].Id.ShouldBe("STT1002");
        diagnostics[0].Severity.ShouldBe(Error);
        diagnostics[0].GetText().ShouldBe("context");
    }

    [Fact]
    public void コンテキストに複数の識別子が存在しない_STT1002()
    {
        var sourceCode = Get("{{ A }}{{ B }}", nameof(ByteArrayContextTestData));
        var (_, diagnostics) = Run(sourceCode);

        diagnostics.Count.ShouldBe(2);

        diagnostics[0].Id.ShouldBe("STT1002");
        diagnostics[0].Severity.ShouldBe(Error);
        diagnostics[0].GetText().ShouldBe("context");

        diagnostics[1].Id.ShouldBe("STT1002");
        diagnostics[1].Severity.ShouldBe(Error);
        diagnostics[1].GetText().ShouldBe("context");
    }

    [Fact]
    public void テンプレート文字列が不正な形式_STT1003()
    {
        var sourceCode = Get("{{");
        var (_, diagnostics) = Run(sourceCode);

        diagnostics.Count.ShouldBe(1);

        diagnostics[0].Id.ShouldBe("STT1003");
        diagnostics[0].Severity.ShouldBe(Error);
        diagnostics[0].GetText().ShouldBe("\"{{\"");
    }

    [Fact]
    public void テンプレート文字列に識別子名の宣言が存在しない_STT1003()
    {
        var sourceCode = Get("{{}}");
        var (_, diagnostics) = Run(sourceCode);

        diagnostics.Count.ShouldBe(1);

        diagnostics[0].Id.ShouldBe("STT1003");
        diagnostics[0].Severity.ShouldBe(Error);
        diagnostics[0].GetText().ShouldBe("\"{{}}\"");
    }

    [Fact]
    public void テンプレート文字列の識別子名宣言が空白_STT1003()
    {
        var sourceCode = Get("{{ }}");
        var (_, diagnostics) = Run(sourceCode);

        diagnostics.Count.ShouldBe(1);

        diagnostics[0].Id.ShouldBe("STT1003");
        diagnostics[0].Severity.ShouldBe(Error);
        diagnostics[0].GetText().ShouldBe("\"{{ }}\"");
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

        diagnostics.Count.ShouldBe(3);

        diagnostics[0].Id.ShouldBe("STT1004");
        diagnostics[0].Severity.ShouldBe(Error);
        diagnostics[0].GetText().ShouldBe("\"{{ StringConstantField:A }}\"");

        diagnostics[1].Id.ShouldBe("STT1004");
        diagnostics[1].Severity.ShouldBe(Error);
        diagnostics[1].GetText().ShouldBe("\"{{ StringConstantField::ja-JP }}\"");

        diagnostics[2].Id.ShouldBe("STT1004");
        diagnostics[2].Severity.ShouldBe(Error);
        diagnostics[2].GetText().ShouldBe("\"{{ StringConstantField:A:ja-JP }}\"");
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

        diagnostics.Count.ShouldBe(2);

        diagnostics[0].Id.ShouldBe("STT1005");
        diagnostics[0].Severity.ShouldBe(Error);
        diagnostics[0].GetText().ShouldBe("\"{{ EnumStaticField::ja-JP }}\"");

        diagnostics[1].Id.ShouldBe("STT1005");
        diagnostics[1].Severity.ShouldBe(Error);
        diagnostics[1].GetText().ShouldBe("\"{{ EnumStaticField:A:ja-JP }}\"");
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

        diagnostics.Count.ShouldBe(3);

        diagnostics[0].Id.ShouldBe("STT1006");
        diagnostics[0].Severity.ShouldBe(Error);
        diagnostics[0].GetText().ShouldBe("\"{{ BytesStaticField:A }}\"");

        diagnostics[1].Id.ShouldBe("STT1006");
        diagnostics[1].Severity.ShouldBe(Error);
        diagnostics[1].GetText().ShouldBe("\"{{ BytesStaticField::ja-JP }}\"");

        diagnostics[2].Id.ShouldBe("STT1006");
        diagnostics[2].Severity.ShouldBe(Error);
        diagnostics[2].GetText().ShouldBe("\"{{ BytesStaticField:A:ja-JP }}\"");
    }
}
