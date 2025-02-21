using Shouldly;
using SimpleTextTemplate.Generator.Tests.Core;
using SimpleTextTemplate.Generator.Tests.Extensions;
using Xunit;
using static SimpleTextTemplate.Generator.Tests.Constants;
using static SimpleTextTemplate.Generator.Tests.GeneratorRunner;
using static SimpleTextTemplate.Generator.Tests.SourceCode;

namespace SimpleTextTemplate.Generator.Tests;

public sealed class TemplateRendererRenderDoubleTest
{
    [Fact]
    public void 定数()
    {
        var sourceCode = Get("{{ DoubleConstantField }}", nameof(DoubleContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(Grow);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("8");
        method.Format.ShouldBeNull();
        method.Provider.ShouldBeNull();

        method = info.Methods.Dequeue();

        method.Name.ShouldBe(DangerousWriteConstantLiteral);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("\"1234.567\"u8");
        method.Format.ShouldBeNull();
        method.Provider.ShouldBeNull();

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }

    [Fact]
    public void 定数_Format指定()
    {
        var sourceCode = Get("{{ DoubleConstantField:N3 }}", nameof(DoubleContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(Grow);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("9");
        method.Format.ShouldBeNull();
        method.Provider.ShouldBeNull();

        method = info.Methods.Dequeue();

        method.Name.ShouldBe(DangerousWriteConstantLiteral);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("\"1,234.567\"u8");
        method.Format.ShouldBeNull();
        method.Provider.ShouldBeNull();

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }

    [Fact]
    public void 定数_特定カルチャー指定()
    {
        var sourceCode = Get("{{ DoubleConstantField::es-ES }}", nameof(DoubleContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(Grow);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("8");
        method.Format.ShouldBeNull();
        method.Provider.ShouldBeNull();

        method = info.Methods.Dequeue();

        method.Name.ShouldBe(DangerousWriteConstantLiteral);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("\"1234,567\"u8");
        method.Format.ShouldBeNull();
        method.Provider.ShouldBeNull();

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }

    [Fact]
    public void 定数_Formatと特定カルチャー指定()
    {
        var sourceCode = Get("{{ DoubleConstantField:N3:es-ES }}", nameof(DoubleContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(Grow);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("9");
        method.Format.ShouldBeNull();
        method.Provider.ShouldBeNull();

        method = info.Methods.Dequeue();

        method.Name.ShouldBe(DangerousWriteConstantLiteral);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("\"1.234,567\"u8");
        method.Format.ShouldBeNull();
        method.Provider.ShouldBeNull();

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }

    [Fact]
    public void 定数_メソッド引数でInvariantCulture指定()
    {
        foreach (var culture in InvariantCultureList)
        {
            var sourceCode = Get("{{ DoubleConstantField }}", nameof(DoubleContextTestData), culture);
            var (compilation, diagnostics) = Run(sourceCode);
            var interceptInfoList = compilation.GetInterceptInfo();

            diagnostics.ShouldBeEmpty();

            var info = interceptInfoList.Dequeue();
            var method = info.Methods.Dequeue();

            method.Name.ShouldBe(Grow);
            method.Text.Count.ShouldBe(1);
            method.Text[0].ShouldBe("8");
            method.Format.ShouldBeNull();
            method.Provider.ShouldBeNull();

            method = info.Methods.Dequeue();

            method.Name.ShouldBe(DangerousWriteConstantLiteral);
            method.Text.Count.ShouldBe(1);
            method.Text[0].ShouldBe("\"1234.567\"u8");
            method.Format.ShouldBeNull();
            method.Provider.ShouldBeNull();

            info.Methods.ShouldBeEmpty();
            interceptInfoList.ShouldBeEmpty();
        }  
    }

    [Fact]
    public void 定数_メソッド引数で特定カルチャー指定()
    {
        var sourceCode = Get("{{ DoubleConstantField }}", nameof(DoubleContextTestData), JaJpCulture);
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(WriteValue);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("global::SimpleTextTemplate.Generator.Tests.Core.DoubleContextTestData.@DoubleConstantField");
        method.Format.ShouldBe(DefaultKeyword);
        method.Provider.ShouldBe(ProviderArgument);

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }

    [Fact]
    public void 定数_メソッド引数とテンプレート文字列で特定カルチャー指定()
    {
        var sourceCode = Get("{{ DoubleConstantField::es-ES }}", nameof(DoubleContextTestData), JaJpCulture);
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(Grow);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("8");
        method.Format.ShouldBeNull();
        method.Provider.ShouldBeNull();

        method = info.Methods.Dequeue();

        method.Name.ShouldBe(DangerousWriteConstantLiteral);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("\"1234,567\"u8");
        method.Format.ShouldBeNull();
        method.Provider.ShouldBeNull();

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }

    [Fact]
    public void 複数の定数()
    {
        var sourceCode = Get("{{ DoubleConstantField }}{{ DoubleConstantField }}", nameof(DoubleContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(Grow);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("16");
        method.Format.ShouldBeNull();
        method.Provider.ShouldBeNull();

        method = info.Methods.Dequeue();

        method.Name.ShouldBe(DangerousWriteConstantLiteral);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("\"1234.5671234.567\"u8");
        method.Format.ShouldBeNull();
        method.Provider.ShouldBeNull();

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }

    [Fact]
    public void 静的フィールド()
    {
        var sourceCode = Get("{{ DoubleStaticField }}", nameof(DoubleContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(WriteValue);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("global::SimpleTextTemplate.Generator.Tests.Core.DoubleContextTestData.@DoubleStaticField");
        method.Format.ShouldBe(DefaultKeyword);
        method.Provider.ShouldBe(GlobalInvariantCulture);

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }

    [Fact]
    public void 静的フィールド_Format指定()
    {
        var sourceCode = Get("{{ DoubleStaticField:N3 }}", nameof(DoubleContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(WriteValue);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("global::SimpleTextTemplate.Generator.Tests.Core.DoubleContextTestData.@DoubleStaticField");
        method.Format.ShouldBe("\"N3\"");
        method.Provider.ShouldBe(GlobalInvariantCulture);

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }

    [Fact]
    public void 静的フィールド_特定カルチャー指定()
    {
        var sourceCode = Get("{{ DoubleStaticField::es-ES }}", nameof(DoubleContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(WriteValue);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("global::SimpleTextTemplate.Generator.Tests.Core.DoubleContextTestData.@DoubleStaticField");
        method.Format.ShouldBe(DefaultKeyword);
        method.Provider.ShouldBe("esES");

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }

    [Fact]
    public void 静的フィールド_Formatと特定カルチャー指定()
    {
        var sourceCode = Get("{{ DoubleStaticField:N3:es-ES }}", nameof(DoubleContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(WriteValue);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("global::SimpleTextTemplate.Generator.Tests.Core.DoubleContextTestData.@DoubleStaticField");
        method.Format.ShouldBe("\"N3\"");
        method.Provider.ShouldBe("esES");

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }

    [Fact]
    public void 静的フィールド_メソッド引数でInvariantCulture指定()
    {
        foreach (var culture in InvariantCultureList)
        {
            var sourceCode = Get("{{ DoubleStaticField }}", nameof(DoubleContextTestData), culture);
            var (compilation, diagnostics) = Run(sourceCode);
            var interceptInfoList = compilation.GetInterceptInfo();

            diagnostics.ShouldBeEmpty();

            var info = interceptInfoList.Dequeue();
            var method = info.Methods.Dequeue();

            method.Name.ShouldBe(WriteValue);
            method.Text.Count.ShouldBe(1);
            method.Text[0].ShouldBe("global::SimpleTextTemplate.Generator.Tests.Core.DoubleContextTestData.@DoubleStaticField");
            method.Format.ShouldBe(DefaultKeyword);
            method.Provider.ShouldBe(GlobalInvariantCulture);

            info.Methods.ShouldBeEmpty();
            interceptInfoList.ShouldBeEmpty();
        }
    }

    [Fact]
    public void 静的フィールド_メソッド引数で特定カルチャー指定()
    {
        var sourceCode = Get("{{ DoubleStaticField }}", nameof(DoubleContextTestData), JaJpCulture);
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(WriteValue);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("global::SimpleTextTemplate.Generator.Tests.Core.DoubleContextTestData.@DoubleStaticField");
        method.Format.ShouldBe(DefaultKeyword);
        method.Provider.ShouldBe(ProviderArgument);

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }

    [Fact]
    public void 静的フィールド_メソッド引数とテンプレート文字列で特定カルチャー指定()
    {
        var sourceCode = Get("{{ DoubleStaticField::es-ES }}", nameof(DoubleContextTestData), JaJpCulture);
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(WriteValue);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("global::SimpleTextTemplate.Generator.Tests.Core.DoubleContextTestData.@DoubleStaticField");
        method.Format.ShouldBe(DefaultKeyword);
        method.Provider.ShouldBe("esES");

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }

    [Fact]
    public void フィールド()
    {
        var sourceCode = Get("{{ DoubleField }}", nameof(DoubleContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(WriteValue);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@DoubleField");
        method.Format.ShouldBe(DefaultKeyword);
        method.Provider.ShouldBe(GlobalInvariantCulture);

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }

    [Fact]
    public void フィールド_Format指定()
    {
        var sourceCode = Get("{{ DoubleField:N3 }}", nameof(DoubleContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(WriteValue);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@DoubleField");
        method.Format.ShouldBe("\"N3\"");
        method.Provider.ShouldBe(GlobalInvariantCulture);

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }

    [Fact]
    public void フィールド_特定カルチャー指定()
    {
        var sourceCode = Get("{{ DoubleField::es-ES }}", nameof(DoubleContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(WriteValue);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@DoubleField");
        method.Format.ShouldBe(DefaultKeyword);
        method.Provider.ShouldBe("esES");

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }

    [Fact]
    public void フィールド_Formatと特定カルチャー指定()
    {
        var sourceCode = Get("{{ DoubleField:N3:es-ES }}", nameof(DoubleContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(WriteValue);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@DoubleField");
        method.Format.ShouldBe("\"N3\"");
        method.Provider.ShouldBe("esES");

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }


    [Fact]
    public void フィールド_メソッド引数でInvariantCulture指定()
    {
        foreach (var culture in InvariantCultureList)
        {
            var sourceCode = Get("{{ DoubleField }}", nameof(DoubleContextTestData), culture);
            var (compilation, diagnostics) = Run(sourceCode);
            var interceptInfoList = compilation.GetInterceptInfo();

            diagnostics.ShouldBeEmpty();

            var info = interceptInfoList.Dequeue();
            var method = info.Methods.Dequeue();

            method.Name.ShouldBe(WriteValue);
            method.Text.Count.ShouldBe(1);
            method.Text[0].ShouldBe("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@DoubleField");
            method.Format.ShouldBe(DefaultKeyword);
            method.Provider.ShouldBe(GlobalInvariantCulture);

            info.Methods.ShouldBeEmpty();
            interceptInfoList.ShouldBeEmpty();
        }
    }

    [Fact]
    public void フィールド_メソッド引数で特定カルチャー指定()
    {
        var sourceCode = Get("{{ DoubleField }}", nameof(DoubleContextTestData), JaJpCulture);
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(WriteValue);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@DoubleField");
        method.Format.ShouldBe(DefaultKeyword);
        method.Provider.ShouldBe(ProviderArgument);

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }

    [Fact]
    public void フィールド_メソッド引数とテンプレート文字列で特定カルチャー指定()
    {
        var sourceCode = Get("{{ DoubleField::es-ES }}", nameof(DoubleContextTestData), JaJpCulture);
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(WriteValue);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@DoubleField");
        method.Format.ShouldBe(DefaultKeyword);
        method.Provider.ShouldBe("esES");

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }

    [Fact]
    public void 静的プロパティ()
    {
        var sourceCode = Get("{{ DoubleStaticProperty }}", nameof(DoubleContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(WriteValue);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("global::SimpleTextTemplate.Generator.Tests.Core.DoubleContextTestData.@DoubleStaticProperty");
        method.Format.ShouldBe(DefaultKeyword);
        method.Provider.ShouldBe(GlobalInvariantCulture);

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }

    [Fact]
    public void 静的プロパティ_Format指定()
    {
        var sourceCode = Get("{{ DoubleStaticProperty:N3 }}", nameof(DoubleContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(WriteValue);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("global::SimpleTextTemplate.Generator.Tests.Core.DoubleContextTestData.@DoubleStaticProperty");
        method.Format.ShouldBe("\"N3\"");
        method.Provider.ShouldBe(GlobalInvariantCulture);

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }

    [Fact]
    public void 静的プロパティ_特定カルチャー指定()
    {
        var sourceCode = Get("{{ DoubleStaticProperty::es-ES }}", nameof(DoubleContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(WriteValue);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("global::SimpleTextTemplate.Generator.Tests.Core.DoubleContextTestData.@DoubleStaticProperty");
        method.Format.ShouldBe(DefaultKeyword);
        method.Provider.ShouldBe("esES");

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }

    [Fact]
    public void 静的プロパティ_Formatと特定カルチャー指定()
    {
        var sourceCode = Get("{{ DoubleStaticProperty:N3:es-ES }}", nameof(DoubleContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(WriteValue);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("global::SimpleTextTemplate.Generator.Tests.Core.DoubleContextTestData.@DoubleStaticProperty");
        method.Format.ShouldBe("\"N3\"");
        method.Provider.ShouldBe("esES");

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }

    [Fact]
    public void 静的プロパティ_メソッド引数でInvariantCulture指定()
    {
        foreach (var culture in InvariantCultureList)
        {
            var sourceCode = Get("{{ DoubleStaticProperty }}", nameof(DoubleContextTestData), InvariantCulture);
            var (compilation, diagnostics) = Run(sourceCode);
            var interceptInfoList = compilation.GetInterceptInfo();

            diagnostics.ShouldBeEmpty();

            var info = interceptInfoList.Dequeue();
            var method = info.Methods.Dequeue();

            method.Name.ShouldBe(WriteValue);
            method.Text.Count.ShouldBe(1);
            method.Text[0].ShouldBe("global::SimpleTextTemplate.Generator.Tests.Core.DoubleContextTestData.@DoubleStaticProperty");
            method.Format.ShouldBe(DefaultKeyword);
            method.Provider.ShouldBe(GlobalInvariantCulture);

            info.Methods.ShouldBeEmpty();
            interceptInfoList.ShouldBeEmpty();
        }
    }

    [Fact]
    public void 静的プロパティ_メソッド引数で特定カルチャー指定()
    {
        var sourceCode = Get("{{ DoubleStaticProperty }}", nameof(DoubleContextTestData), JaJpCulture);
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(WriteValue);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("global::SimpleTextTemplate.Generator.Tests.Core.DoubleContextTestData.@DoubleStaticProperty");
        method.Format.ShouldBe(DefaultKeyword);
        method.Provider.ShouldBe(ProviderArgument);

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }

    [Fact]
    public void 静的プロパティ_メソッド引数とテンプレート文字列で特定カルチャー指定()
    {
        var sourceCode = Get("{{ DoubleStaticProperty::es-ES }}", nameof(DoubleContextTestData), JaJpCulture);
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(WriteValue);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("global::SimpleTextTemplate.Generator.Tests.Core.DoubleContextTestData.@DoubleStaticProperty");
        method.Format.ShouldBe(DefaultKeyword);
        method.Provider.ShouldBe("esES");

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }

    [Fact]
    public void プロパティ()
    {
        var sourceCode = Get("{{ DoubleProperty }}", nameof(DoubleContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(WriteValue);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@DoubleProperty");
        method.Format.ShouldBe(DefaultKeyword);
        method.Provider.ShouldBe(GlobalInvariantCulture);

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }

    [Fact]
    public void プロパティ_Format指定()
    {
        var sourceCode = Get("{{ DoubleProperty:N3 }}", nameof(DoubleContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(WriteValue);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@DoubleProperty");
        method.Format.ShouldBe("\"N3\"");
        method.Provider.ShouldBe(GlobalInvariantCulture);

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }

    [Fact]
    public void プロパティ_特定カルチャー指定()
    {
        var sourceCode = Get("{{ DoubleProperty::es-ES }}", nameof(DoubleContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(WriteValue);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@DoubleProperty");
        method.Format.ShouldBe(DefaultKeyword);
        method.Provider.ShouldBe("esES");

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }

    [Fact]
    public void プロパティ_Formatと特定カルチャー指定()
    {
        var sourceCode = Get("{{ DoubleProperty:N3:es-ES }}", nameof(DoubleContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(WriteValue);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@DoubleProperty");
        method.Format.ShouldBe("\"N3\"");
        method.Provider.ShouldBe("esES");

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }

    [Fact]
    public void プロパティ_メソッド引数でInvariantCulture指定()
    {
        foreach (var culture in InvariantCultureList)
        {
            var sourceCode = Get("{{ DoubleProperty }}", nameof(DoubleContextTestData), InvariantCulture);
            var (compilation, diagnostics) = Run(sourceCode);
            var interceptInfoList = compilation.GetInterceptInfo();

            diagnostics.ShouldBeEmpty();

            var info = interceptInfoList.Dequeue();
            var method = info.Methods.Dequeue();

            method.Name.ShouldBe(WriteValue);
            method.Text.Count.ShouldBe(1);
            method.Text[0].ShouldBe("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@DoubleProperty");
            method.Format.ShouldBe(DefaultKeyword);
            method.Provider.ShouldBe(GlobalInvariantCulture);

            info.Methods.ShouldBeEmpty();
            interceptInfoList.ShouldBeEmpty();
        }
    }

    [Fact]
    public void プロパティ_メソッド引数で特定カルチャー指定()
    {
        var sourceCode = Get("{{ DoubleProperty }}", nameof(DoubleContextTestData), JaJpCulture);
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(WriteValue);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@DoubleProperty");
        method.Format.ShouldBe(DefaultKeyword);
        method.Provider.ShouldBe(ProviderArgument);

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }

    [Fact]
    public void プロパティ_メソッド引数とテンプレート文字列で特定カルチャー指定()
    {
        var sourceCode = Get("{{ DoubleProperty::es-ES }}", nameof(DoubleContextTestData), JaJpCulture);
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(WriteValue);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@DoubleProperty");
        method.Format.ShouldBe(DefaultKeyword);
        method.Provider.ShouldBe("esES");

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }
}
