using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace SimpleTextTemplate.Helpers;

/// <summary>
/// 例外をスローするためのヘルパーメソッドです。
/// </summary>
static class ThrowHelper
{
    /// <summary>
    /// 新しい<see cref="TemplateException"/>例外をスローします。
    /// </summary>
    /// <param name="position">バイト位置</param>
    /// <exception cref="TemplateException">常にこの例外をスローします。</exception>
    [DebuggerHidden]
    [DoesNotReturn]
    public static void ThrowTemplateParserException(nuint position)
        => throw new TemplateException(position);

    /// <summary>
    /// 新しい<see cref="TemplateException"/>例外をスローします。
    /// </summary>
    /// <exception cref="TemplateException">常にこの例外をスローします。</exception>
    [DebuggerHidden]
    [DoesNotReturn]
    public static void ThrowInvalidIdentifierException()
        => throw new TemplateException("invalid identifier");
}
