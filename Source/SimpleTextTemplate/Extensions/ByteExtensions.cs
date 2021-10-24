using System.Runtime.CompilerServices;

namespace SimpleTextTemplate.Extensions;

/// <summary>
/// <see cref="byte"/>関連のヘルパークラスです。
/// </summary>
static class ByteExtensions
{
    /// <summary>
    /// 空白であるかどうかを判別します。
    /// </summary>
    /// <param name="value">UTF-8文字</param>
    /// <returns>
    /// 空白の場合は<see langword="true"/>、
    /// それ以外の場合は<see langword="false"/>。
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsWhiteSpace(this byte value) => value == (byte)' ';
}
