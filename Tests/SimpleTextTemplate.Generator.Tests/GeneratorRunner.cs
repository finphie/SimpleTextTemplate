using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Shouldly;
using SimpleTextTemplate.Generator.Tests.Core;

namespace SimpleTextTemplate.Generator.Tests;

/// <summary>
/// ソースジェネレーターを実行するクラスです。
/// </summary>
static class GeneratorRunner
{
    static Compilation _baseCompilation = null!;

    /// <summary>
    /// 初期化
    /// </summary>
    [ModuleInitializer]
    public static void Initialize()
    {
        var baseAssemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location)!;
        var assemblies = new[]
        {
            "System.Private.CoreLib.dll",
            "System.Runtime.dll",
            "System.Memory.dll"
        };

        var references = assemblies.Select(x => Path.Join(baseAssemblyPath, x))
            .Append(typeof(TemplateRenderer).Assembly.Location)
            .Append(typeof(TemplateWriter<>).Assembly.Location)
            .Append(typeof(ByteArrayContextTestData).Assembly.Location)
            .Select(static x => MetadataReference.CreateFromFile(x));

        _baseCompilation = CSharpCompilation.Create(
            "test",
            references: references);
    }

    /// <summary>
    /// ソースジェネレーターを実行します。
    /// </summary>
    /// <param name="source">ソースコード</param>
    /// <returns>コンパイル結果を返します。</returns>
    public static CompileResult Run(string source)
    {
        var generator = new TemplateGenerator();
        var options = CSharpParseOptions.Default.WithFeatures([new("InterceptorsNamespaces", "SimpleTextTemplate.Generator")]);
        var driver = CSharpGeneratorDriver.Create(generator).WithUpdatedParseOptions(options);

        var compilation = _baseCompilation.AddSyntaxTrees(CSharpSyntaxTree.ParseText(source, options));
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

        compilation.GetDiagnostics().ShouldBeEmpty();

        return new(outputCompilation, diagnostics);
    }
}
