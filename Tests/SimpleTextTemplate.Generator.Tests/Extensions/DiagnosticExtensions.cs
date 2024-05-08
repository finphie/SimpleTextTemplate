using Microsoft.CodeAnalysis;

namespace SimpleTextTemplate.Generator.Tests.Extensions;

/// <summary>
/// <see cref="Diagnostic"/>クラス関連の拡張メソッド集です。
/// </summary>
static class DiagnosticExtensions
{
    /// <summary>
    /// 診断情報発生位置のソースコード文字列を取得します。
    /// </summary>
    /// <param name="diagnostic">診断情報</param>
    /// <returns>診断情報発生位置のソースコード文字列を返します。</returns>
    public static string GetText(this Diagnostic diagnostic)
    {
        var text = diagnostic.Location.SourceTree!.GetText();
        var span = diagnostic.Location.SourceSpan;

        return text.GetSubText(span).ToString();
    }
}
