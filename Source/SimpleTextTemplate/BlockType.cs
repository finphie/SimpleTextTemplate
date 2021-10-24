namespace SimpleTextTemplate;

/// <summary>
/// ブロックのタイプを表します。
/// </summary>
#if !IsGenerator
public
#endif
enum BlockType
{
    /// <summary>
    /// 空のブロック。
    /// </summary>
    None,

    /// <summary>
    /// そのまま出力するブロック
    /// </summary>
    Raw,

    /// <summary>
    /// 識別子
    /// </summary>
    Identifier
}
