namespace SimpleTextTemplate;

/// <summary>
/// テンプレート解析中に発生したエラーを表します。
/// </summary>
public sealed class TemplateException : Exception
{
    /// <summary>
    /// <see cref="TemplateException"/>クラスの新しいインスタンスを初期化します。
    /// </summary>
    public TemplateException()
    {
    }

    /// <summary>
    /// <see cref="TemplateException"/>クラスの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="position">エラーが発生した位置</param>
    public TemplateException(nuint position)
        : base($"parse error at position: {position}") => Position = position;

    /// <summary>
    /// <see cref="TemplateException"/>クラスの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="message">エラーを説明するメッセージ</param>
    public TemplateException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// <see cref="TemplateException"/>クラスの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="message">エラーを説明するメッセージ</param>
    /// <param name="innerException">内部例外</param>
    public TemplateException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// テンプレート文字列のバイト位置を取得します。
    /// </summary>
    /// <value>
    /// バイト位置
    /// </value>
    public nuint Position { get; }
}
