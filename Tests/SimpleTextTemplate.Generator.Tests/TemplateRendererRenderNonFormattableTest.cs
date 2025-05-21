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
        => Test(nameof(NonFormattableContextTestData.NonFormattableStaticField), true);

    [Fact]
    public void フィールド()
        => Test(nameof(NonFormattableContextTestData.NonFormattableField), false);

    [Fact]
    public void 静的プロパティ()
        => Test(nameof(NonFormattableContextTestData.NonFormattableStaticProperty), true);

    [Fact]
    public void プロパティ()
        => Test(nameof(NonFormattableContextTestData.NonFormattableProperty), false);

    static void Test(string memberName, bool isStatic)
    {
        var templateText = $$$"""{{ {{{memberName}}} }}""";
        var contextArgument = GetContextArgumentString<NonFormattableContextTestData>(memberName, isStatic);

        var sourceCode = Get(templateText, nameof(NonFormattableContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(WriteValue);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe(contextArgument);
        method.Format.ShouldBe(DefaultKeyword);
        method.Provider.ShouldBe(GlobalInvariantCulture);

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }
}
