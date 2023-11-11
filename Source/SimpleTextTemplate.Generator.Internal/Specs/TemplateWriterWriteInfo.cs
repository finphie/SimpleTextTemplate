namespace SimpleTextTemplate.Generator.Specs;

/// <summary>
/// 使用するWriteメソッドの種類を表します。
/// </summary>
/// <param name="WriteType">書き込み種類</param>
/// <param name="Value"><see cref="TemplateWriterWriteType.WriteConstantLiteral"/>の場合は定数文字列。その他はフィールド名または変数名。</param>
/// <param name="Format">カスタム文字列</param>
sealed record TemplateWriterWriteInfo(TemplateWriterWriteType WriteType, string Value, string? Format = null);
