using SimpleTextTemplate.Generator.Tests.Extensions;
using SimpleTextTemplate.Tests.TestData;
using static SimpleTextTemplate.Generator.Tests.Constants;
using static SimpleTextTemplate.Generator.Tests.GeneratorRunner;
using static SimpleTextTemplate.Generator.Tests.SourceCode;

namespace SimpleTextTemplate.Generator.Tests;

public sealed class TemplateRendererRenderDoubleTest
{
    [Test]
    public async Task 定数()
    {
        var sourceCode = Get("{{ DoubleConstantField }}", nameof(DoubleContextTestData));
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(Grow);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("8");
        await Assert.That(method.Format).IsNull();
        await Assert.That(method.Provider).IsNull();

        method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(DangerousWriteConstantLiteral);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("\"1234.567\"u8");
        await Assert.That(method.Format).IsNull();
        await Assert.That(method.Provider).IsNull();

        await Assert.That(info.Methods).IsEmpty();
        await Assert.That(interceptInfoList).IsEmpty();
    }

    [Test]
    public async Task 定数_Format指定()
    {
        var sourceCode = Get("{{ DoubleConstantField:N3 }}", nameof(DoubleContextTestData));
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(Grow);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("9");
        await Assert.That(method.Format).IsNull();
        await Assert.That(method.Provider).IsNull();

        method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(DangerousWriteConstantLiteral);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("\"1,234.567\"u8");
        await Assert.That(method.Format).IsNull();
        await Assert.That(method.Provider).IsNull();

        await Assert.That(info.Methods).IsEmpty();
        await Assert.That(interceptInfoList).IsEmpty();
    }

    [Test]
    public async Task 定数_特定カルチャー指定()
    {
        var sourceCode = Get("{{ DoubleConstantField::es-ES }}", nameof(DoubleContextTestData));
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(Grow);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("8");
        await Assert.That(method.Format).IsNull();
        await Assert.That(method.Provider).IsNull();

        method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(DangerousWriteConstantLiteral);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("\"1234,567\"u8");
        await Assert.That(method.Format).IsNull();
        await Assert.That(method.Provider).IsNull();

        await Assert.That(info.Methods).IsEmpty();
        await Assert.That(interceptInfoList).IsEmpty();
    }

    [Test]
    public async Task 定数_Formatと特定カルチャー指定()
    {
        var sourceCode = Get("{{ DoubleConstantField:N3:es-ES }}", nameof(DoubleContextTestData));
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(Grow);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("9");
        await Assert.That(method.Format).IsNull();
        await Assert.That(method.Provider).IsNull();

        method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(DangerousWriteConstantLiteral);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("\"1.234,567\"u8");
        await Assert.That(method.Format).IsNull();
        await Assert.That(method.Provider).IsNull();

        await Assert.That(info.Methods).IsEmpty();
        await Assert.That(interceptInfoList).IsEmpty();
    }

    [Test]
    public async Task 定数_メソッド引数でInvariantCulture指定()
    {
        foreach (var culture in InvariantCultureList)
        {
            var sourceCode = Get("{{ DoubleConstantField }}", nameof(DoubleContextTestData), culture);
            var (compilation, diagnostics) = await RunAsync(sourceCode);
            var interceptInfoList = compilation.GetInterceptInfo();

            await Assert.That(diagnostics).IsEmpty();

            var info = interceptInfoList.Dequeue();
            var method = info.Methods.Dequeue();

            await Assert.That(method.Name).IsEqualTo(Grow);
            await Assert.That(method.Text.Count).IsEqualTo(1);
            await Assert.That(method.Text[0]).IsEqualTo("8");
            await Assert.That(method.Format).IsNull();
            await Assert.That(method.Provider).IsNull();

            method = info.Methods.Dequeue();

            await Assert.That(method.Name).IsEqualTo(DangerousWriteConstantLiteral);
            await Assert.That(method.Text.Count).IsEqualTo(1);
            await Assert.That(method.Text[0]).IsEqualTo("\"1234.567\"u8");
            await Assert.That(method.Format).IsNull();
            await Assert.That(method.Provider).IsNull();

            await Assert.That(info.Methods).IsEmpty();
            await Assert.That(interceptInfoList).IsEmpty();
        }
    }

