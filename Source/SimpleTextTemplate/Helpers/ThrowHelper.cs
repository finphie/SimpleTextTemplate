using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using static SimpleTextTemplate.SimpleTextTemplateException;

namespace SimpleTextTemplate.Helpers
{
    /// <summary>
    /// 例外をスローするためのヘルパーメソッドです。
    /// </summary>
    static class ThrowHelper
    {
        /// <summary>
        /// 新しい<see cref="SimpleTextTemplateException"/>例外をスローします。
        /// </summary>
        /// <param name="error">解析エラー</param>
        /// <param name="position">バイト位置</param>
        /// <exception cref="SimpleTextTemplateException">常にこの例外をスローします。</exception>
        [DoesNotReturn]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowTemplateParserException(ParserError error, int position)
            => throw new SimpleTextTemplateException(error, position);
    }
}