using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using SimpleTextTemplate.Tests.TestData;

namespace SimpleTextTemplate.Generator.Tests;

static class GeneratorRunner
{
    static Compilation _baseCompilation = null!;

    [ModuleInitializer]
    public static void Initialize()
    {
        var baseAssemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location);
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

    public static async Task<CompileResult> RunAsync(string source)
    {
        var generator = new TemplateGenerator();
        var options = CSharpParseOptions.Default.WithFeatures([new("InterceptorsNamespaces", "SimpleTextTemplate.Generator")]);
        var driver = CSharpGeneratorDriver.Create(generator).WithUpdatedParseOptions(options);

        var compilation = _baseCompilation.AddSyntaxTrees(CSharpSyntaxTree.ParseText(source, options));
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

        await Assert.That(compilation.GetDiagnostics()).IsEmpty();

        return new(outputCompilation, diagnostics);
    }
}