    [Test]
    public async Task 定数_メソッド引数で特定カルチャー指定()
    {
        var sourceCode = Get("{{ DoubleConstantField }}", nameof(DoubleContextTestData), JaJpCulture);
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(WriteValue);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("global::SimpleTextTemplate.Generator.Tests.Core.DoubleContextTestData.@DoubleConstantField");
        await Assert.That(method.Format).IsEqualTo(DefaultKeyword);
        await Assert.That(method.Provider).IsEqualTo(ProviderArgument);

        await Assert.That(info.Methods).IsEmpty();
        await Assert.That(interceptInfoList).IsEmpty();
    }

    [Test]
    public async Task 定数_メソッド引数とテンプレート文字列で特定カルチャー指定()
    {
        var sourceCode = Get("{{ DoubleConstantField::es-ES }}", nameof(DoubleContextTestData), JaJpCulture);
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(Grow);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("8");
        await Assert.That(method.Format).IsNull();
        await Assert.That(method.Provider).IsNull();

        method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(DangerousWriteConstantLiteral);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("\"1234,567\"u8");
        await Assert.That(method.Format).IsNull();
        await Assert.That(method.Provider).IsNull();

        await Assert.That(info.Methods).IsEmpty();
        await Assert.That(interceptInfoList).IsEmpty();
    }

    [Test]
    public async Task 複数の定数()
    {
        var sourceCode = Get("{{ DoubleConstantField }}{{ DoubleConstantField }}", nameof(DoubleContextTestData));
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(Grow);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("16");
        await Assert.That(method.Format).IsNull();
        await Assert.That(method.Provider).IsNull();

        method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(DangerousWriteConstantLiteral);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("\"1234.5671234.567\"u8");
        await Assert.That(method.Format).IsNull();
        await Assert.That(method.Provider).IsNull();

        await Assert.That(info.Methods).IsEmpty();
        await Assert.That(interceptInfoList).IsEmpty();
    }

    [Test]
    public async Task 静的フィールド()
    {
        var sourceCode = Get("{{ DoubleStaticField }}", nameof(DoubleContextTestData));
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(WriteValue);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("global::SimpleTextTemplate.Generator.Tests.Core.DoubleContextTestData.@DoubleStaticField");
        await Assert.That(method.Format).IsEqualTo(DefaultKeyword);
        await Assert.That(method.Provider).IsEqualTo(GlobalInvariantCulture);

        await Assert.That(info.Methods).IsEmpty();
        await Assert.That(interceptInfoList).IsEmpty();
    }

    [Test]
    public async Task 静的フィールド_Format指定()
    {
        var sourceCode = Get("{{ DoubleStaticField:N3 }}", nameof(DoubleContextTestData));
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(WriteValue);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("global::SimpleTextTemplate.Generator.Tests.Core.DoubleContextTestData.@DoubleStaticField");
        await Assert.That(method.Format).IsEqualTo("\"N3\"");
        await Assert.That(method.Provider).IsEqualTo(GlobalInvariantCulture);

        await Assert.That(info.Methods).IsEmpty();
        await Assert.That(interceptInfoList).IsEmpty();
    }

    [Test]
    public async Task 静的フィールド_特定カルチャー指定()
    {
        var sourceCode = Get("{{ DoubleStaticField::es-ES }}", nameof(DoubleContextTestData));
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(WriteValue);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("global::SimpleTextTemplate.Generator.Tests.Core.DoubleContextTestData.@DoubleStaticField");
        await Assert.That(method.Format).IsEqualTo(DefaultKeyword);
        await Assert.That(method.Provider).IsEqualTo("esES");

        await Assert.That(info.Methods).IsEmpty();
        await Assert.That(interceptInfoList).IsEmpty();
    }

