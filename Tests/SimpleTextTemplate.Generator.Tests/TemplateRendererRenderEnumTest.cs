using SimpleTextTemplate.Generator.Tests.Extensions;
using SimpleTextTemplate.Tests.TestData;
using static SimpleTextTemplate.Generator.Tests.Constants;
using static SimpleTextTemplate.Generator.Tests.GeneratorRunner;
using static SimpleTextTemplate.Generator.Tests.SourceCode;

namespace SimpleTextTemplate.Generator.Tests;

public sealed class TemplateRendererRenderEnumTest
{
    [Test]
    public async Task 定数()
    {
        var sourceCode = Get("{{ EnumConstantField }}", nameof(EnumContextTestData));
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(Grow);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("5");
        await Assert.That(method.Format).IsNull();
        await Assert.That(method.Provider).IsNull();

        method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(DangerousWriteConstantLiteral);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("\"Test1\"u8");
        await Assert.That(method.Format).IsNull();
        await Assert.That(method.Provider).IsNull();

        await Assert.That(info.Methods).IsEmpty();
        await Assert.That(interceptInfoList).IsEmpty();
    }

    [Test]
    public async Task 定数_FormatにD指定()
    {
        var sourceCode = Get(["{{ EnumConstantField:D }}", "{{ EnumConstantField:d }}"], nameof(EnumContextTestData));
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(Grow);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("1");
        await Assert.That(method.Format).IsNull();
        await Assert.That(method.Provider).IsNull();

        method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(DangerousWriteConstantLiteral);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("\"1\"u8");
        await Assert.That(method.Format).IsNull();
        await Assert.That(method.Provider).IsNull();

        await Assert.That(info.Methods).IsEmpty();
        info = interceptInfoList.Dequeue();
        method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(Grow);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("1");
        await Assert.That(method.Format).IsNull();
        await Assert.That(method.Provider).IsNull();

        method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(DangerousWriteConstantLiteral);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("\"1\"u8");
        await Assert.That(method.Format).IsNull();
        await Assert.That(method.Provider).IsNull();

        await Assert.That(info.Methods).IsEmpty();
        await Assert.That(interceptInfoList).IsEmpty();
    }

    [Test]
    public async Task 定数_FormatにD以外指定()
    {
        var sourceCode = Get("{{ EnumConstantField:G }}", nameof(EnumContextTestData));
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(WriteEnum);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("global::SimpleTextTemplate.Generator.Tests.Core.EnumContextTestData.@EnumConstantField");
        await Assert.That(method.Format).IsEqualTo("\"G\"");
        await Assert.That(method.Provider).IsNull();

        await Assert.That(info.Methods).IsEmpty();
        await Assert.That(interceptInfoList).IsEmpty();
    }

    [Test]
    public async Task 定数が無効な値()
    {
        var sourceCode = Get("{{ EnumConstantFieldInvalidNumber }}", nameof(EnumContextTestData));
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(Grow);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("2");
        await Assert.That(method.Format).IsNull();
        await Assert.That(method.Provider).IsNull();

        method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(DangerousWriteConstantLiteral);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("\"99\"u8");
        await Assert.That(method.Format).IsNull();
        await Assert.That(method.Provider).IsNull();

        await Assert.That(info.Methods).IsEmpty();
        await Assert.That(interceptInfoList).IsEmpty();
    }

    [Test]
    public async Task Flags属性を付与したEnumの定数()
    {
        var sourceCode = Get("{{ FlagEnumConstantField }}", nameof(EnumContextTestData));
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(WriteEnum);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("global::SimpleTextTemplate.Generator.Tests.Core.EnumContextTestData.@FlagEnumConstantField");
        await Assert.That(method.Format).IsEqualTo(DefaultKeyword);
        await Assert.That(method.Provider).IsNull();

        await Assert.That(info.Methods).IsEmpty();
        await Assert.That(interceptInfoList).IsEmpty();
    }

    [Test]
    public async Task Flags属性を付与したEnumの定数_Format指定()
    {
        var sourceCode = Get("{{ FlagEnumConstantField:D }}", nameof(EnumContextTestData));
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(WriteEnum);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("global::SimpleTextTemplate.Generator.Tests.Core.EnumContextTestData.@FlagEnumConstantField");
        await Assert.That(method.Format).IsEqualTo("\"D\"");
        await Assert.That(method.Provider).IsNull();

        await Assert.That(info.Methods).IsEmpty();
        await Assert.That(interceptInfoList).IsEmpty();
    }

