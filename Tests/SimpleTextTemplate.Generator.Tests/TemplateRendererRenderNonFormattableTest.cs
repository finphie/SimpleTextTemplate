using SimpleTextTemplate.Generator.Tests.Extensions;
using SimpleTextTemplate.Tests.TestData;
using static SimpleTextTemplate.Generator.Tests.Constants;
using static SimpleTextTemplate.Generator.Tests.GeneratorRunner;
using static SimpleTextTemplate.Generator.Tests.SourceCode;

namespace SimpleTextTemplate.Generator.Tests;

public sealed class TemplateRendererRenderNonFormattableTest
{
    [Test]
    public Task 静的フィールド()
        => Test(nameof(NonFormattableContextTestData.NonFormattableStaticField), true);

    [Test]
    public Task フィールド()
        => Test(nameof(NonFormattableContextTestData.NonFormattableField), false);

    [Test]
    public Task 静的プロパティ()
        => Test(nameof(NonFormattableContextTestData.NonFormattableStaticProperty), true);

    [Test]
    public Task プロパティ()
        => Test(nameof(NonFormattableContextTestData.NonFormattableProperty), false);

    static async Task Test(string memberName, bool isStatic)
    {
        var templateText = $$$"""{{ {{{memberName}}} }}""";
        var contextArgument = GetContextArgumentString<NonFormattableContextTestData>(memberName, isStatic);

        var sourceCode = Get(templateText, nameof(NonFormattableContextTestData));
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        await Assert.That(method.Name).IsEqualTo(WriteValue);
        await Assert.That(method.Text.Count).IsEqualTo(1);
        await Assert.That(method.Text[0]).IsEqualTo(contextArgument);
        await Assert.That(method.Format).IsEqualTo(DefaultKeyword);
        await Assert.That(method.Provider).IsEqualTo(GlobalInvariantCulture);

        await Assert.That(info.Methods).IsEmpty();
        await Assert.That(interceptInfoList).IsEmpty();
    }
}
