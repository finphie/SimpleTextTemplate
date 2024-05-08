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
        var sourceCode = """
            using System.Buffers;
            using SimpleTextTemplate;
            
            var bufferWriter = new ArrayBufferWriter<byte>();
            var writer = TemplateWriter.Create(bufferWriter);
            var x = "a";
            writer.Write(x);
            """;
        var (_, diagnostics) = Run(sourceCode);

        diagnostics.Should().HaveCount(1);

        diagnostics[0].Id.Should().Be("STT1000");
        diagnostics[0].Severity.Should().Be(Error);
        diagnostics[0].GetText().Should().Be("x");
    }

    [Fact]
    public void テンプレート文字列に識別子がありコンテキストの指定がない_STT1001()
    {
        var sourceCode = """
            using System.Buffers;
            using SimpleTextTemplate;
            
            var bufferWriter = new ArrayBufferWriter<byte>();
            var writer = TemplateWriter.Create(bufferWriter);
            writer.Write("{{ x }}");
            """;
        var (_, diagnostics) = Run(sourceCode);

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
}
