using System.Buffers;

namespace SimpleTextTemplate.Generator.Execute.Tests.Buffers;

/// <summary>
/// 厳密なサイズのバッファーを確保して、データを書き込むことができるクラスです。
/// </summary>
sealed class ExactSizeBufferWriter : IBufferWriter<byte>
{
    byte[] _array = [];
    int _index;

    /// <summary>
    /// 書き込み済みのバッファを取得します。
    /// </summary>
    public ReadOnlySpan<byte> WrittenSpan
        => _array.AsSpan(0, _index);

    /// <inheritdoc/>
    public void Advance(int count)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(count);
        _index += count;
    }

    /// <inheritdoc/>
    public Span<byte> GetSpan(int sizeHint)
    {
        EnsureCapacity(sizeHint);
        return _array.AsSpan(_index);
    }

    /// <inheritdoc/>
    Memory<byte> IBufferWriter<byte>.GetMemory(int sizeHint)
        => throw new NotSupportedException();

    void EnsureCapacity(int sizeHint)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(sizeHint);

        if (sizeHint == 0)
        {
            return;
        }

        if (sizeHint <= _array.Length - _index)
        {
            return;
        }

        var newSize = _index + sizeHint;
        var newArray = new byte[newSize];

        var copyLength = Math.Min(_array.Length, newSize);
        _array.AsSpan(0, copyLength).CopyTo(newArray);

        _array = newArray;
    }
}
