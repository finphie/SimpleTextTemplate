using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SimpleTextTemplate.Generator.Extensions;

/// <summary>
/// <see cref="IncrementalGeneratorInitializationContext"/>構造体関連の拡張メソッド集です。
/// </summary>
static class IncrementalGeneratorInitializationContextExtensions
{
    /// <summary>
    /// 指定された属性が関連付けられているメソッドのシンボルと引数を取得します。
    /// </summary>
    /// <param name="context">コンテキスト</param>
    /// <param name="attributeName">属性の完全修飾名</param>
    /// <returns>ソースジェネレーターで使用するプロバイダーを返します。</returns>
    public static IncrementalValuesProvider<MethodSymbolWithArgument> GetMethodSymbolsWithAttributeArgument(this IncrementalGeneratorInitializationContext context, string attributeName)
    {
        return context.GetMethodSymbols()
            .Select((symbol, token) =>
            {
                foreach (var attribute in symbol.GetAttributes())
                {
                    token.ThrowIfCancellationRequested();

                    var fullName = attribute.AttributeClass?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

                    if (fullName != attributeName)
                    {
                        continue;
                    }

                    var source = attribute.ConstructorArguments[0].Value as string;
                    return new MethodSymbolWithArgument(symbol, source!);
                }

                return default;
            })
            .Where(x => !string.IsNullOrEmpty(x.Argument))
            .WithComparer(EqualityComparer<MethodSymbolWithArgument>.Default);
    }

    static IncrementalValuesProvider<IMethodSymbol> GetMethodSymbols(this IncrementalGeneratorInitializationContext context)
    {
        return context.SyntaxProvider
            .CreateSyntaxProvider(
                static (node, _) => node is MethodDeclarationSyntax { AttributeLists.Count: > 0 },
                static (context, token) => (IMethodSymbol)context.SemanticModel.GetDeclaredSymbol(context.Node, token)!);
    }
}
