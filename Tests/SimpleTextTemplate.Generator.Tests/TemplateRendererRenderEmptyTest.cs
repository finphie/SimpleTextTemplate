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
        => Test(nameof(EmptyContextTestData.EmptyStringStaticField), true);

    [Fact]
    public void フィールド()
        => Test(nameof(EmptyContextTestData.EmptyStringField), false);

    [Fact]
    public void 静的プロパティ()
        => Test(nameof(EmptyContextTestData.EmptyStringStaticProperty), true);

    [Fact]
    public void プロパティ()
        => Test(nameof(EmptyContextTestData.EmptyStringProperty), false);

    static void Test(string memberName, bool isStatic)
    {
        var templateText = $$$"""{{ {{{memberName}}} }}""";
        var contextArgument = GetContextArgumentString<EmptyContextTestData>(memberName, isStatic);

        var sourceCode = Get(templateText, nameof(EmptyContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(Grow);
        method.Text.Count.ShouldBe(3);
        method.Text[0].ShouldBe("0");
        method.Text[1].ShouldBe(Utf8GetMaxByteCount);
        method.Text[2].ShouldBe($"{contextArgument}.Length");
        method.Format.ShouldBeNull();
        method.Provider.ShouldBeNull();

        method = info.Methods.Dequeue();

        method.Name.ShouldBe(DangerousWriteString);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe(contextArgument);
        method.Format.ShouldBeNull();
        method.Provider.ShouldBeNull();

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }
}
