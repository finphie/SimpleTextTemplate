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
        var sourceCode = Get("{{ StringConstantField }}{{ StringStaticField }}", nameof(StringContextTestData));
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
    {
        var sourceCode = Get("{{ StringStaticField }}", nameof(StringContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(WriteString);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("global::SimpleTextTemplate.Generator.Tests.Core.StringContextTestData.@StringStaticField");
        method.Format.ShouldBeNull();
        method.Provider.ShouldBeNull();

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }

    [Fact]
    public void フィールド()
    {
        var sourceCode = Get("{{ StringField }}", nameof(StringContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(WriteString);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@StringField");
        method.Format.ShouldBeNull();
        method.Provider.ShouldBeNull();

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }

    [Fact]
    public void 静的プロパティ()
    {
        var sourceCode = Get("{{ StringStaticProperty }}", nameof(StringContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(WriteString);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("global::SimpleTextTemplate.Generator.Tests.Core.StringContextTestData.@StringStaticProperty");
        method.Format.ShouldBeNull();
        method.Provider.ShouldBeNull();

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }

    [Fact]
    public void プロパティ()
    {
        var sourceCode = Get("{{ StringProperty }}", nameof(StringContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(WriteString);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@StringProperty");
        method.Format.ShouldBeNull();
        method.Provider.ShouldBeNull();

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }
}
