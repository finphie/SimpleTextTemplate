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
    /// 検索対象空間で指定されたバイトシーケンスが出現する位置を取得します。
    /// </summary>
    /// <param name="searchSpace">検索対象空間</param>
    /// <param name="length">検索対象空間の長さ</param>
    /// <param name="value">検索するバイトシーケンス</param>
    /// <returns>
    /// 指定されたバイトシーケンスが出現した位置を返します。
    /// 一致しなかった場合は-1を返します。
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int IndexOf(scoped ref readonly byte searchSpace, int length, ReadOnlySpan<byte> value)
    {
        var span = CreateReadOnlySpan(ref Unsafe.AsRef(in searchSpace), length);
        return span.IndexOf(value);
    }

    /// <summary>
    /// 検索対象空間で指定された値以外が出現する位置を取得します。
    /// </summary>
    /// <param name="searchSpace">検索対象空間</param>
    /// <param name="length">検索対象空間の長さ</param>
    /// <param name="value">値</param>
    /// <returns>
    /// 指定された値以外が出現した位置を返します。
    /// 一致しなかった場合は-1を返します。
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int IndexOfAnyExcept(scoped ref readonly byte searchSpace, int length, byte value)
    {
#if NET8_0_OR_GREATER
        var span = CreateReadOnlySpan(ref Unsafe.AsRef(in searchSpace), length);
        return span.IndexOfAnyExcept(value);
#else
        nint index = 0;

        for (; index < length; index++)
        {
            if (Unsafe.AddByteOffset(ref Unsafe.AsRef(in searchSpace), index) != value)
            {
                return (int)index;
            }
        }

        return -1;
#endif
    }

    /// <summary>
    /// 検索対象空間を末尾から検索を行い、指定された値以外が出現する位置を取得します。
    /// </summary>
    /// <param name="searchSpace">検索対象空間</param>
    /// <param name="length">検索対象空間の長さ</param>
    /// <param name="value">値</param>
    /// <returns>
    /// 末尾から検索を行い、指定された値以外が出現した位置を返します。
    /// 一致しなかった場合は-1を返します。
    /// </returns>
    public static int LastIndexOfAnyExcept(scoped ref readonly byte searchSpace, int length, byte value)
    {
#if NET8_0_OR_GREATER
        var span = CreateReadOnlySpan(ref Unsafe.AsRef(in searchSpace), length);
        return span.LastIndexOfAnyExcept(value);
#else
        for (var index = (nint)length - 1; index >= 0; index--)
        {
            if (Unsafe.AddByteOffset(ref Unsafe.AsRef(in searchSpace), index) != value)
            {
                return (int)index;
            }
        }

        return -1;
#endif
    }

    /// <summary>
    /// 読み取り専用spanを作成します。
    /// </summary>
    /// <param name="value">データへの参照</param>
    /// <param name="length">データの長さ</param>
    /// <returns>指定された長さの読み取り専用spanを返します。</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe ReadOnlySpan<byte> CreateReadOnlySpan(scoped ref readonly byte value, int length)
    {
#if NET8_0_OR_GREATER
        var span = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef(in value), length);
#else
        var span = new ReadOnlySpan<byte>((byte*)Unsafe.AsPointer(ref Unsafe.AsRef(in value)), length);
#endif

        return span;
    }
}