    [Test]
    public async Task 静的フィールド_Formatと特定カルチャー指定()
    {
        var sourceCode = Get("{{ DoubleStaticField:N3:es-ES }}", nameof(DoubleContextTestData));
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(WriteValue);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("global::SimpleTextTemplate.Generator.Tests.Core.DoubleContextTestData.@DoubleStaticField");
        await Assert.That(method.Format).IsEqualTo("\"N3\"");
        await Assert.That(method.Provider).IsEqualTo("esES");

        await Assert.That(info.Methods).IsEmpty();
        await Assert.That(interceptInfoList).IsEmpty();
    }

    [Test]
    public async Task 静的フィールド_メソッド引数でInvariantCulture指定()
    {
        foreach (var culture in InvariantCultureList)
        {
            var sourceCode = Get("{{ DoubleStaticField }}", nameof(DoubleContextTestData), culture);
            var (compilation, diagnostics) = await RunAsync(sourceCode);
            var interceptInfoList = compilation.GetInterceptInfo();

            await Assert.That(diagnostics).IsEmpty();

            var info = interceptInfoList.Dequeue();
            var method = info.Methods.Dequeue();

            await Assert.That(method.Name).IsEqualTo(WriteValue);
            await Assert.That(method.Text.Count).IsEqualTo(1);
            await Assert.That(method.Text[0]).IsEqualTo("global::SimpleTextTemplate.Generator.Tests.Core.DoubleContextTestData.@DoubleStaticField");
            await Assert.That(method.Format).IsEqualTo(DefaultKeyword);
            await Assert.That(method.Provider).IsEqualTo(GlobalInvariantCulture);

            await Assert.That(info.Methods).IsEmpty();
            await Assert.That(interceptInfoList).IsEmpty();
        }
    }

    [Test]
    public async Task 静的フィールド_メソッド引数で特定カルチャー指定()
    {
        var sourceCode = Get("{{ DoubleStaticField }}", nameof(DoubleContextTestData), JaJpCulture);
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(WriteValue);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("global::SimpleTextTemplate.Generator.Tests.Core.DoubleContextTestData.@DoubleStaticField");
        await Assert.That(method.Format).IsEqualTo(DefaultKeyword);
        await Assert.That(method.Provider).IsEqualTo(ProviderArgument);

        await Assert.That(info.Methods).IsEmpty();
        await Assert.That(interceptInfoList).IsEmpty();
    }

    [Test]
    public async Task 静的フィールド_メソッド引数とテンプレート文字列で特定カルチャー指定()
    {
        var sourceCode = Get("{{ DoubleStaticField::es-ES }}", nameof(DoubleContextTestData), JaJpCulture);
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(WriteValue);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("global::SimpleTextTemplate.Generator.Tests.Core.DoubleContextTestData.@DoubleStaticField");
        await Assert.That(method.Format).IsEqualTo(DefaultKeyword);
        await Assert.That(method.Provider).IsEqualTo("esES");

        await Assert.That(info.Methods).IsEmpty();
        await Assert.That(interceptInfoList).IsEmpty();
    }

    [Test]
    public async Task フィールド()
    {
        var sourceCode = Get("{{ DoubleField }}", nameof(DoubleContextTestData));
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(WriteValue);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@DoubleField");
        await Assert.That(method.Format).IsEqualTo(DefaultKeyword);
        await Assert.That(method.Provider).IsEqualTo(GlobalInvariantCulture);

        await Assert.That(info.Methods).IsEmpty();
        await Assert.That(interceptInfoList).IsEmpty();
    }

    [Test]
    public async Task フィールド_Format指定()
    {
        var sourceCode = Get("{{ DoubleField:N3 }}", nameof(DoubleContextTestData));
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(WriteValue);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@DoubleField");
        await Assert.That(method.Format).IsEqualTo("\"N3\"");
        await Assert.That(method.Provider).IsEqualTo(GlobalInvariantCulture);

        await Assert.That(info.Methods).IsEmpty();
        await Assert.That(interceptInfoList).IsEmpty();
    }

