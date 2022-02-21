using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace SimpleTextTemplate.Generator.Extensions;

/// <summary>
/// <see cref="AnalyzerConfigOptions"/>クラス関連の拡張メソッド集です。
/// </summary>
static class AnalyzerConfigOptionsExtensions
{
    /// <summary>
    /// 指定されたビルドプロパティの値を取得します。
    /// </summary>
    /// <param name="options">アナライザーオプション</param>
    /// <param name="key">キー</param>
    /// <param name="value">値</param>
    /// <returns>
    /// 指定されたビルドプロパティを取得できた場合は<see langword="true"/>、
    /// それ以外の場合は<see langword="false"/>を返します。
    /// </returns>
    public static bool TryGetBuildProperty(this AnalyzerConfigOptions options, string key, [MaybeNullWhen(false)] out string value)
        => options.TryGetValue($"build_property.{key}", out value) && !string.IsNullOrEmpty(value);
}
