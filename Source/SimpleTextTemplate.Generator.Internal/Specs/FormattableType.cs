namespace SimpleTextTemplate.Generator.Specs;

/// <summary>
/// 書式設定インターフェイスの種類
/// </summary>
[Flags]
enum FormattableType
{
    /// <summary>
    /// なし
    /// </summary>
    None = 0,

    /// <summary>
    /// <see cref="System.IFormattable"/>実装
    /// </summary>
    IFormattable = 1 << 0,

    /// <summary>
    /// ISpanFormattable実装
    /// </summary>
    ISpanFormattable = 1 << 1,

    /// <summary>
    /// IUtf8SpanFormattable実装
    /// </summary>
    IUtf8Formattable = 1 << 2
}
