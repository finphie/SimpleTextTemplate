using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using SimpleTextTemplate.Helpers;

namespace SimpleTextTemplate.Contexts
{
    /// <summary>
    /// Dictionary型コンテキスト
    /// </summary>
    sealed class DictionaryContext : IContext
    {
        readonly IReadOnlyDictionary<string, byte[]> _symbols;

        /// <summary>
        /// <see cref="DictionaryContext"/>クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="symbols">識別子とUTF-8文字列のペアリスト</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public DictionaryContext(IReadOnlyDictionary<string, byte[]> symbols)
        {
            if (symbols is null)
            {
                ThrowHelper.ThrowArgumentNullException(nameof(symbols));
            }

            _symbols = symbols;
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetValue(string key, [NotNullWhen(true)] out byte[]? value)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                ThrowHelper.ThrowArgumentNullOrWhitespaceException(nameof(key));
            }

            return _symbols.TryGetValue(key, out value);
        }
    }
}
