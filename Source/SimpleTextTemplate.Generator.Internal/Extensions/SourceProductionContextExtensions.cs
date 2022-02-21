using Microsoft.CodeAnalysis;
using SimpleTextTemplate.Generator.Helpers;

namespace SimpleTextTemplate.Generator.Extensions;

/// <summary>
/// <see cref="SourceProductionContext"/>構造体関連の拡張メソッド集です。
/// </summary>
static class SourceProductionContextExtensions
{
    /// <summary>
    /// コンパイルメッセージを追加します。
    /// </summary>
    /// <param name="context">コンテキスト</param>
    /// <param name="descriptor">コンパイルメッセージに関する説明</param>
    /// <param name="symbol">メソッドシンボル</param>
    /// <param name="messageArgs">メッセージ引数</param>
    public static void ReportDiagnostic(this SourceProductionContext context, DiagnosticDescriptor descriptor, IMethodSymbol symbol, params object?[]? messageArgs)
    {
        var location = LocationHelper.GetLocation(symbol);
        context.ReportDiagnostic(Diagnostic.Create(descriptor, location, messageArgs));
    }
}
