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

    [Fact]
    public void 静的フィールド()
    {
        var sourceCode = Get(["{{ NullStringStaticField }}", "{{ NullObjectStaticField }}"], nameof(NullContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(WriteString);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("global::SimpleTextTemplate.Generator.Tests.Core.NullContextTestData.@NullStringStaticField");
        method.Format.ShouldBeNull();
        method.Provider.ShouldBeNull();

        info.Methods.ShouldBeEmpty();
        info = interceptInfoList.Dequeue();
        method = info.Methods.Dequeue();

        method.Name.ShouldBe(WriteValue);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("global::SimpleTextTemplate.Generator.Tests.Core.NullContextTestData.@NullObjectStaticField");
        method.Format.ShouldBe(DefaultKeyword);
        method.Provider.ShouldBe(GlobalInvariantCulture);

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }

    [Fact]
    public void フィールド()
    {
        var sourceCode = Get(["{{ NullStringField }}", "{{ NullObjectField }}"], nameof(NullContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(WriteString);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@NullStringField");
        method.Format.ShouldBeNull();
        method.Provider.ShouldBeNull();

        info.Methods.ShouldBeEmpty();
        info = interceptInfoList.Dequeue();
        method = info.Methods.Dequeue();

        method.Name.ShouldBe(WriteValue);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@NullObjectField");
        method.Format.ShouldBe(DefaultKeyword);
        method.Provider.ShouldBe(GlobalInvariantCulture);

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }

    [Fact]
    public void 静的プロパティ()
    {
        var sourceCode = Get(["{{ NullStringStaticProperty }}", "{{ NullObjectStaticProperty }}"], nameof(NullContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(WriteString);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("global::SimpleTextTemplate.Generator.Tests.Core.NullContextTestData.@NullStringStaticProperty");
        method.Format.ShouldBeNull();
        method.Provider.ShouldBeNull();

        info.Methods.ShouldBeEmpty();
        info = interceptInfoList.Dequeue();
        method = info.Methods.Dequeue();

        method.Name.ShouldBe(WriteValue);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("global::SimpleTextTemplate.Generator.Tests.Core.NullContextTestData.@NullObjectStaticProperty");
        method.Format.ShouldBe(DefaultKeyword);
        method.Provider.ShouldBe(GlobalInvariantCulture);

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }

    [Fact]
    public void プロパティ()
    {
        var sourceCode = Get(["{{ NullStringProperty }}", "{{ NullObjectProperty }}"], nameof(NullContextTestData));
        var (compilation, diagnostics) = Run(sourceCode);
        var interceptInfoList = compilation.GetInterceptInfo();

        diagnostics.ShouldBeEmpty();

        var info = interceptInfoList.Dequeue();
        var method = info.Methods.Dequeue();

        method.Name.ShouldBe(WriteString);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@NullStringProperty");
        method.Format.ShouldBeNull();
        method.Provider.ShouldBeNull();

        info.Methods.ShouldBeEmpty();
        info = interceptInfoList.Dequeue();
        method = info.Methods.Dequeue();

        method.Name.ShouldBe(WriteValue);
        method.Text.Count.ShouldBe(1);
        method.Text[0].ShouldBe("global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@NullObjectProperty");
        method.Format.ShouldBe(DefaultKeyword);
        method.Provider.ShouldBe(GlobalInvariantCulture);

        info.Methods.ShouldBeEmpty();
        interceptInfoList.ShouldBeEmpty();
    }
}
