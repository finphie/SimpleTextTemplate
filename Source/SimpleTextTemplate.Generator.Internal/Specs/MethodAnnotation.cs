namespace SimpleTextTemplate.Generator.Specs;

/// <summary>
/// メソッドの注釈
/// </summary>
[Flags]
enum MethodAnnotation
{
    /// <summary>
    /// なし
    /// </summary>
    None = 0,

    /// <summary>
    /// 静的変数指定
    /// </summary>
    Static = 1 << 0,

    /// <summary>
    /// バッファー事前拡張なしのメソッドを使用
    /// </summary>
    Dangerous = 1 << 1
}
