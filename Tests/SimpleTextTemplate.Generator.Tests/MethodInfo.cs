namespace SimpleTextTemplate.Generator.Tests;

/// <summary>
/// メソッド情報を表すクラスです。
/// </summary>
/// <param name="Name">メソッド名</param>
/// <param name="Text">テンプレート文字列またはGrowメソッドで使用する長さ</param>
/// <param name="Format">書式指定</param>
/// <param name="Provider">カルチャー設定</param>
sealed record MethodInfo(string Name, IReadOnlyList<string> Text, string? Format, string? Provider);
