using Shouldly;
using SimpleTextTemplate.Generator.Tests.Core;
using SimpleTextTemplate.Generator.Tests.Extensions;
using Xunit;
using static SimpleTextTemplate.Generator.Tests.Constants;
using static SimpleTextTemplate.Generator.Tests.GeneratorRunner;
using static SimpleTextTemplate.Generator.Tests.SourceCode;

namespace SimpleTextTemplate.Generator.Tests;

public sealed class TemplateRendererRenderTest
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

        method.Name.ShouldBe(Grow);
        method.Text.Count.ShouldBe(2);
        method.Text[0].ShouldBe("0");
        method.Text[1].ShouldBe("global::SimpleTextTemplate.Generator.Tests.Core.ByteArrayContextTestData.@BytesStaticField.Length");
        method.Format.ShouldBeNull();
        method.Provider.ShouldBeNull();

        method = info.Methods.Dequeue();

        method.Name.ShouldBe(DangerousWriteLiteral);
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

    [Fact]
    public void 複雑なテンプレート文字列()
    {
        const string Text = """
            A{{ ConstantValue }}{{ ConstantValue }}B{{ ConstantValue }}{{ StringValue }}{{ ConstantValue }}{{ ConstantValue }}{{ Utf16Value }}{{ ConstantValue }}{{ Utf8Value }}{{ DoubleValue }}
            A{{ ConstantValue }}{{ ConstantValue }}B{{ ConstantValue }}{{ StringValue }}{{ ConstantValue }}{{ ConstantValue }}{{ Utf16Value }}{{ ConstantValue }}{{ Utf8Value }}
            """;
        var sourceCode = Get(Text.Replace("\r\n", string.Empty), nameof(ContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();
        var info = interceptInfoList.Dequeue();

        Test(info);

        // {{ DoubleValue }}
        var method = info.Methods.Dequeue();
        method.Name.ShouldBe(WriteValue);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("global::SimpleTextTemplate.Generator.Tests.Core.ContextTestData.@DoubleValue");
        method.Format.ShouldBe(DefaultKeyword);
        method.Provider.ShouldBe(GlobalInvariantCulture);

        Test(info);

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();

        static void Test(InterceptInfo info)
        {
            // Grow(
            //     (1 + 14 + 14 + 1 + 14)
            //     + GetUtf8MaxByteCount(StringValue.Length)
            //     + (14 + 14)
            //     + GetUtf8MaxByteCount(Utf16Value.Length)
            //     + 14
            //     + Utf8Value.Length)
            var method = info.Methods.Dequeue();
            method.Name.ShouldBe(Grow);
            method.Text.Count.ShouldBe(5);
            method.Text[0].ShouldBe("86");
            method.Text[1].ShouldBe("global::SimpleTextTemplate.Generator.Tests.Core.ContextTestData.@Utf8Value.Length");
            method.Text[2].ShouldBe(Utf8GetMaxByteCount);
            method.Text[3].ShouldBe("global::SimpleTextTemplate.Generator.Tests.Core.ContextTestData.@StringValue.Length");
            method.Text[4].ShouldBe("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@Utf16Value.Length");
            method.Format.ShouldBeNull();
            method.Provider.ShouldBeNull();

            // A{{ ConstantValue }}{{ ConstantValue }}B{{ ConstantValue }}
            method = info.Methods.Dequeue();
            method.Name.ShouldBe(DangerousWriteConstantLiteral);
            method.Text.Count.ShouldBe(1);
            method.Text[0].ShouldBe("\"A_ConstantValue_ConstantValueB_ConstantValue\"u8");
            method.Format.ShouldBeNull();
            method.Provider.ShouldBeNull();

            // {{ StringValue }}
            method = info.Methods.Dequeue();
            method.Name.ShouldBe(DangerousWriteString);
            method.Text.Count.ShouldBe(1);
            method.Text[0].ShouldBe("global::SimpleTextTemplate.Generator.Tests.Core.ContextTestData.@StringValue");
            method.Format.ShouldBeNull();
            method.Provider.ShouldBeNull();

            // {{ ConstantValue }}{{ ConstantValue }}
            method = info.Methods.Dequeue();
            method.Name.ShouldBe(DangerousWriteConstantLiteral);
            method.Text.Count.ShouldBe(1);
            method.Text[0].ShouldBe("\"_ConstantValue_ConstantValue\"u8");
            method.Format.ShouldBeNull();
            method.Provider.ShouldBeNull();

            // {{ Utf16Value }}
            method = info.Methods.Dequeue();
            method.Name.ShouldBe(DangerousWriteString);
            method.Text.Count.ShouldBe(1);
            method.Text[0].ShouldBe("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@Utf16Value");
            method.Format.ShouldBeNull();
            method.Provider.ShouldBeNull();

            // {{ ConstantValue }}
            method = info.Methods.Dequeue();
            method.Name.ShouldBe(DangerousWriteConstantLiteral);
            method.Text.Count.ShouldBe(1);
            method.Text[0].ShouldBe("\"_ConstantValue\"u8");
            method.Format.ShouldBeNull();
            method.Provider.ShouldBeNull();

            // {{ Utf8Value }}
            method = info.Methods.Dequeue();
            method.Name.ShouldBe(DangerousWriteLiteral);
            method.Text.Count.ShouldBe(1);
            method.Text[0].ShouldBe("global::SimpleTextTemplate.Generator.Tests.Core.ContextTestData.@Utf8Value");
            method.Format.ShouldBeNull();
            method.Provider.ShouldBeNull();
        }
    }
}
