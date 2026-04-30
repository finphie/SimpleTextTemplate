using SimpleTextTemplate.Generator.Tests.Extensions;
using SimpleTextTemplate.Tests.TestData;
using static SimpleTextTemplate.Generator.Tests.Constants;
using static SimpleTextTemplate.Generator.Tests.GeneratorRunner;
using static SimpleTextTemplate.Generator.Tests.SourceCode;

namespace SimpleTextTemplate.Generator.Tests;

public sealed class TemplateRendererRenderTest
{
    [Test]
    public async Task 識別子なし()
    {
        var sourceCode = Get(["A", "B"]);
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(WriteConstantLiteral);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("\"A\"u8");
        await Assert.That(method.Format).IsNull();
        await Assert.That(method.Provider).IsNull();

        await Assert.That(info.Methods).IsEmpty();
        info = interceptInfoList.Dequeue();
        method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(WriteConstantLiteral);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("\"B\"u8");
        await Assert.That(method.Format).IsNull();
        await Assert.That(method.Provider).IsNull();

        await Assert.That(info.Methods).IsEmpty();
        await Assert.That(interceptInfoList).IsEmpty();
    }

    [Test]
    public async Task StringEmpty()
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
        var (compilation, diagnostics) = await RunAsync(SourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        await Assert.That(interceptInfoList.Dequeue().Methods).IsEmpty();
        await Assert.That(interceptInfoList.Dequeue().Methods).IsEmpty();
        await Assert.That(interceptInfoList.Dequeue().Methods).IsEmpty();
        await Assert.That(interceptInfoList.Dequeue().Methods).IsEmpty();
        await Assert.That(interceptInfoList.Dequeue().Methods).IsEmpty();

        await Assert.That(interceptInfoList).IsEmpty();
    }

    [Test]
    public async Task トップレベルステートメント()
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
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(Grow);
        await Assert.That(method.Text.Count).IsEqualTo(2);
        await Assert.That(method.Text[0]).IsEqualTo("0");
        await Assert.That(method.Text[1]).IsEqualTo("global::SimpleTextTemplate.Generator.Tests.Core.ByteArrayContextTestData.@BytesStaticField.Length");
        await Assert.That(method.Format).IsNull();
        await Assert.That(method.Provider).IsNull();

        method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(DangerousWriteLiteral);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("global::SimpleTextTemplate.Generator.Tests.Core.ByteArrayContextTestData.@BytesStaticField");
        await Assert.That(method.Format).IsNull();
        await Assert.That(method.Provider).IsNull();

