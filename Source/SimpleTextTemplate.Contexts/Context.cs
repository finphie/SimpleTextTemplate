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
    /// <param name="symbols">識別子とUTF-8配列のペアリスト</param>
    /// <returns>コンテキスト</returns>
    public static IContext Create(IReadOnlyUtf8ArrayDictionary<Utf8Array> symbols)
        => new DictionaryContext(symbols);
}