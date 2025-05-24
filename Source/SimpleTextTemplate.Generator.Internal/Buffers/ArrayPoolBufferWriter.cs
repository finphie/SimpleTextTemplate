using System.Buffers;

namespace SimpleTextTemplate.Generator.Buffers;

/// <summary>
/// データを書き込むことができるクラスです。
/// </summary>
/// <typeparam name="T">書き込むデータの型</typeparam>
sealed class ArrayPoolBufferWriter<T> : IBufferWriter<T>, IDisposable
{
    const int DefaultInitialBufferSize = 1024;

    readonly ArrayPool<T> _pool;
    T[] _array;
    int _index;

    /// <summary>
    /// <see cref="ArrayPoolBufferWriter{T}"/>クラスの新しいインスタンスを初期化します。
    /// </summary>
    public ArrayPoolBufferWriter()
    {
        _pool = ArrayPool<T>.Shared;
        _array = _pool.Rent(DefaultInitialBufferSize);
        _index = 0;
    }

    /// <summary>
    /// 書き込み済みのバッファを取得します。
    /// </summary>
    public ReadOnlySpan<T> WrittenSpan
        => _array.AsSpan(0, _index);

    /// <inheritdoc/>
    public void Advance(int count)
    {
        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }

        _index += count;
    }

    /// <inheritdoc/>
    public Memory<T> GetMemory(int sizeHint = 0)
    {
        EnsureCapacity(sizeHint);
        return _array.AsMemory(_index);
    }

    /// <inheritdoc/>
    public Span<T> GetSpan(int sizeHint = 0)
    {
        EnsureCapacity(sizeHint);
        return _array.AsSpan(_index);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _pool.Return(_array);
        _array = null!;
    }

    void EnsureCapacity(int sizeHint)
    {
        if (sizeHint < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(sizeHint));
        }

        if (sizeHint == 0)
        {
            sizeHint = 1;
        }

        if (sizeHint <= _array.Length - _index)
        {
            return;
        }

        var newSize = _index + sizeHint;
        var newArray = _pool.Rent(newSize);

        var copyLength = Math.Min(_array.Length, newSize);
        _array.AsSpan(0, copyLength).CopyTo(newArray);

        _pool.Return(_array);
        _array = newArray;
    }
}
