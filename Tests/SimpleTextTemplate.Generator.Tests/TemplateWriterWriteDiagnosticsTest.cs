using SimpleTextTemplate.Generator.Tests.Extensions;
using SimpleTextTemplate.Tests.TestData;
using static Microsoft.CodeAnalysis.DiagnosticSeverity;
using static SimpleTextTemplate.Generator.Tests.GeneratorRunner;
using static SimpleTextTemplate.Generator.Tests.SourceCode;

namespace SimpleTextTemplate.Generator.Tests;

public sealed class TemplateWriterWriteDiagnosticsTest
{
    [Test]
    public async Task テンプレート文字列が定数ではない_STT1000()
    {
        const string SourceCode = """
            using System.Buffers;
            using SimpleTextTemplate;

            var bufferWriter = new ArrayBufferWriter<byte>();
            var writer = TemplateWriter.Create(bufferWriter);
            var x = "a";
            TemplateRenderer.Render(ref writer, x);
            """;
        var (_, diagnostics) = await RunAsync(SourceCode);

        await Assert.That(diagnostics.Count).IsEqualTo(1);

        await Assert.That(diagnostics[0].Id).IsEqualTo("STT1000");
        await Assert.That(diagnostics[0].Severity).IsEqualTo(Error);
        await Assert.That(diagnostics[0].GetText()).IsEqualTo("x");
    }

    [Test]
    public async Task テンプレート文字列がnull_STT1000()
    {
        var sourceCode = Get(templateText: null);
        var (_, diagnostics) = await RunAsync(sourceCode);

        await Assert.That(diagnostics.Count).IsEqualTo(1);

        await Assert.That(diagnostics[0].Id).IsEqualTo("STT1000");
        await Assert.That(diagnostics[0].Severity).IsEqualTo(Error);
        await Assert.That(diagnostics[0].GetText()).IsEqualTo("null");
    }

    [Test]
    public async Task テンプレート文字列に識別子がありコンテキストの指定がない_STT1001()
    {
        const string SourceCode = """
            using System.Buffers;
            using SimpleTextTemplate;

            var bufferWriter = new ArrayBufferWriter<byte>();
            var writer = TemplateWriter.Create(bufferWriter);
            TemplateRenderer.Render(ref writer, "{{ x }}");
            """;
        var (_, diagnostics) = await RunAsync(SourceCode);

        await Assert.That(diagnostics.Count).IsEqualTo(1);

        await Assert.That(diagnostics[0].Id).IsEqualTo("STT1001");
        await Assert.That(diagnostics[0].Severity).IsEqualTo(Error);
        await Assert.That(diagnostics[0].GetText()).IsEqualTo("TemplateRenderer.Render(ref writer, \"{{ x }}\")");
    }

    [Test]
    public async Task コンテキストに識別子が存在しない_STT1002()
    {
        var sourceCode = Get("{{ A }}", nameof(ByteArrayContextTestData));
        var (_, diagnostics) = await RunAsync(sourceCode);

        await Assert.That(diagnostics.Count).IsEqualTo(1);

        await Assert.That(diagnostics[0].Id).IsEqualTo("STT1002");
        await Assert.That(diagnostics[0].Severity).IsEqualTo(Error);
        await Assert.That(diagnostics[0].GetText()).IsEqualTo("context");
    }

    [Test]
    public async Task コンテキストに複数の識別子が存在しない_STT1002()
    {
        var sourceCode = Get("{{ A }}{{ B }}", nameof(ByteArrayContextTestData));
        var (_, diagnostics) = await RunAsync(sourceCode);

        await Assert.That(diagnostics.Count).IsEqualTo(2);

        await Assert.That(diagnostics[0].Id).IsEqualTo("STT1002");
        await Assert.That(diagnostics[0].Severity).IsEqualTo(Error);
        await Assert.That(diagnostics[0].GetText()).IsEqualTo("context");

        await Assert.That(diagnostics[1].Id).IsEqualTo("STT1002");
        await Assert.That(diagnostics[1].Severity).IsEqualTo(Error);
        await Assert.That(diagnostics[1].GetText()).IsEqualTo("context");
    }

    [Test]
    public async Task テンプレート文字列が不正な形式_STT1003()
    {
        var sourceCode = Get("{{");
        var (_, diagnostics) = await RunAsync(sourceCode);

        await Assert.That(diagnostics.Count).IsEqualTo(1);

        await Assert.That(diagnostics[0].Id).IsEqualTo("STT1003");
        await Assert.That(diagnostics[0].Severity).IsEqualTo(Error);
        await Assert.That(diagnostics[0].GetText()).IsEqualTo("\"{{\"");
    }

