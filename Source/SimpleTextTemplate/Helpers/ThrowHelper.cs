using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using static SimpleTextTemplate.SimpleTextTemplateException;

namespace SimpleTextTemplate.Helpers
{
    /// <summary>
    /// 例外をスローするためのヘルパーメソッドです。
    /// </summary>
    static class ThrowHelper
    {
        /// <summary>
        /// 新しい<see cref="ArgumentNullException"/>例外をスローします。
        /// </summary>
        /// <param name="paramName">引数名</param>
        [DebuggerHidden]
        [DoesNotReturn]
        public static void ThrowArgumentNullException(string paramName)
            => throw new ArgumentNullException(paramName);

        /// <summary>
        /// 新しい<see cref="ArgumentException"/>例外をスローします。
        /// </summary>
        /// <param name="paramName">引数名</param>
        [DebuggerHidden]
        [DoesNotReturn]
        public static void ThrowArgumentNullOrWhitespaceException(string paramName)
            => throw new ArgumentException($"引数'{paramName}'は、nullまたは空白のみの文字列にはできません。", paramName);

        /// <summary>
        /// 新しい<see cref="SimpleTextTemplateException"/>例外をスローします。
        /// </summary>
        /// <param name="error">解析エラー</param>
        /// <param name="position">バイト位置</param>
        /// <exception cref="SimpleTextTemplateException">常にこの例外をスローします。</exception>
        [DebuggerHidden]
        [DoesNotReturn]
        public static void ThrowTemplateParserException(ParserError error, int position)
            => throw new SimpleTextTemplateException(error, position);
    }
}