namespace SimpleTextTemplate;

/// <summary>
/// 範囲を表す構造体です。
/// </summary>
/// <param name="Start">開始位置</param>
/// <param name="End">終了位置</param>
public readonly record struct TextRange(int Start, int End)
{
    /// <summary>
    /// 長さを取得します。
    /// </summary>
    /// <value>
    /// <see cref="End"/>-<see cref="Start"/>
    /// </value>
    public int Length => End - Start;
}