    [Test]
    public async Task テンプレート文字列に識別子名の宣言が存在しない_STT1003()
    {
        var sourceCode = Get("{{}}");
        var (_, diagnostics) = await RunAsync(sourceCode);

        await Assert.That(diagnostics.Count).IsEqualTo(1);

        await Assert.That(diagnostics[0].Id).IsEqualTo("STT1003");
        await Assert.That(diagnostics[0].Severity).IsEqualTo(Error);
        await Assert.That(diagnostics[0].GetText()).IsEqualTo("\"{{}}\"");
    }

    [Test]
    public async Task テンプレート文字列の識別子名宣言が空白_STT1003()
    {
        var sourceCode = Get("{{ }}");
        var (_, diagnostics) = await RunAsync(sourceCode);

        await Assert.That(diagnostics.Count).IsEqualTo(1);

        await Assert.That(diagnostics[0].Id).IsEqualTo("STT1003");
        await Assert.That(diagnostics[0].Severity).IsEqualTo(Error);
        await Assert.That(diagnostics[0].GetText()).IsEqualTo("\"{{ }}\"");
    }

    [Test]
    public async Task 文字列定数識別子に対して書式指定_STT1004()
    {
        var sourceCode = Get(
            [
                "{{ StringConstantField:A }}",
                "{{ StringConstantField::ja-JP }}",
                "{{ StringConstantField:A:ja-JP }}"
            ],
            nameof(StringContextTestData));
        var (_, diagnostics) = await RunAsync(sourceCode);

        await Assert.That(diagnostics.Count).IsEqualTo(3);

        await Assert.That(diagnostics[0].Id).IsEqualTo("STT1004");
        await Assert.That(diagnostics[0].Severity).IsEqualTo(Error);
        await Assert.That(diagnostics[0].GetText()).IsEqualTo("\"{{ StringConstantField:A }}\"");

        await Assert.That(diagnostics[1].Id).IsEqualTo("STT1004");
        await Assert.That(diagnostics[1].Severity).IsEqualTo(Error);
        await Assert.That(diagnostics[1].GetText()).IsEqualTo("\"{{ StringConstantField::ja-JP }}\"");

        await Assert.That(diagnostics[2].Id).IsEqualTo("STT1004");
        await Assert.That(diagnostics[2].Severity).IsEqualTo(Error);
        await Assert.That(diagnostics[2].GetText()).IsEqualTo("\"{{ StringConstantField:A:ja-JP }}\"");
    }

    [Test]
    public async Task 列挙型識別子に対して書式指定_STT1005()
    {
        var sourceCode = Get(
            [
                "{{ EnumStaticField::ja-JP }}",
                "{{ EnumStaticField:A:ja-JP }}"
            ],
            nameof(EnumContextTestData));
        var (_, diagnostics) = await RunAsync(sourceCode);

        await Assert.That(diagnostics.Count).IsEqualTo(2);

        await Assert.That(diagnostics[0].Id).IsEqualTo("STT1005");
        await Assert.That(diagnostics[0].Severity).IsEqualTo(Error);
        await Assert.That(diagnostics[0].GetText()).IsEqualTo("\"{{ EnumStaticField::ja-JP }}\"");

        await Assert.That(diagnostics[1].Id).IsEqualTo("STT1005");
        await Assert.That(diagnostics[1].Severity).IsEqualTo(Error);
        await Assert.That(diagnostics[1].GetText()).IsEqualTo("\"{{ EnumStaticField:A:ja-JP }}\"");
    }

    [Test]
    public async Task IFormattable_ISpanFormattable_IUtf8Formattableを実装していない識別子に対して書式指定_STT1006()
    {
        var sourceCode = Get(
            [
                "{{ BytesStaticField:A }}",
                "{{ BytesStaticField::ja-JP }}",
                "{{ BytesStaticField:A:ja-JP }}"
            ],
            nameof(ByteArrayContextTestData));
        var (_, diagnostics) = await RunAsync(sourceCode);

        await Assert.That(diagnostics.Count).IsEqualTo(3);

        await Assert.That(diagnostics[0].Id).IsEqualTo("STT1006");
        await Assert.That(diagnostics[0].Severity).IsEqualTo(Error);
        await Assert.That(diagnostics[0].GetText()).IsEqualTo("\"{{ BytesStaticField:A }}\"");

        await Assert.That(diagnostics[1].Id).IsEqualTo("STT1006");
        await Assert.That(diagnostics[1].Severity).IsEqualTo(Error);
        await Assert.That(diagnostics[1].GetText()).IsEqualTo("\"{{ BytesStaticField::ja-JP }}\"");

        await Assert.That(diagnostics[2].Id).IsEqualTo("STT1006");
        await Assert.That(diagnostics[2].Severity).IsEqualTo(Error);
        await Assert.That(diagnostics[2].GetText()).IsEqualTo("\"{{ BytesStaticField:A:ja-JP }}\"");
    }
}
