using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using SimpleTextTemplate.Generator.Helpers;

namespace SimpleTextTemplate.Generator;

/// <summary>
/// テンプレートを生成するソースジェネレーターです。
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class TemplateFileGenerator : IIncrementalGenerator
{
    const string TemplateFileAttributeName = $"global::{nameof(SimpleTextTemplate)}.TemplateFileAttribute";

    static readonly DiagnosticDescriptor STT1002Rule = new(
        "STT1002",
        "指定されたファイルが存在しません。",
        "テンプレート解析時にエラーが発生しました。指定されたファイルが存在しません。Path: {0}",
        "Generator",
        DiagnosticSeverity.Error,
        true);

    /// <inheritdoc/>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var methodSymbols = context.SyntaxProvider
            .CreateSyntaxProvider(
                static (node, _) => node is MethodDeclarationSyntax { AttributeLists.Count: > 0 },
                static (context, token) => (IMethodSymbol)context.SemanticModel.GetDeclaredSymbol(context.Node, token)!);

        var methodSymbolsWithAttributeData = methodSymbols
            .Select(static (symbol, token) =>
            {
                foreach (var attribute in symbol.GetAttributes())
                {
                    token.ThrowIfCancellationRequested();

                    var fullName = attribute.AttributeClass?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

                    if (fullName != TemplateFileAttributeName)
                    {
                        continue;
                    }

                    var path = attribute.ConstructorArguments[0].Value as string;
                    return (Symbol: symbol, Path: path);
                }

                return default;
            })
            .Where(x => !string.IsNullOrEmpty(x.Path));

        var options = context.AnalyzerConfigOptionsProvider.Select(static (provider, token) =>
        {
            token.ThrowIfCancellationRequested();

            var globalOptions = provider.GlobalOptions;

            if (globalOptions.TryGetValue("build_property.SimpleTextTemplatePath", out var path) || string.IsNullOrEmpty(path))
            {
                return path;
            }

            if (globalOptions.TryGetValue("build_property.ProjectDir", out var projectDirectory) || string.IsNullOrEmpty(projectDirectory))
            {
                return projectDirectory;
            }

            return string.Empty;
        });

        context.RegisterSourceOutput(methodSymbolsWithAttributeData, static (context, method) =>
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            if (!File.Exists(method.Path))
            {
                ReportDiagnostic(STT1002Rule);
                return;
            }

            var template = new SourceCodeTemplate(method.Symbol, File.ReadAllBytes(method.Path));
            var result = template.TryParse();

            if (result == ParseResult.None)
            {
                ReportDiagnostic(STT1000Rule);
                return;
            }

            if (result == ParseResult.InvalidIdentifier)
            {
                ReportDiagnostic(STT1001Rule);
                return;
            }

            var fileName = $"__{nameof(TemplateFileGenerator)}.{template.ClassName}.{template.MethodName}.Generated.cs";
            context.AddSource(fileName, SourceText.From(template.TransformText(), Encoding.UTF8));

            void ReportDiagnostic(DiagnosticDescriptor descriptor)
            {
                var location = LocationHelper.GetLocation(method.Symbol);
                context.ReportDiagnostic(Diagnostic.Create(descriptor, location, method.Path));
            }
        });
    }
}
