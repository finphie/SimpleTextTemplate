using System.Diagnostics.CodeAnalysis;

namespace SimpleTextTemplate;

/// <summary>
/// 範囲を表す構造体です。
/// </summary>
[SuppressMessage("Performance", "CA1815:equals および operator equals を値型でオーバーライドします", Justification = "不要なため。")]
#if !IsGenerator
public
#endif
readonly struct TextRange
{
    /// <summary>
    /// <see cref="TextRange"/>構造体の新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="start">開始位置</param>
    /// <param name="end">終了位置</param>
    public TextRange(int start, int end)
        => (Start, End) = (start, end);

    /// <summary>
    /// 開始位置を取得します。
    /// </summary>
    /// <value>
    /// 開始位置
    /// </value>
    public int Start { get; }

    /// <summary>
    /// 終了位置を取得します。
    /// </summary>
    /// <value>
    /// 終了位置
    /// </value>
    public int End { get; }

    /// <summary>
    /// 長さを取得します。
    /// </summary>
    /// <value>
    /// 長さ
    /// </value>
    public int Length => End - Start;
}
