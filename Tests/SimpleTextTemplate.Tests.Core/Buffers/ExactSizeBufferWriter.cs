using System.Buffers;

namespace SimpleTextTemplate.Tests.Buffers;

public sealed class ExactSizeBufferWriter : IBufferWriter<byte>
{
    byte[] _array = [];
    int _index;

    public ReadOnlyMemory<byte> WrittenMemory
        => _array.AsMemory(0, _index);

    public void Advance(int count)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(count);
        _index += count;
    }

    public Span<byte> GetSpan(int sizeHint)
    {
        EnsureCapacity(sizeHint);
        return _array.AsSpan(_index);
    }

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
