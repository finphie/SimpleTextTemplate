using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using SimpleTextTemplate.Generator.Extensions;

namespace SimpleTextTemplate.Generator;

static class SourceCodeGenerator
{
    public static void Execute(IMethodSymbol symbol, string source, SourceProductionContext context)
    {
        context.CancellationToken.ThrowIfCancellationRequested();

        var template = new SourceCodeTemplate(symbol, source);
        var result = template.TryParse();

        if (result == ParseResult.None)
        {
            context.ReportDiagnostic(DiagnosticDescriptors.UnknownError, symbol);
            return;
        }

        if (result == ParseResult.InvalidIdentifier)
        {
            context.ReportDiagnostic(DiagnosticDescriptors.InvalidIdentifierError, symbol);
            return;
        }

        var fileName = $"__{nameof(TemplateGenerator)}.{template.ClassName}.{template.MethodName}.Generated.cs";
        context.AddSource(fileName, SourceText.From(template.TransformText(), Encoding.UTF8));
    }
}
