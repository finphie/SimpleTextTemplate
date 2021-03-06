#if NET5_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

using System.Runtime.CompilerServices;
using static SimpleTextTemplate.HtmlParserException;

namespace SimpleTextTemplate.Helpers
{
    /// <summary>
    /// 例外をスローするためのヘルパーメソッドです。
    /// </summary>
    static class ThrowHelper
    {
        /// <summary>
        /// 新しい<see cref="HtmlParserException"/>例外をスローします。
        /// </summary>
        /// <param name="error">解析エラー</param>
        /// <param name="position">バイト位置</param>
        /// <exception cref="HtmlParserException">常にこの例外をスローします。</exception>
#if NET5_0_OR_GREATER
        [DoesNotReturn]
#endif
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowHtmlParserException(ParserError error, int position)
            => throw new HtmlParserException(error, position);
    }
}