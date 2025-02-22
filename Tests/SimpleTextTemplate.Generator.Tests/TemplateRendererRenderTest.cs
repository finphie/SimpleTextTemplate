using System.Globalization;
using Shouldly;
using SimpleTextTemplate.Generator.Tests.Core;
using SimpleTextTemplate.Generator.Tests.Extensions;
using Xunit;
using static SimpleTextTemplate.Generator.Tests.Constants;
using static SimpleTextTemplate.Generator.Tests.GeneratorRunner;
using static SimpleTextTemplate.Generator.Tests.SourceCode;

namespace SimpleTextTemplate.Generator.Tests;

public sealed class TemplateRendererRender
{
    [Fact]
    public void 識別子なし()
    {
        var sourceCode = Get(["A", "B"]);
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(WriteConstantLiteral);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("\"A\"u8");
        method.Format.ShouldBeNull();
        method.Provider.ShouldBeNull();

        info.Methods.ShouldBeEmpty();
        info = interceptInfoList.Dequeue();
        method = info.Methods.Dequeue();

        method.Name.ShouldBe(WriteConstantLiteral);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("\"B\"u8");
        method.Format.ShouldBeNull();
        method.Provider.ShouldBeNull();

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
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
            TemplateRenderer.Render(ref writer, string.Empty);
            TemplateRenderer.Render(ref writer, System.String.Empty);
            TemplateRenderer.Render(ref writer, global::System.String.Empty);
            TemplateRenderer.Render(ref writer, S.Empty);
            TemplateRenderer.Render(ref writer, Empty);
            """;
        var (compilation, diagnostics) = Run(SourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        interceptInfoList.Dequeue().Methods.ShouldBeEmpty();
        interceptInfoList.Dequeue().Methods.ShouldBeEmpty();
        interceptInfoList.Dequeue().Methods.ShouldBeEmpty();
        interceptInfoList.Dequeue().Methods.ShouldBeEmpty();
        interceptInfoList.Dequeue().Methods.ShouldBeEmpty();

        interceptInfoList.ShouldBeEmpty();
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
            TemplateRenderer.Render(ref writer, "{{ BytesStaticField }}", in context);
            """;
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(WriteLiteral);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("global::SimpleTextTemplate.Generator.Tests.Core.ByteArrayContextTestData.@BytesStaticField");
        method.Format.ShouldBeNull();
        method.Provider.ShouldBeNull();

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }

    [Fact]
    public void カルチャーにInvariantInfo指定()
    {
        var sourceCode = Get(["{{ DoubleConstantField }}", "{{ DoubleStaticField }}"], nameof(DoubleContextTestData), InvariantInfo);
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(Grow);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("8");
        method.Format.ShouldBeNull();
        method.Provider.ShouldBeNull();

        method = info.Methods.Dequeue();

        method.Name.ShouldBe(DangerousWriteConstantLiteral);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("\"1234.567\"u8");
        method.Format.ShouldBeNull();
        method.Provider.ShouldBeNull();

        info.Methods.ShouldBeEmpty();
        info = interceptInfoList.Dequeue();
        method = info.Methods.Dequeue();

        method.Name.ShouldBe(WriteValue);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("global::SimpleTextTemplate.Generator.Tests.Core.DoubleContextTestData.@DoubleStaticField");
        method.Format.ShouldBe(DefaultKeyword);
        method.Provider.ShouldBe(GlobalInvariantCulture);

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }

    [Fact]
    public void Formatやカルチャー指定省略()
    {
        var sourceCode = Get(["{{ DoubleStaticField:}}", "{{ DoubleStaticField: }}", "{{ DoubleStaticField::}}", "{{ DoubleStaticField:: }}", "{{ DoubleStaticField::  }}"], nameof(DoubleContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(WriteValue);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("global::SimpleTextTemplate.Generator.Tests.Core.DoubleContextTestData.@DoubleStaticField");
        method.Format.ShouldBe(DefaultKeyword);
        method.Provider.ShouldBe(GlobalInvariantCulture);

        info.Methods.ShouldBeEmpty();
        info = interceptInfoList.Dequeue();
        method = info.Methods.Dequeue();

        method.Name.ShouldBe(WriteValue);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("global::SimpleTextTemplate.Generator.Tests.Core.DoubleContextTestData.@DoubleStaticField");
        method.Format.ShouldBe(DefaultKeyword);
        method.Provider.ShouldBe(GlobalInvariantCulture);

        info.Methods.ShouldBeEmpty();
        info = interceptInfoList.Dequeue();
        method = info.Methods.Dequeue();

        method.Name.ShouldBe(WriteValue);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("global::SimpleTextTemplate.Generator.Tests.Core.DoubleContextTestData.@DoubleStaticField");
        method.Format.ShouldBe(DefaultKeyword);
        method.Provider.ShouldBe(GlobalInvariantCulture);

        info.Methods.ShouldBeEmpty();
        info = interceptInfoList.Dequeue();
        method = info.Methods.Dequeue();

        method.Name.ShouldBe(WriteValue);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("global::SimpleTextTemplate.Generator.Tests.Core.DoubleContextTestData.@DoubleStaticField");
        method.Format.ShouldBe(DefaultKeyword);
        method.Provider.ShouldBe(GlobalInvariantCulture);

        info.Methods.ShouldBeEmpty();
        info = interceptInfoList.Dequeue();
        method = info.Methods.Dequeue();

        method.Name.ShouldBe(WriteValue);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("global::SimpleTextTemplate.Generator.Tests.Core.DoubleContextTestData.@DoubleStaticField");
        method.Format.ShouldBe(DefaultKeyword);
        method.Provider.ShouldBe(GlobalInvariantCulture);

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }
}
