using SimpleTextTemplate.Generator.Tests.Extensions;
using SimpleTextTemplate.Tests.TestData;
using static SimpleTextTemplate.Generator.Tests.Constants;
using static SimpleTextTemplate.Generator.Tests.GeneratorRunner;
using static SimpleTextTemplate.Generator.Tests.SourceCode;

namespace SimpleTextTemplate.Generator.Tests;

public sealed class TemplateRendererRenderByteArrayTest
{
    [Test]
    public Task 静的フィールド()
        => Test(nameof(ByteArrayContextTestData.BytesStaticField), true);

    [Test]
    public Task フィールド()
        => Test(nameof(ByteArrayContextTestData.BytesField), false);

    [Test]
    public Task 静的プロパティ()
        => Test(nameof(ByteArrayContextTestData.BytesStaticProperty), true);

    [Test]
    public Task プロパティ()
        => Test(nameof(ByteArrayContextTestData.BytesProperty), false);

    [Test]
    public Task 静的ReadOnlySpanプロパティ()
        => Test(nameof(ByteArrayContextTestData.BytesSpanStaticProperty), true);

    [Test]
    public Task ReadOnlySpanプロパティ()
        => Test(nameof(ByteArrayContextTestData.BytesSpanProperty), false);

    static async Task Test(string memberName, bool isStatic)
    {
        var templateText = $$$"""{{ {{{memberName}}} }}""";
        var contextArgument = GetContextArgumentString<ByteArrayContextTestData>(memberName, isStatic);

        var sourceCode = Get<ByteArrayContextTestData>(templateText);
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(Grow);
        await Assert.That(method.Text.Count).IsEqualTo(2);
        await Assert.That(method.Text[0]).IsEqualTo("0");
        await Assert.That(method.Text[1]).IsEqualTo($"{contextArgument}.Length");
        await Assert.That(method.Format).IsNull();
        await Assert.That(method.Provider).IsNull();

        method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(DangerousWriteLiteral);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo(contextArgument);
        await Assert.That(method.Format).IsNull();
        await Assert.That(method.Provider).IsNull();

        await Assert.That(info.Methods).IsEmpty();
        await Assert.That(interceptInfoList).IsEmpty();
    }
}
