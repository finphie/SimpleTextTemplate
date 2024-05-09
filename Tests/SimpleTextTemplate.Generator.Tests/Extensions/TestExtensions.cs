using System.Runtime.CompilerServices;

namespace SimpleTextTemplate.Generator.Tests.Extensions;

/// <summary>
/// テストで使用する拡張メソッド集です。
/// </summary>
static class TestExtensions
{
    /// <summary>
    /// 指定された式の文字列を返します。
    /// </summary>
    /// <typeparam name="T">任意の型</typeparam>
    /// <param name="_">式</param>
    /// <param name="expressionString">式の文字列</param>
    /// <returns><paramref name="_"/>に指定された式を文字列として返します。</returns>
    public static string ToExpressionString<T>(this T _, [CallerArgumentExpression(nameof(_))] string? expressionString = null)
        => expressionString!;
}
