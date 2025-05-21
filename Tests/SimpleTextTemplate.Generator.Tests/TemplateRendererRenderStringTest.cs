using System.Globalization;
using Shouldly;
using SimpleTextTemplate.Generator.Tests.Core;
using SimpleTextTemplate.Generator.Tests.Extensions;
using Xunit;
using static SimpleTextTemplate.Generator.Tests.Constants;
using static SimpleTextTemplate.Generator.Tests.GeneratorRunner;
using static SimpleTextTemplate.Generator.Tests.SourceCode;

namespace SimpleTextTemplate.Generator.Tests;

public sealed class TemplateRendererRenderStringTest
{
    [Fact]
    public void 定数()
    {
        var sourceCode = Get("{{ StringConstantField }}", nameof(StringContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(Grow);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe(StringContextTestData.StringConstantField.Length.ToString(CultureInfo.InvariantCulture));
        method.Format.ShouldBeNull();
        method.Provider.ShouldBeNull();

        method = info.Methods.Dequeue();

        method.Name.ShouldBe(DangerousWriteConstantLiteral);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("\"_StringConstantField\"u8");
        method.Format.ShouldBeNull();
        method.Provider.ShouldBeNull();

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }

    [Fact]
    public void 複数の定数()
    {
        var sourceCode = Get("{{ StringConstantField }}{{ StringConstantField }}", nameof(StringContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(Grow);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe((StringContextTestData.StringConstantField.Length * 2).ToString(CultureInfo.InvariantCulture));
        method.Format.ShouldBeNull();
        method.Provider.ShouldBeNull();

        method = info.Methods.Dequeue();

        method.Name.ShouldBe(DangerousWriteConstantLiteral);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("\"_StringConstantField_StringConstantField\"u8");
        method.Format.ShouldBeNull();
        method.Provider.ShouldBeNull();

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }

    [Fact]
    public void 静的フィールド()
        => Test(nameof(StringContextTestData.StringStaticField), true);

    [Fact]
    public void フィールド()
        => Test(nameof(StringContextTestData.StringField), false);

    [Fact]
    public void 静的プロパティ()
        => Test(nameof(StringContextTestData.StringStaticProperty), true);

    [Fact]
    public void プロパティ()
        => Test(nameof(StringContextTestData.StringProperty), false);

    static void Test(string memberName, bool isStatic)
    {
        var templateText = $$$"""{{ {{{memberName}}} }}""";
        var contextArgument = GetContextArgumentString<StringContextTestData>(memberName, isStatic);

        var sourceCode = Get(templateText, nameof(StringContextTestData));
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
