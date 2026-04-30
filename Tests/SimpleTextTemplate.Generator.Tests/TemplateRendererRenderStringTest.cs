using System.Globalization;
using SimpleTextTemplate.Generator.Tests.Extensions;
using SimpleTextTemplate.Tests.TestData;
using static SimpleTextTemplate.Generator.Tests.Constants;
using static SimpleTextTemplate.Generator.Tests.GeneratorRunner;
using static SimpleTextTemplate.Generator.Tests.SourceCode;

namespace SimpleTextTemplate.Generator.Tests;

public sealed class TemplateRendererRenderStringTest
{
    [Test]
    public async Task 定数()
    {
        var sourceCode = Get("{{ StringConstantField }}", nameof(StringContextTestData));
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(Grow);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo(StringContextTestData.StringConstantField.Length.ToString(CultureInfo.InvariantCulture));
        await Assert.That(method.Format).IsNull();
        await Assert.That(method.Provider).IsNull();

        method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(DangerousWriteConstantLiteral);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("\"_StringConstantField\"u8");
        await Assert.That(method.Format).IsNull();
        await Assert.That(method.Provider).IsNull();

        await Assert.That(info.Methods).IsEmpty();
        await Assert.That(interceptInfoList).IsEmpty();
    }

    [Test]
    public async Task 複数の定数()
    {
        var sourceCode = Get("{{ StringConstantField }}{{ StringConstantField }}", nameof(StringContextTestData));
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(Grow);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo((StringContextTestData.StringConstantField.Length * 2).ToString(CultureInfo.InvariantCulture));
        await Assert.That(method.Format).IsNull();
        await Assert.That(method.Provider).IsNull();

        method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(DangerousWriteConstantLiteral);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo("\"_StringConstantField_StringConstantField\"u8");
        await Assert.That(method.Format).IsNull();
        await Assert.That(method.Provider).IsNull();

        await Assert.That(info.Methods).IsEmpty();
        await Assert.That(interceptInfoList).IsEmpty();
    }

    [Test]
    public Task 静的フィールド()
        => Test(nameof(StringContextTestData.StringStaticField), true);

    [Test]
    public Task フィールド()
        => Test(nameof(StringContextTestData.StringField), false);

    [Test]
    public Task 静的プロパティ()
        => Test(nameof(StringContextTestData.StringStaticProperty), true);

    [Test]
    public Task プロパティ()
        => Test(nameof(StringContextTestData.StringProperty), false);

    static async Task Test(string memberName, bool isStatic)
    {
        var templateText = $$$"""{{ {{{memberName}}} }}""";
        var contextArgument = GetContextArgumentString<StringContextTestData>(memberName, isStatic);

        var sourceCode = Get(templateText, nameof(StringContextTestData));
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(Grow);
        await Assert.That(method.Text.Count).IsEqualTo(3);
        await Assert.That(method.Text[0]).IsEqualTo("0");
        await Assert.That(method.Text[1]).IsEqualTo(Utf8GetMaxByteCount);
        await Assert.That(method.Text[2]).IsEqualTo($"{contextArgument}.Length");
        await Assert.That(method.Format).IsNull();
        await Assert.That(method.Provider).IsNull();

        method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(DangerousWriteString);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo(contextArgument);
        await Assert.That(method.Format).IsNull();
        await Assert.That(method.Provider).IsNull();

        await Assert.That(info.Methods).IsEmpty();
        await Assert.That(interceptInfoList).IsEmpty();
    }
}
