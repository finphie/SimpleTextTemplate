using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using static SimpleTextTemplate.TemplateException;

namespace SimpleTextTemplate.Helpers
{
    /// <summary>
    /// 例外をスローするためのヘルパーメソッドです。
    /// </summary>
    static class ThrowHelper
    {
        /// <summary>
        /// 新しい<see cref="TemplateException"/>例外をスローします。
        /// </summary>
        /// <param name="error">解析エラー</param>
        /// <param name="position">バイト位置</param>
        /// <exception cref="TemplateException">常にこの例外をスローします。</exception>
        [DebuggerHidden]
        [DoesNotReturn]
        public static void ThrowTemplateParserException(ParserError error, int position)
            => throw new TemplateException(error, position);
    }
}