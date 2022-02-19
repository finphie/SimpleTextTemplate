using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using SimpleTextTemplate.Generator.Extensions;

namespace SimpleTextTemplate.Generator;

/// <summary>
/// テンプレートを生成するソースジェネレーターです。
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class TemplateGenerator : IIncrementalGenerator
{
    const string TemplateAttributeName = $"global::{nameof(SimpleTextTemplate)}.TemplateAttribute";

    /// <inheritdoc/>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var methodSymbols = context.GetMethodSymbolsWithAttributeArgument(TemplateAttributeName);

        context.RegisterSourceOutput(methodSymbols, static (context, method) =>
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            var template = new SourceCodeTemplate(method.Symbol, method.Argument);
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

            var fileName = $"__{nameof(TemplateGenerator)}.{template.ClassName}.{template.MethodName}.Generated.cs";
            context.AddSource(fileName, SourceText.From(template.TransformText(), Encoding.UTF8));
        });
    }
}
