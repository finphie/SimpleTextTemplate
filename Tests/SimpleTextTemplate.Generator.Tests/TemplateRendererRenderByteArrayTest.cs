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
        => Test(nameof(ByteArrayContextTestData.BytesStaticField), true);

    [Fact]
    public void フィールド()
        => Test(nameof(ByteArrayContextTestData.BytesField), false);

    [Fact]
    public void 静的プロパティ()
        => Test(nameof(ByteArrayContextTestData.BytesStaticProperty), true);

    [Fact]
    public void プロパティ()
        => Test(nameof(ByteArrayContextTestData.BytesProperty), false);

    [Fact]
    public void 静的ReadOnlySpanプロパティ()
        => Test(nameof(ByteArrayContextTestData.BytesSpanStaticProperty), true);

    [Fact]
    public void ReadOnlySpanプロパティ()
        => Test(nameof(ByteArrayContextTestData.BytesSpanProperty), false);

    static void Test(string memberName, bool isStatic)
    {
        var templateText = $$$"""{{ {{{memberName}}} }}""";
        var contextArgument = GetContextArgumentString<ByteArrayContextTestData>(memberName, isStatic);

        var sourceCode = Get(templateText, nameof(ByteArrayContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(Grow);
        method.Text.Count.ShouldBe(2);
        method.Text[0].ShouldBe("0");
        method.Text[1].ShouldBe($"{contextArgument}.Length");
        method.Format.ShouldBeNull();
        method.Provider.ShouldBeNull();

        method = info.Methods.Dequeue();

        method.Name.ShouldBe(DangerousWriteLiteral);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe(contextArgument);
        method.Format.ShouldBeNull();
        method.Provider.ShouldBeNull();

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }
}
