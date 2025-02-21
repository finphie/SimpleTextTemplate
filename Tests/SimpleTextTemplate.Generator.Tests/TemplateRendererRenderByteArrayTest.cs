using Shouldly;
using SimpleTextTemplate.Generator.Tests.Core;
using SimpleTextTemplate.Generator.Tests.Extensions;
using Xunit;
using static SimpleTextTemplate.Generator.Tests.Constants;
using static SimpleTextTemplate.Generator.Tests.GeneratorRunner;
using static SimpleTextTemplate.Generator.Tests.SourceCode;

namespace SimpleTextTemplate.Generator.Tests;

public sealed class TemplateRendererRenderByteArrayTest
{
    [Fact]
    public void 静的フィールド()
    {
        var sourceCode = Get("{{ BytesStaticField }}", nameof(ByteArrayContextTestData));
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
    public void フィールド()
    {
        var sourceCode = Get("{{ BytesField }}", nameof(ByteArrayContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(WriteLiteral);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@BytesField");
        method.Format.ShouldBeNull();
        method.Provider.ShouldBeNull();

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }

    [Fact]
    public void 静的プロパティ()
    {
        var sourceCode = Get("{{ BytesStaticProperty }}", nameof(ByteArrayContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(WriteLiteral);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("global::SimpleTextTemplate.Generator.Tests.Core.ByteArrayContextTestData.@BytesStaticProperty");
        method.Format.ShouldBeNull();
        method.Provider.ShouldBeNull();

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }

    [Fact]
    public void プロパティ()
    {
        var sourceCode = Get("{{ BytesProperty }}", nameof(ByteArrayContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(WriteLiteral);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@BytesProperty");
        method.Format.ShouldBeNull();
        method.Provider.ShouldBeNull();

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }

    [Fact]
    public void 静的ReadOnlySpanプロパティ()
    {
        var sourceCode = Get("{{ BytesSpanStaticProperty }}", nameof(ByteArrayContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(Grow);
        method.Text.Count.ShouldBe(2);
        method.Text[0].ShouldBe("0");
        method.Text[1].ShouldBe("global::SimpleTextTemplate.Generator.Tests.Core.ByteArrayContextTestData.@BytesSpanStaticProperty.Length");
        method.Format.ShouldBeNull();
        method.Provider.ShouldBeNull();

        method = info.Methods.Dequeue();

        method.Name.ShouldBe(DangerousWriteLiteral);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("global::SimpleTextTemplate.Generator.Tests.Core.ByteArrayContextTestData.@BytesSpanStaticProperty");
        method.Format.ShouldBeNull();
        method.Provider.ShouldBeNull();

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }

    [Fact]
    public void ReadOnlySpanプロパティ()
    {
        var sourceCode = Get("{{ BytesSpanProperty }}", nameof(ByteArrayContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(Grow);
        method.Text.Count.ShouldBe(2);
        method.Text[0].ShouldBe("0");
        method.Text[1].ShouldBe("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@BytesSpanProperty.Length");
        method.Format.ShouldBeNull();
        method.Provider.ShouldBeNull();

        method = info.Methods.Dequeue();

        method.Name.ShouldBe(DangerousWriteLiteral);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@BytesSpanProperty");
        method.Format.ShouldBeNull();
        method.Provider.ShouldBeNull();

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }
}