    [Test]
    public async Task フィールド_特定カルチャー指定()
    {
        var sourceCode = Get("{{ DoubleField::es-ES }}", nameof(DoubleContextTestData));
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(WriteValue);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@DoubleField");
        await Assert.That(method.Format).IsEqualTo(DefaultKeyword);
        await Assert.That(method.Provider).IsEqualTo("esES");

        await Assert.That(info.Methods).IsEmpty();
        await Assert.That(interceptInfoList).IsEmpty();
    }

    [Test]
    public async Task フィールド_Formatと特定カルチャー指定()
    {
        var sourceCode = Get("{{ DoubleField:N3:es-ES }}", nameof(DoubleContextTestData));
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(WriteValue);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@DoubleField");
        await Assert.That(method.Format).IsEqualTo("\"N3\"");
        await Assert.That(method.Provider).IsEqualTo("esES");

        await Assert.That(info.Methods).IsEmpty();
        await Assert.That(interceptInfoList).IsEmpty();
    }

    [Test]
    public async Task フィールド_メソッド引数でInvariantCulture指定()
    {
        foreach (var culture in InvariantCultureList)
        {
            var sourceCode = Get("{{ DoubleField }}", nameof(DoubleContextTestData), culture);
            var (compilation, diagnostics) = await RunAsync(sourceCode);
            var interceptInfoList = compilation.GetInterceptInfo();

            await Assert.That(diagnostics).IsEmpty();

            var info = interceptInfoList.Dequeue();
            var method = info.Methods.Dequeue();

            await Assert.That(method.Name).IsEqualTo(WriteValue);
            await Assert.That(method.Text.Count).IsEqualTo(1);
            await Assert.That(method.Text[0]).IsEqualTo("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@DoubleField");
            await Assert.That(method.Format).IsEqualTo(DefaultKeyword);
            await Assert.That(method.Provider).IsEqualTo(GlobalInvariantCulture);

            await Assert.That(info.Methods).IsEmpty();
            await Assert.That(interceptInfoList).IsEmpty();
        }
    }

    [Test]
    public async Task フィールド_メソッド引数で特定カルチャー指定()
    {
        var sourceCode = Get("{{ DoubleField }}", nameof(DoubleContextTestData), JaJpCulture);
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(WriteValue);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@DoubleField");
        await Assert.That(method.Format).IsEqualTo(DefaultKeyword);
        await Assert.That(method.Provider).IsEqualTo(ProviderArgument);

        await Assert.That(info.Methods).IsEmpty();
        await Assert.That(interceptInfoList).IsEmpty();
    }

    [Test]
    public async Task フィールド_メソッド引数とテンプレート文字列で特定カルチャー指定()
    {
        var sourceCode = Get("{{ DoubleField::es-ES }}", nameof(DoubleContextTestData), JaJpCulture);
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(WriteValue);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@DoubleField");
        await Assert.That(method.Format).IsEqualTo(DefaultKeyword);
        await Assert.That(method.Provider).IsEqualTo("esES");

        await Assert.That(info.Methods).IsEmpty();
        await Assert.That(interceptInfoList).IsEmpty();
    }

    [Test]
    public async Task 静的プロパティ()
    {
        var sourceCode = Get("{{ DoubleStaticProperty }}", nameof(DoubleContextTestData));
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(WriteValue);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("global::SimpleTextTemplate.Generator.Tests.Core.DoubleContextTestData.@DoubleStaticProperty");
        await Assert.That(method.Format).IsEqualTo(DefaultKeyword);
        await Assert.That(method.Provider).IsEqualTo(GlobalInvariantCulture);

        await Assert.That(info.Methods).IsEmpty();
        await Assert.That(interceptInfoList).IsEmpty();
    }

    [Test]
    public async Task 静的プロパティ_Format指定()
    {
        var sourceCode = Get("{{ DoubleStaticProperty:N3 }}", nameof(DoubleContextTestData));
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(WriteValue);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("global::SimpleTextTemplate.Generator.Tests.Core.DoubleContextTestData.@DoubleStaticProperty");
        await Assert.That(method.Format).IsEqualTo("\"N3\"");
        await Assert.That(method.Provider).IsEqualTo(GlobalInvariantCulture);

        await Assert.That(info.Methods).IsEmpty();
        await Assert.That(interceptInfoList).IsEmpty();
    }

