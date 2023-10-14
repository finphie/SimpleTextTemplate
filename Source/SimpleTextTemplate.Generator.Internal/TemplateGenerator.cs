using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using SimpleTextTemplate.Generator.Extensions;

namespace SimpleTextTemplate.Generator;

/// <summary>
/// 属性引数からテンプレート文字列を取得して、ソースコードを生成します。
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class TemplateGenerator : IIncrementalGenerator
{
    const string TemplateAttributeName = $"global::{nameof(SimpleTextTemplate)}.TemplateAttribute";

    /// <inheritdoc/>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var methods = context.GetMethodSymbolsWithAttributeArgumentProvider(TemplateAttributeName);

        context.RegisterSourceOutput(methods, (context, method) =>
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            var source = Encoding.UTF8.GetBytes(method.Argument);
            var template = new SourceCodeTemplate(method.Symbol, source);
            var result = template.TryParse();

            if (result == ParseResult.InvalidIdentifier)
            {
                context.ReportDiagnostic(DiagnosticDescriptors.InvalidIdentifierError, method.Symbol);
                return;
            }

            if (result != ParseResult.Success)
            {
                context.ReportDiagnostic(DiagnosticDescriptors.UnknownError, method.Symbol);
                return;
            }

            var fileName = $"__{nameof(TemplateGenerator)}.{template.ClassName}.{template.MethodName}.Generated.cs";
            context.AddSource(fileName, SourceText.From(template.TransformText(), Encoding.UTF8));
        });
    }
}
