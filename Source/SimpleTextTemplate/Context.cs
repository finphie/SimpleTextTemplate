using System.Collections.Generic;
using SimpleTextTemplate.Abstractions;
using SimpleTextTemplate.Contexts;

namespace SimpleTextTemplate
{
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
        public static IContext Create(IReadOnlyDictionary<string, byte[]> symbols)
            => new DictionaryContext(symbols);
    }
}