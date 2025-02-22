using Shouldly;
using SimpleTextTemplate.Generator.Tests.Core;
using SimpleTextTemplate.Generator.Tests.Extensions;
using Xunit;
using static SimpleTextTemplate.Generator.Tests.Constants;
using static SimpleTextTemplate.Generator.Tests.GeneratorRunner;
using static SimpleTextTemplate.Generator.Tests.SourceCode;

namespace SimpleTextTemplate.Generator.Tests;

public sealed class TemplateRendererRenderEnumTest
{
    [Fact]
    public void 定数()
    {
        var sourceCode = Get("{{ EnumConstantField }}", nameof(EnumContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(Grow);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("5");
        method.Format.ShouldBeNull();
        method.Provider.ShouldBeNull();

        method = info.Methods.Dequeue();

        method.Name.ShouldBe(DangerousWriteConstantLiteral);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("\"Test1\"u8");
        method.Format.ShouldBeNull();
        method.Provider.ShouldBeNull();

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }

    [Fact]
    public void 定数_FormatにD指定()
    {
        var sourceCode = Get(["{{ EnumConstantField:D }}", "{{ EnumConstantField:d }}"], nameof(EnumContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(Grow);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("1");
        method.Format.ShouldBeNull();
        method.Provider.ShouldBeNull();

        method = info.Methods.Dequeue();

        method.Name.ShouldBe(DangerousWriteConstantLiteral);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("\"1\"u8");
        method.Format.ShouldBeNull();
        method.Provider.ShouldBeNull();

        info.Methods.ShouldBeEmpty();
        info = interceptInfoList.Dequeue();
        method = info.Methods.Dequeue();

        method.Name.ShouldBe(Grow);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("1");
        method.Format.ShouldBeNull();
        method.Provider.ShouldBeNull();

        method = info.Methods.Dequeue();

        method.Name.ShouldBe(DangerousWriteConstantLiteral);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("\"1\"u8");
        method.Format.ShouldBeNull();
        method.Provider.ShouldBeNull();

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }

    [Fact]
    public void 定数_FormatにD以外指定()
    {
        var sourceCode = Get("{{ EnumConstantField:G }}", nameof(EnumContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(WriteEnum);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("global::SimpleTextTemplate.Generator.Tests.Core.EnumContextTestData.@EnumConstantField");
        method.Format.ShouldBe("\"G\"");
        method.Provider.ShouldBeNull();

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }

    [Fact]
    public void 定数が無効な値()
    {
        var sourceCode = Get("{{ EnumConstantFieldInvalidNumber }}", nameof(EnumContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(Grow);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("2");
        method.Format.ShouldBeNull();
        method.Provider.ShouldBeNull();

        method = info.Methods.Dequeue();

        method.Name.ShouldBe(DangerousWriteConstantLiteral);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("\"99\"u8");
        method.Format.ShouldBeNull();
        method.Provider.ShouldBeNull();

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }

    [Fact]
    public void Flags属性を付与したEnumの定数()
    {
        var sourceCode = Get("{{ FlagEnumConstantField }}", nameof(EnumContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(WriteEnum);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("global::SimpleTextTemplate.Generator.Tests.Core.EnumContextTestData.@FlagEnumConstantField");
        method.Format.ShouldBe(DefaultKeyword);
        method.Provider.ShouldBeNull();

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }

    [Fact]
    public void Flags属性を付与したEnumの定数_Format指定()
    {
        var sourceCode = Get("{{ FlagEnumConstantField:D }}", nameof(EnumContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(WriteEnum);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("global::SimpleTextTemplate.Generator.Tests.Core.EnumContextTestData.@FlagEnumConstantField");
        method.Format.ShouldBe("\"D\"");
        method.Provider.ShouldBeNull();

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }

    [Fact]
    public void 複数の定数()
    {
        var sourceCode = Get("{{ EnumConstantField }}{{ EnumConstantField }}", nameof(EnumContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(Grow);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("10");
        method.Format.ShouldBeNull();
        method.Provider.ShouldBeNull();

        method = info.Methods.Dequeue();

        method.Name.ShouldBe(DangerousWriteConstantLiteral);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("\"Test1Test1\"u8");
        method.Format.ShouldBeNull();
        method.Provider.ShouldBeNull();

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }

    [Fact]
    public void 静的フィールド()
    {
        var sourceCode = Get("{{ EnumStaticField }}", nameof(EnumContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(WriteEnum);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("global::SimpleTextTemplate.Generator.Tests.Core.EnumContextTestData.@EnumStaticField");
        method.Format.ShouldBe(DefaultKeyword);
        method.Provider.ShouldBeNull();

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }

    [Fact]
    public void 静的フィールド_Format指定()
    {
        var sourceCode = Get("{{ EnumStaticField:D }}", nameof(EnumContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(WriteEnum);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("global::SimpleTextTemplate.Generator.Tests.Core.EnumContextTestData.@EnumStaticField");
        method.Format.ShouldBe("\"D\"");
        method.Provider.ShouldBeNull();

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }

    [Fact]
    public void フィールド()
    {
        var sourceCode = Get("{{ EnumField }}", nameof(EnumContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(WriteEnum);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@EnumField");
        method.Format.ShouldBe(DefaultKeyword);
        method.Provider.ShouldBeNull();

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }

    [Fact]
    public void フィールド_Format指定()
    {
        var sourceCode = Get("{{ EnumField:D }}", nameof(EnumContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(WriteEnum);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@EnumField");
        method.Format.ShouldBe("\"D\"");
        method.Provider.ShouldBeNull();

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }


    [Fact]
    public void 静的プロパティ()
    {
        var sourceCode = Get("{{ EnumStaticProperty }}", nameof(EnumContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(WriteEnum);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("global::SimpleTextTemplate.Generator.Tests.Core.EnumContextTestData.@EnumStaticProperty");
        method.Format.ShouldBe(DefaultKeyword);
        method.Provider.ShouldBeNull();

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }

    [Fact]
    public void 静的プロパティ_Format指定()
    {
        var sourceCode = Get("{{ EnumStaticProperty:D }}", nameof(EnumContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(WriteEnum);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("global::SimpleTextTemplate.Generator.Tests.Core.EnumContextTestData.@EnumStaticProperty");
        method.Format.ShouldBe("\"D\"");
        method.Provider.ShouldBeNull();

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }

    [Fact]
    public void プロパティ()
    {
        var sourceCode = Get("{{ EnumProperty }}", nameof(EnumContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(WriteEnum);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@EnumProperty");
        method.Format.ShouldBe(DefaultKeyword);
        method.Provider.ShouldBeNull();

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }

    [Fact]
    public void プロパティ_Format指定()
    {
        var sourceCode = Get("{{ EnumProperty:D }}", nameof(EnumContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(WriteEnum);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@EnumProperty");
        method.Format.ShouldBe("\"D\"");
        method.Provider.ShouldBeNull();

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }
}
