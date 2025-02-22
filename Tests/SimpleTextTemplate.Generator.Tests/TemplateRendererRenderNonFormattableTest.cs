using Shouldly;
using SimpleTextTemplate.Generator.Tests.Core;
using SimpleTextTemplate.Generator.Tests.Extensions;
using Xunit;
using static SimpleTextTemplate.Generator.Tests.Constants;
using static SimpleTextTemplate.Generator.Tests.GeneratorRunner;
using static SimpleTextTemplate.Generator.Tests.SourceCode;

namespace SimpleTextTemplate.Generator.Tests;

public sealed class TemplateRendererRenderNonFormattableTest
{
    [Fact]
    public void 静的フィールド()
    {
        var sourceCode = Get("{{ NonFormattableStaticField }}", nameof(NonFormattableContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(WriteValue);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("global::SimpleTextTemplate.Generator.Tests.Core.NonFormattableContextTestData.@NonFormattableStaticField");
        method.Format.ShouldBe(DefaultKeyword);
        method.Provider.ShouldBe(GlobalInvariantCulture);

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }

    [Fact]
    public void フィールド()
    {
        var sourceCode = Get("{{ NonFormattableField }}", nameof(NonFormattableContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(WriteValue);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@NonFormattableField");
        method.Format.ShouldBe(DefaultKeyword);
        method.Provider.ShouldBe(GlobalInvariantCulture);

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }

    [Fact]
    public void 静的プロパティ()
    {
        var sourceCode = Get("{{ NonFormattableStaticProperty }}", nameof(NonFormattableContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(WriteValue);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("global::SimpleTextTemplate.Generator.Tests.Core.NonFormattableContextTestData.@NonFormattableStaticProperty");
        method.Format.ShouldBe(DefaultKeyword);
        method.Provider.ShouldBe(GlobalInvariantCulture);

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }

    [Fact]
    public void プロパティ()
    {
        var sourceCode = Get("{{ NonFormattableProperty }}", nameof(NonFormattableContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(WriteValue);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@NonFormattableProperty");
        method.Format.ShouldBe(DefaultKeyword);
        method.Provider.ShouldBe(GlobalInvariantCulture);

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }
}
