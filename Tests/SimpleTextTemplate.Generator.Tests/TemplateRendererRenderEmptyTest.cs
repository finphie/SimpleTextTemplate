using Shouldly;
using SimpleTextTemplate.Generator.Tests.Core;
using SimpleTextTemplate.Generator.Tests.Extensions;
using Xunit;
using static SimpleTextTemplate.Generator.Tests.Constants;
using static SimpleTextTemplate.Generator.Tests.GeneratorRunner;
using static SimpleTextTemplate.Generator.Tests.SourceCode;

namespace SimpleTextTemplate.Generator.Tests;

public sealed class TemplateRendererRenderEmptyTest
{
    [Fact]
    public void 定数()
    {
        var sourceCode = Get("{{ EmptyStringConstantField }}", nameof(EmptyContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        interceptInfoList.Dequeue().Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }

    [Fact]
    public void 複数の定数()
    {
        var sourceCode = Get("{{ EmptyStringConstantField }}{{ EmptyStringConstantField }}", nameof(EmptyContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        interceptInfoList.Dequeue().Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }

    [Fact]
    public void 静的フィールド()
    {
        var sourceCode = Get("{{ EmptyStringStaticField }}", nameof(EmptyContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(WriteString);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("global::SimpleTextTemplate.Generator.Tests.Core.EmptyContextTestData.@EmptyStringStaticField");
        method.Format.ShouldBeNull();
        method.Provider.ShouldBeNull();

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }

    [Fact]
    public void フィールド()
    {
        var sourceCode = Get("{{ EmptyStringField }}", nameof(EmptyContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(WriteString);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@EmptyStringField");
        method.Format.ShouldBeNull();
        method.Provider.ShouldBeNull();

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }

    [Fact]
    public void 静的プロパティ()
    {
        var sourceCode = Get("{{ EmptyStringStaticProperty }}", nameof(EmptyContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(WriteString);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("global::SimpleTextTemplate.Generator.Tests.Core.EmptyContextTestData.@EmptyStringStaticProperty");
        method.Format.ShouldBeNull();
        method.Provider.ShouldBeNull();

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }

    [Fact]
    public void プロパティ()
    {
        var sourceCode = Get("{{ EmptyStringProperty }}", nameof(EmptyContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(WriteString);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@EmptyStringProperty");
        method.Format.ShouldBeNull();
        method.Provider.ShouldBeNull();

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }
}
