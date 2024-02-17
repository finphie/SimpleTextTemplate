using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using SimpleTextTemplate.Generator.Specs;

namespace SimpleTextTemplate.Generator.Extensions;

/// <summary>
/// Roslyn関連の拡張メソッド集です。
/// </summary>
static class RoslynExtensions
{
    /// <summary>
    /// InterceptsLocation属性の情報を取得します。
    /// </summary>
    /// <param name="invocation">メソッド呼び出し</param>
    /// <param name="cancellationToken">キャンセル要求を行うためのトークン</param>
    /// <returns>指定されたメソッドからInterceptsLocation属性の情報を生成して返します。</returns>
    /// <exception cref="ArgumentException">引数が<see cref="InvocationExpressionSyntax"/>ではありません。</exception>
    public static InterceptsLocationInfo GetInterceptsLocationInfo(this IInvocationOperation invocation, CancellationToken cancellationToken)
    {
        if (invocation.Syntax is not InvocationExpressionSyntax { Expression: MemberAccessExpressionSyntax memberAccessorExpressionSyntax })
        {
            throw new ArgumentException("InvocationExpressionSyntaxである必要があります。", nameof(invocation));
        }

        var operationSyntaxTree = invocation.Syntax.SyntaxTree;
        var lineSpan = operationSyntaxTree.GetLineSpan(memberAccessorExpressionSyntax.Name.Span, cancellationToken);

        return new(
            FilePath: GetInterceptorFilePath(invocation, operationSyntaxTree),
            Line: lineSpan.StartLinePosition.Line + 1,
            Column: lineSpan.StartLinePosition.Character + 1);

        static string GetInterceptorFilePath(IInvocationOperation invocation, SyntaxTree operationSyntaxTree)
        {
            var filePath = operationSyntaxTree.FilePath;
            return invocation.SemanticModel?.Compilation.Options.SourceReferenceResolver?.NormalizePath(filePath, null) ?? filePath;
        }
    }

    /// <summary>
    /// フィールドとプロパティ情報を取得します。
    /// </summary>
    /// <param name="type">型情報</param>
    /// <returns>
    /// アクセシビリティが<see cref="Accessibility.Public"/>や<see cref="Accessibility.Internal"/>、<see cref="Accessibility.ProtectedOrInternal"/>のいずれかとなる、
    /// フィールドまたはプロパティ情報を返します。
    /// </returns>
    public static IEnumerable<ISymbol> GetFieldsAndProperties(this ITypeSymbol type)
    {
        return type.GetMembers()
            .Where(static x =>
                x.DeclaredAccessibility is Accessibility.Public or Accessibility.Internal or Accessibility.ProtectedOrInternal &&
                x.Kind is SymbolKind.Field or SymbolKind.Property);
    }

    /// <summary>
    /// 型情報を取得します。
    /// </summary>
    /// <param name="symbol">シンボル情報</param>
    /// <returns>指定されたシンボル（フィールドやプロパティ、メソッド戻り値、イベント、パラメーター）の型を返します。</returns>
    /// <exception cref="InvalidOperationException">指定されたシンボルがフィールドやプロパティ、メソッド戻り値、イベント、パラメーターのいずれにも該当しません。</exception>
    public static ITypeSymbol GetMemberType(this ISymbol symbol)
    {
        return symbol switch
        {
            IFieldSymbol fieldSymbol => fieldSymbol.Type,
            IPropertySymbol propertySymbol => propertySymbol.Type,
            IMethodSymbol methodSymbol => methodSymbol.ReturnType,
            IEventSymbol eventSymbol => eventSymbol.Type,
            IParameterSymbol parameterSymbol => parameterSymbol.Type,
            _ => throw new InvalidOperationException($"Unexpected type '{symbol.ToDisplayString()}'.")
        };
    }

    /// <summary>
    /// <see cref="ReadOnlySpan{T}"/>の型情報を取得します。
    /// </summary>
    /// <param name="compilation">コンパイルオブジェクト</param>
    /// <param name="specialType"><see cref="ReadOnlySpan{T}"/>の型パラメーターの型</param>
    /// <returns>指定された型パラメーターとなる<see cref="ReadOnlySpan{T}"/>の型情報を返します。</returns>
    /// <exception cref="InvalidOperationException">型情報を取得できませんでした。</exception>
    public static INamedTypeSymbol GetReadOnlySpanTypeSymbol(this Compilation compilation, SpecialType specialType)
    {
        var readOnlySpanSymbol = compilation.GetTypeByMetadataName("System.ReadOnlySpan`1");
        var symbol = readOnlySpanSymbol?.Construct(compilation.GetSpecialType(specialType))
            ?? throw new InvalidOperationException($"Type System.ReadOnlySpan<{specialType}> is not found.");

        return symbol;
    }

    /// <summary>
    /// エスケープされた文字列リテラルを取得します。
    /// </summary>
    /// <param name="value">文字列</param>
    /// <returns>""内で使用できるエスケープされた文字列リテラルを返します。</returns>
    public static string ToLiteral(this string value)
        => SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(value)).ToFullString();

    /// <summary>
    /// エスケープされたUTF-8文字列リテラルを取得します。
    /// </summary>
    /// <param name="value">文字列</param>
    /// <returns>""内で使用できるエスケープされたUTF-8文字列リテラルを返します。</returns>
    public static string ToUtf8Literal(this string value)
        => $"{value.ToLiteral()}u8";
}