    [Test]
    public async Task 静的プロパティ_特定カルチャー指定()
    {
        var sourceCode = Get("{{ DoubleStaticProperty::es-ES }}", nameof(DoubleContextTestData));
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(WriteValue);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("global::SimpleTextTemplate.Generator.Tests.Core.DoubleContextTestData.@DoubleStaticProperty");
        await Assert.That(method.Format).IsEqualTo(DefaultKeyword);
        await Assert.That(method.Provider).IsEqualTo("esES");

        await Assert.That(info.Methods).IsEmpty();
        await Assert.That(interceptInfoList).IsEmpty();
    }

    [Test]
    public async Task 静的プロパティ_Formatと特定カルチャー指定()
    {
        var sourceCode = Get("{{ DoubleStaticProperty:N3:es-ES }}", nameof(DoubleContextTestData));
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(WriteValue);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("global::SimpleTextTemplate.Generator.Tests.Core.DoubleContextTestData.@DoubleStaticProperty");
        await Assert.That(method.Format).IsEqualTo("\"N3\"");
        await Assert.That(method.Provider).IsEqualTo("esES");

        await Assert.That(info.Methods).IsEmpty();
        await Assert.That(interceptInfoList).IsEmpty();
    }

    [Test]
    public async Task 静的プロパティ_メソッド引数でInvariantCulture指定()
    {
        foreach (var culture in InvariantCultureList)
        {
            var sourceCode = Get("{{ DoubleStaticProperty }}", nameof(DoubleContextTestData), InvariantCulture);
            var (compilation, diagnostics) = await RunAsync(sourceCode);
            var interceptInfoList = compilation.GetInterceptInfo();

            await Assert.That(diagnostics).IsEmpty();

            var info = interceptInfoList.Dequeue();
            var method = info.Methods.Dequeue();

            await Assert.That(method.Name).IsEqualTo(WriteValue);
            await Assert.That(method.Text.Count).IsEqualTo(1);
            await Assert.That(method.Text[0]).IsEqualTo("global::SimpleTextTemplate.Generator.Tests.Core.DoubleContextTestData.@DoubleStaticProperty");
            await Assert.That(method.Format).IsEqualTo(DefaultKeyword);
            await Assert.That(method.Provider).IsEqualTo(GlobalInvariantCulture);

            await Assert.That(info.Methods).IsEmpty();
            await Assert.That(interceptInfoList).IsEmpty();
        }
    }

    [Test]
    public async Task 静的プロパティ_メソッド引数で特定カルチャー指定()
    {
        var sourceCode = Get("{{ DoubleStaticProperty }}", nameof(DoubleContextTestData), JaJpCulture);
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(WriteValue);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("global::SimpleTextTemplate.Generator.Tests.Core.DoubleContextTestData.@DoubleStaticProperty");
        await Assert.That(method.Format).IsEqualTo(DefaultKeyword);
        await Assert.That(method.Provider).IsEqualTo(ProviderArgument);

        await Assert.That(info.Methods).IsEmpty();
        await Assert.That(interceptInfoList).IsEmpty();
    }

    [Test]
    public async Task 静的プロパティ_メソッド引数とテンプレート文字列で特定カルチャー指定()
    {
        var sourceCode = Get("{{ DoubleStaticProperty::es-ES }}", nameof(DoubleContextTestData), JaJpCulture);
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(WriteValue);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("global::SimpleTextTemplate.Generator.Tests.Core.DoubleContextTestData.@DoubleStaticProperty");
        await Assert.That(method.Format).IsEqualTo(DefaultKeyword);
        await Assert.That(method.Provider).IsEqualTo("esES");

        await Assert.That(info.Methods).IsEmpty();
        await Assert.That(interceptInfoList).IsEmpty();
    }

