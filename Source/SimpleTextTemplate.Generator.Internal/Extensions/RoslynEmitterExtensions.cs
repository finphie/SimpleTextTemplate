using Microsoft.CodeAnalysis;

namespace SimpleTextTemplate.Generator.Extensions;

/// <summary>
/// Roslynのコード生成に関する拡張メソッド集です。
/// </summary>
static class RoslynEmitterExtensions
{
    /// <summary>
    /// パラメーターのテキストを取得します。
    /// </summary>
    /// <param name="symbol">パラメーターシンボル</param>
    /// <returns>パラメーターのテキストを返します。</returns>
    public static string GetParameterText(this IParameterSymbol symbol)
    {
        var modifiers = symbol.RefKind.GetParameterModifiers();
        var typeName = symbol.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var name = symbol.Name;
        var defaultValue = symbol.HasExplicitDefaultValue ? $"= {GetDefaultValue(symbol)}" : string.Empty;

        var parameters = new[] { modifiers, typeName, name, defaultValue }
            .Where(static x => !string.IsNullOrEmpty(x));
        return string.Join(" ", parameters);
    }

    static string GetParameterModifiers(this RefKind refKind)
    {
        return refKind switch
        {
            RefKind.None => string.Empty,
            RefKind.Ref => "ref",
            RefKind.Out => "out",
            RefKind.In => "in",
            RefKind.RefReadOnlyParameter => "ref readonly",
            _ => throw new ArgumentOutOfRangeException(nameof(refKind), refKind, "Unexpected RefKind value.")
        };
    }

    static string GetDefaultValue(this IParameterSymbol symbol)
    {
        return !symbol.HasExplicitDefaultValue
            ? string.Empty
            : symbol.ExplicitDefaultValue switch
            {
                null => "null",
                _ => throw new InvalidOperationException("サポート対象外のデフォルト値です。")
            };
    }
}
