using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using SimpleTextTemplate.Generator.Extensions;

namespace SimpleTextTemplate.Generator;

/// <summary>
/// テンプレートを生成するソースジェネレーターです。
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class TemplateFileGenerator : IIncrementalGenerator
{
    const string TemplateFileAttributeName = $"global::{nameof(SimpleTextTemplate)}.TemplateFileAttribute";

    /// <inheritdoc/>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var methodSymbols = context.GetMethodSymbolsWithAttributeArgument(TemplateFileAttributeName);
        var options = context.AnalyzerConfigOptionsProvider
            .Select(static (provider, token) =>
            {
                token.ThrowIfCancellationRequested();

                var globalOptions = provider.GlobalOptions;

                if (globalOptions.TryGetValue("build_property.SimpleTextTemplatePath", out var path) && path is not null)
                {
                    return path;
                }

                if (globalOptions.TryGetValue("build_property.ProjectDir", out var projectDirectory) && projectDirectory is not null)
                {
                    return projectDirectory;
                }

                return string.Empty;
            })
            .WithComparer(EqualityComparer<string>.Default);

        context.RegisterSourceOutput(methodSymbols.Combine(options), static (context, info) =>
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            var (method, directory) = info;
            var path = Path.Combine(directory, method.Argument);

            if (!File.Exists(path))
            {
                context.ReportDiagnostic(DiagnosticDescriptors.STT1002Rule, method.Symbol, path);
                return;
            }

            var template = new SourceCodeTemplate(method.Symbol, File.ReadAllBytes(path));
            var result = template.TryParse();

            if (result == ParseResult.None)
            {
                context.ReportDiagnostic(DiagnosticDescriptors.STT1000Rule, method.Symbol);
                return;
            }

            if (result == ParseResult.InvalidIdentifier)
            {
                context.ReportDiagnostic(DiagnosticDescriptors.STT1001Rule, method.Symbol);
                return;
            }

            var fileName = $"__{nameof(TemplateFileGenerator)}.{template.ClassName}.{template.MethodName}.Generated.cs";
            context.AddSource(fileName, SourceText.From(template.TransformText(), Encoding.UTF8));
        });
    }
}