    [Test]
    public async Task プロパティ()
    {
        var sourceCode = Get("{{ DoubleProperty }}", nameof(DoubleContextTestData));
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(WriteValue);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@DoubleProperty");
        await Assert.That(method.Format).IsEqualTo(DefaultKeyword);
        await Assert.That(method.Provider).IsEqualTo(GlobalInvariantCulture);

        await Assert.That(info.Methods).IsEmpty();
        await Assert.That(interceptInfoList).IsEmpty();
    }

    [Test]
    public async Task プロパティ_Format指定()
    {
        var sourceCode = Get("{{ DoubleProperty:N3 }}", nameof(DoubleContextTestData));
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(WriteValue);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@DoubleProperty");
        await Assert.That(method.Format).IsEqualTo("\"N3\"");
        await Assert.That(method.Provider).IsEqualTo(GlobalInvariantCulture);

        await Assert.That(info.Methods).IsEmpty();
        await Assert.That(interceptInfoList).IsEmpty();
    }

    [Test]
    public async Task プロパティ_特定カルチャー指定()
    {
        var sourceCode = Get("{{ DoubleProperty::es-ES }}", nameof(DoubleContextTestData));
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(WriteValue);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@DoubleProperty");
        await Assert.That(method.Format).IsEqualTo(DefaultKeyword);
        await Assert.That(method.Provider).IsEqualTo("esES");

        await Assert.That(info.Methods).IsEmpty();
        await Assert.That(interceptInfoList).IsEmpty();
    }

    [Test]
    public async Task プロパティ_Formatと特定カルチャー指定()
    {
        var sourceCode = Get("{{ DoubleProperty:N3:es-ES }}", nameof(DoubleContextTestData));
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(WriteValue);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@DoubleProperty");
        await Assert.That(method.Format).IsEqualTo("\"N3\"");
        await Assert.That(method.Provider).IsEqualTo("esES");

        await Assert.That(info.Methods).IsEmpty();
        await Assert.That(interceptInfoList).IsEmpty();
    }

    [Test]
    public async Task プロパティ_メソッド引数でInvariantCulture指定()
    {
        foreach (var culture in InvariantCultureList)
        {
            var sourceCode = Get("{{ DoubleProperty }}", nameof(DoubleContextTestData), InvariantCulture);
            var (compilation, diagnostics) = await RunAsync(sourceCode);
            var interceptInfoList = compilation.GetInterceptInfo();

            await Assert.That(diagnostics).IsEmpty();

            var info = interceptInfoList.Dequeue();
            var method = info.Methods.Dequeue();

            await Assert.That(method.Name).IsEqualTo(WriteValue);
            await Assert.That(method.Text.Count).IsEqualTo(1);
            await Assert.That(method.Text[0]).IsEqualTo("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@DoubleProperty");
            await Assert.That(method.Format).IsEqualTo(DefaultKeyword);
            await Assert.That(method.Provider).IsEqualTo(GlobalInvariantCulture);

            await Assert.That(info.Methods).IsEmpty();
            await Assert.That(interceptInfoList).IsEmpty();
        }
    }

    [Test]
    public async Task プロパティ_メソッド引数で特定カルチャー指定()
    {
        var sourceCode = Get("{{ DoubleProperty }}", nameof(DoubleContextTestData), JaJpCulture);
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(WriteValue);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@DoubleProperty");
        await Assert.That(method.Format).IsEqualTo(DefaultKeyword);
        await Assert.That(method.Provider).IsEqualTo(ProviderArgument);

        await Assert.That(info.Methods).IsEmpty();
        await Assert.That(interceptInfoList).IsEmpty();
    }

    [Test]
    public async Task プロパティ_メソッド引数とテンプレート文字列で特定カルチャー指定()
    {
        var sourceCode = Get("{{ DoubleProperty::es-ES }}", nameof(DoubleContextTestData), JaJpCulture);
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(WriteValue);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@DoubleProperty");
        await Assert.That(method.Format).IsEqualTo(DefaultKeyword);
        await Assert.That(method.Provider).IsEqualTo("esES");

        await Assert.That(info.Methods).IsEmpty();
        await Assert.That(interceptInfoList).IsEmpty();
    }
}
