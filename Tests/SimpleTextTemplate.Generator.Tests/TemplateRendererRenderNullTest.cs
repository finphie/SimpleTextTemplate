using Shouldly;
using SimpleTextTemplate.Generator.Tests.Core;
using SimpleTextTemplate.Generator.Tests.Extensions;
using Xunit;
using static SimpleTextTemplate.Generator.Tests.Constants;
using static SimpleTextTemplate.Generator.Tests.GeneratorRunner;
using static SimpleTextTemplate.Generator.Tests.SourceCode;

namespace SimpleTextTemplate.Generator.Tests;

public sealed class TemplateRendererRenderNullTest
{
    [Fact]
    public void 定数()
    {
        var sourceCode = Get(["{{ NullStringConstantField }}", "{{ NullObjectConstantField }}"], nameof(NullContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        interceptInfoList.Dequeue().Methods.ShouldBeEmpty();
        interceptInfoList.Dequeue().Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }

    [Fact]
    public void 複数の定数()
    {
        var sourceCode = Get("{{ NullStringConstantField }}{{ NullStringConstantField }}", nameof(NullContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        interceptInfoList.Dequeue().Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }
}
