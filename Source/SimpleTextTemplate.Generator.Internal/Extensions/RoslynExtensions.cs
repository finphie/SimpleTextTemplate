using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using SimpleTextTemplate.Generator.Specs;

namespace SimpleTextTemplate.Generator.Extensions;

/// <summary>
/// Roslyn関連の拡張メソッド集です。
/// </summary>
static class RoslynExtensions
{
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
                x.DeclaredAccessibility is Accessibility.Public or Accessibility.Internal or Accessibility.ProtectedOrInternal
                    && x.Kind is SymbolKind.Field or SymbolKind.Property);
    }

    /// <summary>
    /// 型情報を取得します。
    /// </summary>
    /// <param name="symbol">シンボル情報</param>
    /// <returns>指定されたシンボルの型を返します。</returns>
    /// <exception cref="InvalidOperationException">指定されたシンボルがフィールドやプロパティではありません。</exception>
    public static ITypeSymbol GetFieldOrPropertyType(this ISymbol symbol)
    {
        return symbol switch
        {
            IFieldSymbol fieldSymbol => fieldSymbol.Type,
            IPropertySymbol propertySymbol => propertySymbol.Type,
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

    /// <summary>
    /// 指定されたシンボルリストに実装されている書式設定インターフェイスの種類を取得します。
    /// </summary>
    /// <param name="symbols">シンボル情報のリスト</param>
    /// <returns>実装されている書式設定インターフェイスの種類を返します。</returns>
    public static FormattableType GetFormattableType(this IReadOnlyList<INamedTypeSymbol> symbols)
    {
        var result = FormattableType.None;

        foreach (var symbol in symbols)
        {
            var type = symbol.GetFormattableType();

            if (type == FormattableType.None)
            {
                continue;
            }

            result |= type;
        }

        return result;
    }

    /// <summary>
    /// 指定されたシンボルに実装されている書式設定インターフェイスの種類を取得します。
    /// </summary>
    /// <param name="symbol">シンボル情報</param>
    /// <returns>実装されている書式設定インターフェイスの種類を返します。</returns>
    public static FormattableType GetFormattableType(this INamedTypeSymbol symbol)
    {
        return symbol switch
        {
            { Name: nameof(IFormattable), ContainingNamespace: { Name: nameof(System), ContainingNamespace.IsGlobalNamespace: true } } => FormattableType.IFormattable,
            { Name: "ISpanFormattable", ContainingNamespace: { Name: nameof(System), ContainingNamespace.IsGlobalNamespace: true } } => FormattableType.ISpanFormattable,
            { Name: "IUtf8Formattable", ContainingNamespace: { Name: nameof(System), ContainingNamespace.IsGlobalNamespace: true } } => FormattableType.IUtf8Formattable,
            _ => FormattableType.None
        };
    }
}
