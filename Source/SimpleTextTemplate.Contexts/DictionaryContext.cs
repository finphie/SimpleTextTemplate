using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using SimpleTextTemplate.Abstractions;
using SimpleTextTemplate.Contexts.Helpers;
using Utf8Utility;

namespace SimpleTextTemplate.Contexts;

/// <summary>
/// Dictionary型コンテキスト
/// </summary>
sealed class DictionaryContext : IContext
{
    readonly IReadOnlyUtf8StringDictionary<Utf8String> _symbols;

    /// <summary>
    /// <see cref="DictionaryContext"/>クラスの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="symbols">識別子とUTF-8文字列のペアリスト</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public DictionaryContext(IReadOnlyUtf8StringDictionary<Utf8String> symbols)
    {
        if (symbols is null)
        {
            ThrowHelper.ThrowArgumentNullException(nameof(symbols));
        }

        _symbols = symbols;
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetValue(Utf8String key, [NotNullWhen(true)] out Utf8String value)
        => _symbols.TryGetValue(key, out value);

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetValue(ReadOnlySpan<byte> key, [NotNullWhen(true)] out Utf8String value)
        => _symbols.TryGetValue(key, out value);
}