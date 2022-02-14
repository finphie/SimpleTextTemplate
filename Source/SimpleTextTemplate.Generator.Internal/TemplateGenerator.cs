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
        "テンプレート解析時にエラーが発生しました。識別子に無効な文字が含まれています。Source: {0}",
        "Generator",
        DiagnosticSeverity.Error,
        true);

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

                    if (fullName is not TemplateAttributeName and not TemplateFileAttributeName)
                    {
                        continue;
                    }

                    var isPath = fullName == TemplateFileAttributeName;
                    var sourceOrPath = attribute.ConstructorArguments[0].Value as string;

                    return (Symbol: symbol, IsPath: isPath, SourceOrPath: sourceOrPath);
                }

                return default;
            })
            .Where(x => !string.IsNullOrEmpty(x.SourceOrPath));

        context.RegisterSourceOutput(methodSymbolsWithAttributeData, static (context, method) =>
        {
            SourceCodeTemplate template;

            if (method.IsPath)
            {
                if (!File.Exists(method.SourceOrPath))
                {
                    ReportDiagnostic(STT1002Rule);
                    return;
                }

                template = new(method.Symbol, File.ReadAllBytes(method.SourceOrPath));
            }
            else
            {
                template = new(method.Symbol, method.SourceOrPath!);
            }

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

            var fileName = template.ClassName + "." + template.MethodName + ".Generated.cs";
            context.AddSource(fileName, SourceText.From(template.TransformText(), Encoding.UTF8));

            void ReportDiagnostic(DiagnosticDescriptor descriptor)
            {
                var location = LocationHelper.GetLocation(method.Symbol);
                context.ReportDiagnostic(Diagnostic.Create(descriptor, location, method.SourceOrPath));
            }
        });
    }
}