    [Test]
    public async Task 複数の定数()
    {
        var sourceCode = Get("{{ EnumConstantField }}{{ EnumConstantField }}", nameof(EnumContextTestData));
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(Grow);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("10");
        await Assert.That(method.Format).IsNull();
        await Assert.That(method.Provider).IsNull();

        method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(DangerousWriteConstantLiteral);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("\"Test1Test1\"u8");
        await Assert.That(method.Format).IsNull();
        await Assert.That(method.Provider).IsNull();

        await Assert.That(info.Methods).IsEmpty();
        await Assert.That(interceptInfoList).IsEmpty();
    }

    [Test]
    public async Task 静的フィールド()
    {
        var sourceCode = Get("{{ EnumStaticField }}", nameof(EnumContextTestData));
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(WriteEnum);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("global::SimpleTextTemplate.Generator.Tests.Core.EnumContextTestData.@EnumStaticField");
        await Assert.That(method.Format).IsEqualTo(DefaultKeyword);
        await Assert.That(method.Provider).IsNull();

        await Assert.That(info.Methods).IsEmpty();
        await Assert.That(interceptInfoList).IsEmpty();
    }

    [Test]
    public async Task 静的フィールド_Format指定()
    {
        var sourceCode = Get("{{ EnumStaticField:D }}", nameof(EnumContextTestData));
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(WriteEnum);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("global::SimpleTextTemplate.Generator.Tests.Core.EnumContextTestData.@EnumStaticField");
        await Assert.That(method.Format).IsEqualTo("\"D\"");
        await Assert.That(method.Provider).IsNull();

        await Assert.That(info.Methods).IsEmpty();
        await Assert.That(interceptInfoList).IsEmpty();
    }

    [Test]
    public async Task フィールド()
    {
        var sourceCode = Get("{{ EnumField }}", nameof(EnumContextTestData));
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(WriteEnum);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@EnumField");
        await Assert.That(method.Format).IsEqualTo(DefaultKeyword);
        await Assert.That(method.Provider).IsNull();

        await Assert.That(info.Methods).IsEmpty();
        await Assert.That(interceptInfoList).IsEmpty();
    }

    [Test]
    public async Task フィールド_Format指定()
    {
        var sourceCode = Get("{{ EnumField:D }}", nameof(EnumContextTestData));
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(WriteEnum);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@EnumField");
        await Assert.That(method.Format).IsEqualTo("\"D\"");
        await Assert.That(method.Provider).IsNull();

        await Assert.That(info.Methods).IsEmpty();
        await Assert.That(interceptInfoList).IsEmpty();
    }

    [Test]
    public async Task 静的プロパティ()
    {
        var sourceCode = Get("{{ EnumStaticProperty }}", nameof(EnumContextTestData));
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(WriteEnum);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("global::SimpleTextTemplate.Generator.Tests.Core.EnumContextTestData.@EnumStaticProperty");
        await Assert.That(method.Format).IsEqualTo(DefaultKeyword);
        await Assert.That(method.Provider).IsNull();

        await Assert.That(info.Methods).IsEmpty();
        await Assert.That(interceptInfoList).IsEmpty();
    }

    [Test]
    public async Task 静的プロパティ_Format指定()
    {
        var sourceCode = Get("{{ EnumStaticProperty:D }}", nameof(EnumContextTestData));
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(WriteEnum);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("global::SimpleTextTemplate.Generator.Tests.Core.EnumContextTestData.@EnumStaticProperty");
        await Assert.That(method.Format).IsEqualTo("\"D\"");
        await Assert.That(method.Provider).IsNull();

        await Assert.That(info.Methods).IsEmpty();
        await Assert.That(interceptInfoList).IsEmpty();
    }

    [Test]
    public async Task プロパティ()
    {
        var sourceCode = Get("{{ EnumProperty }}", nameof(EnumContextTestData));
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(WriteEnum);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@EnumProperty");
        await Assert.That(method.Format).IsEqualTo(DefaultKeyword);
        await Assert.That(method.Provider).IsNull();

        await Assert.That(info.Methods).IsEmpty();
        await Assert.That(interceptInfoList).IsEmpty();
    }

    [Test]
    public async Task プロパティ_Format指定()
    {
        var sourceCode = Get("{{ EnumProperty:D }}", nameof(EnumContextTestData));
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(WriteEnum);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@EnumProperty");
        await Assert.That(method.Format).IsEqualTo("\"D\"");
        await Assert.That(method.Provider).IsNull();

        await Assert.That(info.Methods).IsEmpty();
        await Assert.That(interceptInfoList).IsEmpty();
    }
}
