using System.Runtime.CompilerServices;

#if NET8_0_OR_GREATER
using System.Runtime.InteropServices;
#endif

namespace SimpleTextTemplate.Helpers;

/// <summary>
/// バイナリ処理関連のヘルパークラスです。
/// </summary>
static class BinaryHelper
{
    /// <summary>
    /// 検索対象空間で指定されたバイト値が最初に出現する位置を取得します。
    /// </summary>
    /// <param name="searchSpace">検索対象空間</param>
    /// <param name="length">検索対象空間の長さ</param>
    /// <param name="value">検索するバイト値</param>
    /// <returns>
    /// 指定されたバイト値が最初に出現した位置を返します。
    /// 一致しなかった場合は-1を返します。
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int IndexOf(scoped ref readonly byte searchSpace, int length, byte value)
    {
        var span = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef(in searchSpace), length);
        return span.IndexOf(value);
    }

    /// <summary>
    /// 検索対象空間で指定されたバイトシーケンスが最初に出現する位置を取得します。
    /// </summary>
    /// <param name="searchSpace">検索対象空間</param>
    /// <param name="length">検索対象空間の長さ</param>
    /// <param name="value">検索するバイトシーケンス</param>
    /// <returns>
    /// 指定されたバイトシーケンスが最初に出現した位置を返します。
    /// 一致しなかった場合は-1を返します。
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int IndexOf(scoped ref readonly byte searchSpace, int length, ReadOnlySpan<byte> value)
    {
        var span = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef(in searchSpace), length);
        return span.IndexOf(value);
    }

    /// <summary>
    /// 検索対象空間で指定された値以外が最初に出現する位置を取得します。
    /// </summary>
    /// <param name="searchSpace">検索対象空間</param>
    /// <param name="length">検索対象空間の長さ</param>
    /// <param name="value">値</param>
    /// <returns>
    /// 指定された値以外が最初に出現した位置を返します。
    /// 一致しなかった場合は-1を返します。
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int IndexOfAnyExcept(scoped ref readonly byte searchSpace, int length, byte value)
    {
        var span = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef(in searchSpace), length);
        return span.IndexOfAnyExcept(value);
    }

    /// <summary>
    /// 検索対象空間の末尾から検索を行い、指定された値以外が最初に出現する位置を取得します。
    /// </summary>
    /// <param name="searchSpace">検索対象空間</param>
    /// <param name="length">検索対象空間の長さ</param>
    /// <param name="value">値</param>
    /// <returns>
    /// 末尾から検索を行い、指定された値以外が最初に出現した位置を返します。
    /// 一致しなかった場合は-1を返します。
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int LastIndexOfAnyExcept(scoped ref readonly byte searchSpace, int length, byte value)
    {
        var span = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef(in searchSpace), length);
        return span.LastIndexOfAnyExcept(value);
    }
}
