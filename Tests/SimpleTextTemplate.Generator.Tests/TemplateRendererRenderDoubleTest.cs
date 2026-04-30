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
        var sourceCode = Get<DoubleContextTestData>("{{ DoubleConstantField }}");
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
        var sourceCode = Get<DoubleContextTestData>("{{ DoubleConstantField:N3 }}");
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
        var sourceCode = Get<DoubleContextTestData>("{{ DoubleConstantField::es-ES }}");
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
        var sourceCode = Get<DoubleContextTestData>("{{ DoubleConstantField:N3:es-ES }}");
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
            var sourceCode = GetWithCulture<DoubleContextTestData>(culture, "{{ DoubleConstantField }}");
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
        var sourceCode = GetWithCulture<DoubleContextTestData>(JaJpCulture, "{{ DoubleConstantField }}");
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(WriteValue);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("global::SimpleTextTemplate.Tests.TestData.DoubleContextTestData.@DoubleConstantField");
        await Assert.That(method.Format).IsEqualTo(DefaultKeyword);
        await Assert.That(method.Provider).IsEqualTo(ProviderArgument);

        await Assert.That(info.Methods).IsEmpty();
        await Assert.That(interceptInfoList).IsEmpty();
    }

    [Test]
    public async Task 定数_メソッド引数とテンプレート文字列で特定カルチャー指定()
    {
        var sourceCode = GetWithCulture<DoubleContextTestData>(JaJpCulture, "{{ DoubleConstantField::es-ES }}");
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
        var sourceCode = Get<DoubleContextTestData>("{{ DoubleConstantField }}{{ DoubleConstantField }}");
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
        var sourceCode = Get<DoubleContextTestData>("{{ DoubleStaticField }}");
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(WriteValue);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("global::SimpleTextTemplate.Tests.TestData.DoubleContextTestData.@DoubleStaticField");
        await Assert.That(method.Format).IsEqualTo(DefaultKeyword);
        await Assert.That(method.Provider).IsEqualTo(GlobalInvariantCulture);

        await Assert.That(info.Methods).IsEmpty();
        await Assert.That(interceptInfoList).IsEmpty();
    }

    [Test]
    public async Task 静的フィールド_Format指定()
    {
        var sourceCode = Get<DoubleContextTestData>("{{ DoubleStaticField:N3 }}");
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(WriteValue);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("global::SimpleTextTemplate.Tests.TestData.DoubleContextTestData.@DoubleStaticField");
        await Assert.That(method.Format).IsEqualTo("\"N3\"");
        await Assert.That(method.Provider).IsEqualTo(GlobalInvariantCulture);

        await Assert.That(info.Methods).IsEmpty();
        await Assert.That(interceptInfoList).IsEmpty();
    }

    [Test]
    public async Task 静的フィールド_特定カルチャー指定()
    {
        var sourceCode = Get<DoubleContextTestData>("{{ DoubleStaticField::es-ES }}");
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(WriteValue);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("global::SimpleTextTemplate.Tests.TestData.DoubleContextTestData.@DoubleStaticField");
        await Assert.That(method.Format).IsEqualTo(DefaultKeyword);
        await Assert.That(method.Provider).IsEqualTo("esES");

        await Assert.That(info.Methods).IsEmpty();
        await Assert.That(interceptInfoList).IsEmpty();
    }

    [Test]
    public async Task 静的フィールド_Formatと特定カルチャー指定()
    {
        var sourceCode = Get<DoubleContextTestData>("{{ DoubleStaticField:N3:es-ES }}");
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(WriteValue);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("global::SimpleTextTemplate.Tests.TestData.DoubleContextTestData.@DoubleStaticField");
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
            var sourceCode = GetWithCulture<DoubleContextTestData>(culture, "{{ DoubleStaticField }}");
            var (compilation, diagnostics) = await RunAsync(sourceCode);
            var interceptInfoList = compilation.GetInterceptInfo();

            await Assert.That(diagnostics).IsEmpty();

            var info = interceptInfoList.Dequeue();
            var method = info.Methods.Dequeue();

            await Assert.That(method.Name).IsEqualTo(WriteValue);
            await Assert.That(method.Text.Count).IsEqualTo(1);
            await Assert.That(method.Text[0]).IsEqualTo("global::SimpleTextTemplate.Tests.TestData.DoubleContextTestData.@DoubleStaticField");
            await Assert.That(method.Format).IsEqualTo(DefaultKeyword);
            await Assert.That(method.Provider).IsEqualTo(GlobalInvariantCulture);

            await Assert.That(info.Methods).IsEmpty();
            await Assert.That(interceptInfoList).IsEmpty();
        }
    }

    [Test]
    public async Task 静的フィールド_メソッド引数で特定カルチャー指定()
    {
        var sourceCode = GetWithCulture<DoubleContextTestData>(JaJpCulture, "{{ DoubleStaticField }}");
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(WriteValue);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("global::SimpleTextTemplate.Tests.TestData.DoubleContextTestData.@DoubleStaticField");
        await Assert.That(method.Format).IsEqualTo(DefaultKeyword);
        await Assert.That(method.Provider).IsEqualTo(ProviderArgument);

        await Assert.That(info.Methods).IsEmpty();
        await Assert.That(interceptInfoList).IsEmpty();
    }

    [Test]
    public async Task 静的フィールド_メソッド引数とテンプレート文字列で特定カルチャー指定()
    {
        var sourceCode = GetWithCulture<DoubleContextTestData>(JaJpCulture, "{{ DoubleStaticField::es-ES }}");
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(WriteValue);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("global::SimpleTextTemplate.Tests.TestData.DoubleContextTestData.@DoubleStaticField");
        await Assert.That(method.Format).IsEqualTo(DefaultKeyword);
        await Assert.That(method.Provider).IsEqualTo("esES");

        await Assert.That(info.Methods).IsEmpty();
        await Assert.That(interceptInfoList).IsEmpty();
    }

    [Test]
    public async Task フィールド()
    {
        var sourceCode = Get<DoubleContextTestData>("{{ DoubleField }}");
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
        var sourceCode = Get<DoubleContextTestData>("{{ DoubleField:N3 }}");
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
        var sourceCode = Get<DoubleContextTestData>("{{ DoubleField::es-ES }}");
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
        var sourceCode = Get<DoubleContextTestData>("{{ DoubleField:N3:es-ES }}");
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
            var sourceCode = GetWithCulture<DoubleContextTestData>(culture, "{{ DoubleField }}");
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
        var sourceCode = GetWithCulture<DoubleContextTestData>(JaJpCulture, "{{ DoubleField }}");
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
        var sourceCode = GetWithCulture<DoubleContextTestData>(JaJpCulture, "{{ DoubleField::es-ES }}");
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
        var sourceCode = Get<DoubleContextTestData>("{{ DoubleStaticProperty }}");
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(WriteValue);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("global::SimpleTextTemplate.Tests.TestData.DoubleContextTestData.@DoubleStaticProperty");
        await Assert.That(method.Format).IsEqualTo(DefaultKeyword);
        await Assert.That(method.Provider).IsEqualTo(GlobalInvariantCulture);

        await Assert.That(info.Methods).IsEmpty();
        await Assert.That(interceptInfoList).IsEmpty();
    }

    [Test]
    public async Task 静的プロパティ_Format指定()
    {
        var sourceCode = Get<DoubleContextTestData>("{{ DoubleStaticProperty:N3 }}");
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(WriteValue);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("global::SimpleTextTemplate.Tests.TestData.DoubleContextTestData.@DoubleStaticProperty");
        await Assert.That(method.Format).IsEqualTo("\"N3\"");
        await Assert.That(method.Provider).IsEqualTo(GlobalInvariantCulture);

        await Assert.That(info.Methods).IsEmpty();
        await Assert.That(interceptInfoList).IsEmpty();
    }

    [Test]
    public async Task 静的プロパティ_特定カルチャー指定()
    {
        var sourceCode = Get<DoubleContextTestData>("{{ DoubleStaticProperty::es-ES }}");
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(WriteValue);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("global::SimpleTextTemplate.Tests.TestData.DoubleContextTestData.@DoubleStaticProperty");
        await Assert.That(method.Format).IsEqualTo(DefaultKeyword);
        await Assert.That(method.Provider).IsEqualTo("esES");

        await Assert.That(info.Methods).IsEmpty();
        await Assert.That(interceptInfoList).IsEmpty();
    }

    [Test]
    public async Task 静的プロパティ_Formatと特定カルチャー指定()
    {
        var sourceCode = Get<DoubleContextTestData>("{{ DoubleStaticProperty:N3:es-ES }}");
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(WriteValue);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("global::SimpleTextTemplate.Tests.TestData.DoubleContextTestData.@DoubleStaticProperty");
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
            var sourceCode = GetWithCulture<DoubleContextTestData>(culture, "{{ DoubleStaticProperty }}");
            var (compilation, diagnostics) = await RunAsync(sourceCode);
            var interceptInfoList = compilation.GetInterceptInfo();

            await Assert.That(diagnostics).IsEmpty();

            var info = interceptInfoList.Dequeue();
            var method = info.Methods.Dequeue();

            await Assert.That(method.Name).IsEqualTo(WriteValue);
            await Assert.That(method.Text.Count).IsEqualTo(1);
            await Assert.That(method.Text[0]).IsEqualTo("global::SimpleTextTemplate.Tests.TestData.DoubleContextTestData.@DoubleStaticProperty");
            await Assert.That(method.Format).IsEqualTo(DefaultKeyword);
            await Assert.That(method.Provider).IsEqualTo(GlobalInvariantCulture);

            await Assert.That(info.Methods).IsEmpty();
            await Assert.That(interceptInfoList).IsEmpty();
        }
    }

    [Test]
    public async Task 静的プロパティ_メソッド引数で特定カルチャー指定()
    {
        var sourceCode = GetWithCulture<DoubleContextTestData>(JaJpCulture, "{{ DoubleStaticProperty }}");
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(WriteValue);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("global::SimpleTextTemplate.Tests.TestData.DoubleContextTestData.@DoubleStaticProperty");
        await Assert.That(method.Format).IsEqualTo(DefaultKeyword);
        await Assert.That(method.Provider).IsEqualTo(ProviderArgument);

        await Assert.That(info.Methods).IsEmpty();
        await Assert.That(interceptInfoList).IsEmpty();
    }

    [Test]
    public async Task 静的プロパティ_メソッド引数とテンプレート文字列で特定カルチャー指定()
    {
        var sourceCode = GetWithCulture<DoubleContextTestData>(JaJpCulture, "{{ DoubleStaticProperty::es-ES }}");
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(WriteValue);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("global::SimpleTextTemplate.Tests.TestData.DoubleContextTestData.@DoubleStaticProperty");
        await Assert.That(method.Format).IsEqualTo(DefaultKeyword);
        await Assert.That(method.Provider).IsEqualTo("esES");

        await Assert.That(info.Methods).IsEmpty();
        await Assert.That(interceptInfoList).IsEmpty();
    }

    [Test]
    public async Task プロパティ()
    {
        var sourceCode = Get<DoubleContextTestData>("{{ DoubleProperty }}");
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
        var sourceCode = Get<DoubleContextTestData>("{{ DoubleProperty:N3 }}");
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
        var sourceCode = Get<DoubleContextTestData>("{{ DoubleProperty::es-ES }}");
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
        var sourceCode = Get<DoubleContextTestData>("{{ DoubleProperty:N3:es-ES }}");
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
            var sourceCode = GetWithCulture<DoubleContextTestData>(culture, "{{ DoubleProperty }}");
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
        var sourceCode = GetWithCulture<DoubleContextTestData>(JaJpCulture, "{{ DoubleProperty }}");
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
        var sourceCode = GetWithCulture<DoubleContextTestData>(JaJpCulture, "{{ DoubleProperty::es-ES }}");
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
