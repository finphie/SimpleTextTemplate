#if NETSTANDARD2_0
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace SimpleTextTemplate.Contexts.Helpers;

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
}
#endif