        await Assert.That(info.Methods).IsEmpty();
        await Assert.That(interceptInfoList).IsEmpty();
    }

    [Test]
    public async Task カルチャーにInvariantInfo指定()
    {
        var sourceCode = Get(["{{ DoubleConstantField }}", "{{ DoubleStaticField }}"], nameof(DoubleContextTestData), InvariantInfo);
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(Grow);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("8");
        await Assert.That(method.Format).IsNull();
        await Assert.That(method.Provider).IsNull();

        method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(DangerousWriteConstantLiteral);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("\"1234.567\"u8");
        await Assert.That(method.Format).IsNull();
        await Assert.That(method.Provider).IsNull();

        await Assert.That(info.Methods).IsEmpty();
        info = interceptInfoList.Dequeue();
        method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(WriteValue);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("global::SimpleTextTemplate.Generator.Tests.Core.DoubleContextTestData.@DoubleStaticField");
        await Assert.That(method.Format).IsEqualTo(DefaultKeyword);
        await Assert.That(method.Provider).IsEqualTo(GlobalInvariantCulture);

        await Assert.That(info.Methods).IsEmpty();
        await Assert.That(interceptInfoList).IsEmpty();
    }

    [Test]
    public async Task Formatやカルチャー指定省略()
    {
        var sourceCode = Get(["{{ DoubleStaticField:}}", "{{ DoubleStaticField: }}", "{{ DoubleStaticField::}}", "{{ DoubleStaticField:: }}", "{{ DoubleStaticField::  }}"], nameof(DoubleContextTestData));
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(WriteValue);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("global::SimpleTextTemplate.Generator.Tests.Core.DoubleContextTestData.@DoubleStaticField");
        await Assert.That(method.Format).IsEqualTo(DefaultKeyword);
        await Assert.That(method.Provider).IsEqualTo(GlobalInvariantCulture);

        await Assert.That(info.Methods).IsEmpty();
        info = interceptInfoList.Dequeue();
        method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(WriteValue);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("global::SimpleTextTemplate.Generator.Tests.Core.DoubleContextTestData.@DoubleStaticField");
        await Assert.That(method.Format).IsEqualTo(DefaultKeyword);
        await Assert.That(method.Provider).IsEqualTo(GlobalInvariantCulture);

        await Assert.That(info.Methods).IsEmpty();
        info = interceptInfoList.Dequeue();
        method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(WriteValue);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("global::SimpleTextTemplate.Generator.Tests.Core.DoubleContextTestData.@DoubleStaticField");
        await Assert.That(method.Format).IsEqualTo(DefaultKeyword);
        await Assert.That(method.Provider).IsEqualTo(GlobalInvariantCulture);

        await Assert.That(info.Methods).IsEmpty();
        info = interceptInfoList.Dequeue();
        method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(WriteValue);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("global::SimpleTextTemplate.Generator.Tests.Core.DoubleContextTestData.@DoubleStaticField");
        await Assert.That(method.Format).IsEqualTo(DefaultKeyword);
        await Assert.That(method.Provider).IsEqualTo(GlobalInvariantCulture);

        await Assert.That(info.Methods).IsEmpty();
        info = interceptInfoList.Dequeue();
        method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(WriteValue);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("global::SimpleTextTemplate.Generator.Tests.Core.DoubleContextTestData.@DoubleStaticField");
        await Assert.That(method.Format).IsEqualTo(DefaultKeyword);
        await Assert.That(method.Provider).IsEqualTo(GlobalInvariantCulture);

        await Assert.That(info.Methods).IsEmpty();
        await Assert.That(interceptInfoList).IsEmpty();
    }

    [Test]
    public async Task 複雑なテンプレート文字列()
    {
        const string Text = """
            A{{ ConstantValue }}{{ ConstantValue }}B{{ ConstantValue }}{{ StringValue }}{{ ConstantValue }}{{ ConstantValue }}{{ Utf16Value }}{{ ConstantValue }}{{ Utf8Value }}{{ DoubleValue }}
            A{{ ConstantValue }}{{ ConstantValue }}B{{ ConstantValue }}{{ StringValue }}{{ ConstantValue }}{{ ConstantValue }}{{ Utf16Value }}{{ ConstantValue }}{{ Utf8Value }}
            """;
        var sourceCode = Get(Text.Replace("\r\n", string.Empty, StringComparison.Ordinal), nameof(ContextTestData));
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();
        var info = interceptInfoList.Dequeue();

        await Test(info);

        // {{ DoubleValue }}
        var method = info.Methods.Dequeue();
        await Assert.That(method.Name).IsEqualTo(WriteValue);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("global::SimpleTextTemplate.Generator.Tests.Core.ContextTestData.@DoubleValue");
        await Assert.That(method.Format).IsEqualTo(DefaultKeyword);
        await Assert.That(method.Provider).IsEqualTo(GlobalInvariantCulture);

        await Test(info);

        await Assert.That(info.Methods).IsEmpty();
        await Assert.That(interceptInfoList).IsEmpty();

        static async Task Test(InterceptInfo info)
        {
            // Grow(
            //     (1 + 14 + 14 + 1 + 14)
            //     + GetUtf8MaxByteCount(StringValue.Length)
            //     + (14 + 14)
            //     + GetUtf8MaxByteCount(Utf16Value.Length)
            //     + 14
            //     + Utf8Value.Length)
            var method = info.Methods.Dequeue();
            await Assert.That(method.Name).IsEqualTo(Grow);
            await Assert.That(method.Text.Count).IsEqualTo(5);
            await Assert.That(method.Text[0]).IsEqualTo("86");
            await Assert.That(method.Text[1]).IsEqualTo("global::SimpleTextTemplate.Generator.Tests.Core.ContextTestData.@Utf8Value.Length");
            await Assert.That(method.Text[2]).IsEqualTo(Utf8GetMaxByteCount);
            await Assert.That(method.Text[3]).IsEqualTo("global::SimpleTextTemplate.Generator.Tests.Core.ContextTestData.@StringValue.Length");
            await Assert.That(method.Text[4]).IsEqualTo("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@Utf16Value.Length");
            await Assert.That(method.Format).IsNull();
            await Assert.That(method.Provider).IsNull();

            // A{{ ConstantValue }}{{ ConstantValue }}B{{ ConstantValue }}
            method = info.Methods.Dequeue();
            await Assert.That(method.Name).IsEqualTo(DangerousWriteConstantLiteral);
            await Assert.That(method.Text.Count).IsEqualTo(1);
            await Assert.That(method.Text[0]).IsEqualTo("\"A_ConstantValue_ConstantValueB_ConstantValue\"u8");
            await Assert.That(method.Format).IsNull();
            await Assert.That(method.Provider).IsNull();

            // {{ StringValue }}
            method = info.Methods.Dequeue();
            await Assert.That(method.Name).IsEqualTo(DangerousWriteString);
            await Assert.That(method.Text.Count).IsEqualTo(1);
            await Assert.That(method.Text[0]).IsEqualTo("global::SimpleTextTemplate.Generator.Tests.Core.ContextTestData.@StringValue");
            await Assert.That(method.Format).IsNull();
            await Assert.That(method.Provider).IsNull();

            // {{ ConstantValue }}{{ ConstantValue }}
            method = info.Methods.Dequeue();
            await Assert.That(method.Name).IsEqualTo(DangerousWriteConstantLiteral);
            await Assert.That(method.Text.Count).IsEqualTo(1);
            await Assert.That(method.Text[0]).IsEqualTo("\"_ConstantValue_ConstantValue\"u8");
            await Assert.That(method.Format).IsNull();
            await Assert.That(method.Provider).IsNull();

            // {{ Utf16Value }}
            method = info.Methods.Dequeue();
            await Assert.That(method.Name).IsEqualTo(DangerousWriteString);
            await Assert.That(method.Text.Count).IsEqualTo(1);
            await Assert.That(method.Text[0]).IsEqualTo("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@Utf16Value");
            await Assert.That(method.Format).IsNull();
            await Assert.That(method.Provider).IsNull();

            // {{ ConstantValue }}
            method = info.Methods.Dequeue();
            await Assert.That(method.Name).IsEqualTo(DangerousWriteConstantLiteral);
            await Assert.That(method.Text.Count).IsEqualTo(1);
            await Assert.That(method.Text[0]).IsEqualTo("\"_ConstantValue\"u8");
            await Assert.That(method.Format).IsNull();
            await Assert.That(method.Provider).IsNull();

            // {{ Utf8Value }}
            method = info.Methods.Dequeue();
            await Assert.That(method.Name).IsEqualTo(DangerousWriteLiteral);
            await Assert.That(method.Text.Count).IsEqualTo(1);
            await Assert.That(method.Text[0]).IsEqualTo("global::SimpleTextTemplate.Generator.Tests.Core.ContextTestData.@Utf8Value");
            await Assert.That(method.Format).IsNull();
            await Assert.That(method.Provider).IsNull();
        }
    }
}
