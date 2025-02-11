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
    /// 文字列
    /// </summary>
    WriteString,

    /// <summary>
    /// 列挙型の値に対応する名前
    /// </summary>
    WriteEnum,

    /// <summary>
    /// 任意型
    /// </summary>
    WriteValue,

    /// <summary>
    /// バッファー拡張
    /// </summary>
    Grow
}
