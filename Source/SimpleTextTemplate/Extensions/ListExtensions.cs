using System.Runtime.CompilerServices;

namespace SimpleTextTemplate.Extensions;

/// <summary>
/// <see cref="List{T}"/>関連のヘルパークラスです。
/// </summary>
static class ListExtensions
{
    /// <summary>
    /// <see cref="List{T}"/>内部の配列を<see cref="ReadOnlySpan{T}"/>として取得します。
    /// </summary>
    /// <typeparam name="T">リストアイテムの型</typeparam>
    /// <param name="list"><see cref="ReadOnlySpan{T}"/>を作成するためのリスト</param>
    /// <returns><see cref="ReadOnlySpan{T}"/>インスタンス</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> AsSpan<T>(this List<T> list)
#if NET5_0_OR_GREATER
        => System.Runtime.InteropServices.CollectionsMarshal.AsSpan(list);
#else
#pragma warning disable
        => Unsafe.As<InternalList<T>>(list)!._items.AsSpan(0, list.Count);

        class InternalList<T>
        {
            internal T[] _items;
        }
#pragma warning restore
#endif
}
