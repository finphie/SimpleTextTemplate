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
    /// <param name="symbols">識別子とUTF-8文字列のペアリスト</param>
    /// <returns>コンテキスト</returns>
    public static IContext Create(IReadOnlyUtf8StringDictionary<Utf8String> symbols)
        => new DictionaryContext(symbols);
}