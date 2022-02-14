using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace SimpleTextTemplate.Generator;

/// <summary>
/// テンプレートを生成するソースジェネレーターです。
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class TemplateGenerator : IIncrementalGenerator
{
    const string TemplateAttributeName = "global::SimpleTextTemplate.TemplateAttribute";
    const string TemplateFileAttributeName = "global::SimpleTextTemplate.TemplateFileAttribute";

    static readonly DiagnosticDescriptor STT1000Rule = new(
        "STT1000",
        "テンプレート解析時にエラーが発生しました。",
        "テンプレート解析時に未知のエラーが発生しました。",
        "Generator",
        DiagnosticSeverity.Error,
        true);

    static readonly DiagnosticDescriptor STT1001Rule = new(
        "STT1001",
        "識別子に無効な文字が含まれています。",
        "テンプレート解析時にエラーが発生しました。識別子に無効な文字が含まれています。",
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

                    if (fullName is not TemplateAttributeName and not TemplateFileAttributeName)
                    {
                        continue;
                    }

                    var source = attribute.ConstructorArguments[0].Value as string;
                    return (Symbol: symbol, FullName: fullName, Source: source);
                }

                return default;
            })
            .Where(x => !string.IsNullOrEmpty(x.Source));

        context.RegisterSourceOutput(methodSymbolsWithAttributeData, static (context, symbol) =>
        {
            var template = new SourceCodeTemplate(symbol.Symbol, symbol.Source!);
            var result = template.TryParse();

            if (result == ParseResult.None)
            {
                context.ReportDiagnostic(Diagnostic.Create(STT1000Rule, null));
                return;
            }

            if (result == ParseResult.InvalidIdentifier)
            {
                context.ReportDiagnostic(Diagnostic.Create(STT1001Rule, null));
                return;
            }

            var fileName = template.ClassName + "." + template.MethodName + ".Generated.cs";
            context.AddSource(fileName, SourceText.From(template.TransformText(), Encoding.UTF8));
        });
    }
}
