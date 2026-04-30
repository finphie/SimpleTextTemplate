using SimpleTextTemplate.Generator.Tests.Extensions;
using SimpleTextTemplate.Tests.TestData;
using static SimpleTextTemplate.Generator.Tests.GeneratorRunner;
using static SimpleTextTemplate.Generator.Tests.SourceCode;

namespace SimpleTextTemplate.Generator.Tests;

public sealed class TemplateRendererRenderNullTest
{
    [Test]
    public async Task 定数()
    {
        var sourceCode = Get<NullContextTestData>("{{ NullStringConstantField }}", "{{ NullObjectConstantField }}");
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        await Assert.That(interceptInfoList.Dequeue().Methods).IsEmpty();
        await Assert.That(interceptInfoList.Dequeue().Methods).IsEmpty();
        await Assert.That(interceptInfoList).IsEmpty();
    }

    [Test]
    public async Task 複数の定数()
    {
        var sourceCode = Get<NullContextTestData>("{{ NullStringConstantField }}{{ NullStringConstantField }}");
        var (compilation, diagnostics) = await RunAsync(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        await Assert.That(diagnostics).IsEmpty();

        await Assert.That(interceptInfoList.Dequeue().Methods).IsEmpty();
        await Assert.That(interceptInfoList).IsEmpty();
    }
}
