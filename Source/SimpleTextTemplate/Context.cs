namespace SimpleTextTemplate;

/// <summary>
/// コンテキスト作成クラスです。
/// </summary>
public static class Context
{
    /// <summary>
    /// UTF-8エンコーディングの <see cref="byte"/>配列をキーとする<see cref="Dictionary{TKey, TValue}"/>を作成します。
    /// </summary>
    /// <returns>UTF-8エンコーディングの<see cref="byte"/>配列をキーとする<see cref="Dictionary{TKey, TValue}"/>の新しいインスタンス。</returns>
    public static Dictionary<byte[], object> Create()
        => [with(Utf8StringEqualityComparer.Default)];
}
