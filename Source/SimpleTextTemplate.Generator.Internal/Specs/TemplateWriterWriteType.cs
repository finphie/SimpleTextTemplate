namespace SimpleTextTemplate.Generator.Specs;

/// <summary>
/// TemplateWriterの書き込み種類
/// </summary>
enum TemplateWriterWriteType
{
    /// <summary>
    /// UTF-8定数文字列
    /// </summary>
    WriteConstantLiteral,

    /// <summary>
    /// UTF-8文字列
    /// </summary>
    WriteLiteral,

    /// <summary>
    /// 静的UTF-8文字列
    /// </summary>
    WriteStaticLiteral,

    /// <summary>
    /// 文字列
    /// </summary>
    WriteString,

    /// <summary>
    /// 静的文字列
    /// </summary>
    WriteStaticString,

    /// <summary>
    /// 任意型
    /// </summary>
    WriteValue,

    /// <summary>
    /// 静的任意型
    /// </summary>
    WriteStaticValue
}
