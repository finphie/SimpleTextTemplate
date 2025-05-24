namespace SimpleTextTemplate.Generator.Specs;

/// <summary>
/// コンテキストのメンバーを表します。
/// </summary>
/// <param name="Name">メンバー名</param>
/// <param name="WriteType">書き込み種類</param>
/// <param name="Annotation">注釈</param>
sealed record ContextMember(
    string Name,
    TemplateWriterWriteType WriteType,
    MethodAnnotation Annotation);
