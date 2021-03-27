using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace SimpleTextTemplate.Contexts.Helpers
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
    }
}