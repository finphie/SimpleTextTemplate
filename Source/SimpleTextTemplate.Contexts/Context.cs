using Utf8Utility;

namespace SimpleTextTemplate.Contexts;

/// <summary>
/// コンテキスト作成クラスです。
/// </summary>
public static class Context
{
    /// <summary>
    /// コンテキストを作成します。
    /// </summary>
    /// <param name="symbols">識別子と値のペアリスト</param>
    /// <returns>コンテキスト</returns>
    public static IContext Create(IReadOnlyUtf8ArrayDictionary<object> symbols)
        => new DictionaryContext(symbols);
}
