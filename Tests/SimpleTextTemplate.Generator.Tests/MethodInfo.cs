namespace SimpleTextTemplate.Generator.Tests;

/// <summary>
/// メソッド情報を表すクラスです。
/// </summary>
/// <param name="Name">メソッド名</param>
/// <param name="Text">テンプレート文字列</param>
/// <param name="Format">書式指定</param>
/// <param name="Provider">カルチャー設定</param>
sealed record MethodInfo(string Name, string Text, string? Format, string? Provider);
