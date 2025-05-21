using Shouldly;
using SimpleTextTemplate.Generator.Tests.Core;
using SimpleTextTemplate.Generator.Tests.Extensions;
using Xunit;
using static SimpleTextTemplate.Generator.Tests.Constants;
using static SimpleTextTemplate.Generator.Tests.GeneratorRunner;
using static SimpleTextTemplate.Generator.Tests.SourceCode;

namespace SimpleTextTemplate.Generator.Tests;

public sealed class TemplateRendererRenderCharArrayTest
{
    [Fact]
    public void 静的フィールド()
        => Test(nameof(CharArrayContextTestData.CharsStaticField), true);

    [Fact]
    public void フィールド()
        => Test(nameof(CharArrayContextTestData.CharsField), false);

    [Fact]
    public void 静的プロパティ()
        => Test(nameof(CharArrayContextTestData.CharsStaticProperty), true);

    [Fact]
    public void プロパティ()
        => Test(nameof(CharArrayContextTestData.CharsProperty), false);

    [Fact]
    public void 静的ReadOnlySpanプロパティ()
        => Test(nameof(CharArrayContextTestData.CharsSpanStaticProperty), true);

    [Fact]
    public void ReadOnlySpanプロパティ()
        => Test(nameof(CharArrayContextTestData.CharsSpanProperty), false);

    static void Test(string memberName, bool isStatic)
    {
        var templateText = $$$"""{{ {{{memberName}}} }}""";
        var contextArgument = GetContextArgumentString<CharArrayContextTestData>(memberName, isStatic);

        var sourceCode = Get(templateText, nameof(CharArrayContextTestData));
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
