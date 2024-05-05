namespace SimpleTextTemplate.Generator.Tests;

/// <summary>
/// インターセプター情報を表すクラスです。
/// </summary>
/// <param name="Methods">メソッド情報のリスト</param>
sealed record InterceptInfo(IReadOnlyList<MethodInfo> Methods);
