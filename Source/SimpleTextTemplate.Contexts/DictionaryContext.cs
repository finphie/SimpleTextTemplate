using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Utf8Utility;

namespace SimpleTextTemplate.Contexts;

/// <summary>
/// Dictionary型コンテキスト
/// </summary>
sealed class DictionaryContext : IContext
{
    readonly IReadOnlyUtf8ArrayDictionary<object> _symbols;

    /// <summary>
    /// <see cref="DictionaryContext"/>クラスの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="symbols">識別子とUTF-8配列のペアリスト</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public DictionaryContext(IReadOnlyUtf8ArrayDictionary<object> symbols)
    {
        ArgumentNullException.ThrowIfNull(symbols);
        _symbols = symbols;
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetValue(Utf8Array key, [MaybeNullWhen(false)] out object value)
        => _symbols.TryGetValue(key, out value);

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetValue(ReadOnlySpan<byte> key, [MaybeNullWhen(false)] out object value)
        => _symbols.TryGetValue(key, out value);
}
