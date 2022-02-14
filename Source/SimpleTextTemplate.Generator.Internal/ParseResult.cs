namespace SimpleTextTemplate.Generator;

/// <summary>
/// 解析結果
/// </summary>
public enum ParseResult
{
    /// <summary>
    /// なし
    /// </summary>
    None,

    /// <summary>
    /// 成功
    /// </summary>
    Success,

    /// <summary>
    /// 無効な識別子
    /// </summary>
    InvalidIdentifier
}
