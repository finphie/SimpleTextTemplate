using System.Buffers;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace SimpleTextTemplate;

/// <summary>
/// 文字列などをバッファーに書き込む構造体です。
/// </summary>
/// <typeparam name="T">バッファーライターの型</typeparam>
public ref struct TemplateWriter<T>
    where T : notnull, IBufferWriter<byte>, allows ref struct
{
    readonly T _bufferWriter;

    int _bufferLength;

    ref byte _destination;
    int _destinationLength;

    /// <summary>
    /// 新しい<see cref="TemplateWriter{T}"/>構造体を初期化します。
    /// </summary>
    /// <param name="bufferWriter">バッファーライター</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TemplateWriter(T bufferWriter)
    {
        _bufferWriter = bufferWriter;

        var span = _bufferWriter.GetSpan();
        _destination = ref MemoryMarshal.GetReference(span);

        var length = span.Length;
        _bufferLength = length;
        _destinationLength = length;
    }

    readonly Span<byte> Destination => MemoryMarshal.CreateSpan(ref _destination, _destinationLength);

    readonly int WrittenCount
    {
        get
        {
            var writtenCount = _bufferLength - _destinationLength;
            Debug.Assert(writtenCount >= 0, "書き込みバイト数は0以上の数値である必要があります。");

            return writtenCount;
        }
    }

    /// <summary>
    /// 指定されたサイズ以上にバッファーサイズを拡張します。
    /// </summary>
    /// <param name="length">最小バッファーサイズ</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Grow(int length)
    {
        if (_destinationLength >= length)
        {
            return;
        }

        GrowCore(length);
    }

    /// <summary>
    /// 書き込み処理を反映します。
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void Flush()
    {
        var writtenCount = WrittenCount;
        _bufferWriter.Advance(writtenCount);
    }

    /// <summary>
    /// バッファーにUTF-8文字列定数を書き込みます。
    /// </summary>
    /// <param name="value">UTF-8文字列定数</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteConstantLiteral(scoped ReadOnlySpan<byte> value)
    {
        Grow(value.Length);
        DangerousWriteConstantLiteral(value);
    }

    /// <summary>
    /// バッファーにUTF-8文字列定数を書き込みます。バッファーサイズの事前拡張は行いません。
    /// </summary>
    /// <param name="value">UTF-8文字列定数</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DangerousWriteConstantLiteral(scoped ReadOnlySpan<byte> value)
    {
        Debug.Assert(value.Length <= _destinationLength, "バッファーのサイズが不足しています。");

        switch (value.Length)
        {
            case 1:
                _destination = value[0];
                break;
            case 2:
                Unsafe.WriteUnaligned(ref _destination, MemoryMarshal.Read<ushort>(value));
                break;
            case 3:
                Unsafe.AddByteOffset(ref _destination, 2) = value[2];
                Unsafe.WriteUnaligned(ref _destination, MemoryMarshal.Read<ushort>(value));
                break;
            case 4:
                Unsafe.WriteUnaligned(ref _destination, MemoryMarshal.Read<uint>(value));
                break;
            case 5:
                Unsafe.AddByteOffset(ref _destination, 4) = value[4];
                Unsafe.WriteUnaligned(ref _destination, MemoryMarshal.Read<uint>(value));
                break;
            case 6:
                Write6(ref _destination, value);
                break;
            case 7:
                Write7(ref _destination, value);
                break;
            case 8:
                Unsafe.WriteUnaligned(ref _destination, MemoryMarshal.Read<ulong>(value));
                break;
            default:
                Unsafe.CopyBlockUnaligned(ref _destination, ref MemoryMarshal.GetReference(value), (uint)value.Length);
                break;
        }

        Advance(value.Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void Write6(ref byte destination, ReadOnlySpan<byte> value)
        {
            ref var reference = ref MemoryMarshal.GetReference(value);
            Unsafe.WriteUnaligned(ref Unsafe.AddByteOffset(ref destination, 4), Unsafe.ReadUnaligned<ushort>(ref Unsafe.AddByteOffset(ref reference, 4)));
            Unsafe.WriteUnaligned(ref destination, Unsafe.ReadUnaligned<uint>(ref reference));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void Write7(ref byte destination, ReadOnlySpan<byte> value)
        {
            ref var reference = ref MemoryMarshal.GetReference(value);
            Unsafe.WriteUnaligned(ref Unsafe.AddByteOffset(ref destination, 3), Unsafe.ReadUnaligned<uint>(ref Unsafe.AddByteOffset(ref reference, 3)));
            Unsafe.WriteUnaligned(ref destination, Unsafe.ReadUnaligned<uint>(ref reference));
        }
    }

    /// <summary>
    /// バッファーにUTF-8文字列を書き込みます。
    /// </summary>
    /// <param name="value">UTF-8文字列</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteLiteral(scoped ReadOnlySpan<byte> value)
    {
        Grow(value.Length);
        DangerousWriteLiteral(value);
    }

    /// <summary>
    /// バッファーにUTF-8文字列を書き込みます。バッファーサイズの事前拡張は行いません。
    /// </summary>
    /// <param name="value">UTF-8文字列</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DangerousWriteLiteral(scoped ReadOnlySpan<byte> value)
    {
        Debug.Assert(value.Length <= _destinationLength, "バッファーのサイズが不足しています。");

        Unsafe.CopyBlockUnaligned(ref _destination, ref MemoryMarshal.GetReference(value), (uint)value.Length);
        Advance(value.Length);
    }

    /// <summary>
    /// バッファーに文字列を書き込みます。
    /// </summary>
    /// <param name="value">文字列</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteString(scoped ReadOnlySpan<char> value)
    {
        var maxCount = Encoding.UTF8.GetMaxByteCount(value.Length);
        Grow(maxCount);
        DangerousWriteString(value);
    }

    /// <summary>
    /// バッファーに文字列を書き込みます。バッファーサイズの事前拡張は行いません。
    /// </summary>
    /// <param name="value">文字列</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DangerousWriteString(scoped ReadOnlySpan<char> value)
    {
        Debug.Assert(Encoding.UTF8.GetMaxByteCount(value.Length) <= _destinationLength, "バッファーのサイズが不足しています。");

        var success = Encoding.UTF8.TryGetBytes(value, Destination, out var bytesWritten);
        Debug.Assert(success, "UTF-8への変換に失敗しました。");

        Advance(bytesWritten);
    }

    /// <summary>
    /// バッファーに列挙型の値に対応する名前を書き込みます。
    /// </summary>
    /// <typeparam name="TValue">列挙型</typeparam>
    /// <param name="value">列挙型の値</param>
    /// <param name="format">カスタム形式</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteEnum<TValue>(TValue value, ReadOnlySpan<char> format = default)
        where TValue : struct, Enum
    {
        Span<char> destination = stackalloc char[256];

        if (Enum.TryFormat(value, destination, out var charsWritten, format))
        {
            WriteString(MemoryMarshal.CreateReadOnlySpan(ref MemoryMarshal.GetReference(destination), charsWritten));
            return;
        }

        GrowAndWrite(ref this, value, destination.Length, format);

        [MethodImpl(MethodImplOptions.NoInlining)]
        static void GrowAndWrite(scoped ref TemplateWriter<T> writer, TValue value, int length, ReadOnlySpan<char> format)
        {
            while (true)
            {
                var newLength = length * 2;

                if ((uint)newLength > Array.MaxLength)
                {
                    newLength = length == Array.MaxLength
                        ? Array.MaxLength + 1
                        : Array.MaxLength;
                }

                var array = ArrayPool<char>.Shared.Rent(newLength);
                var destination = MemoryMarshal.CreateSpan(ref MemoryMarshal.GetArrayDataReference(array), array.Length);

                try
                {
                    if (Enum.TryFormat(value, array, out var charsWritten, format))
                    {
                        writer.WriteString(MemoryMarshal.CreateReadOnlySpan(ref MemoryMarshal.GetReference(destination), charsWritten));
                        return;
                    }
                }
                finally
                {
                    ArrayPool<char>.Shared.Return(array);
                }
            }
        }
    }

    /// <summary>
    /// バッファーに変数の値を書き込みます。
    /// </summary>
    /// <typeparam name="TValue">書き込む変数の型</typeparam>
    /// <param name="value">変数の値</param>
    /// <param name="format">カスタム形式</param>
    /// <param name="provider">カルチャー指定</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteValue<TValue>(TValue value, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        if (value is IUtf8SpanFormattable)
        {
            WriteUtf8SpanFormattable(value, format, provider);
            return;
        }

        if (value is ISpanFormattable)
        {
            WriteSpanFormattable(value, format, provider);
            return;
        }

        if (value is IFormattable formattableValue)
        {
            WriteString(formattableValue.ToString(format.ToString(), provider));
            return;
        }

        WriteString(value?.ToString());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void WriteUtf8SpanFormattable<TValue>(TValue value, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        Debug.Assert(value is IUtf8SpanFormattable, "IUtf8SpanFormattableではありません。");

        if (!((IUtf8SpanFormattable)value).TryFormat(Destination, out var bytesWritten, format, provider))
        {
            var newLength = _destinationLength * 2;

            if ((uint)newLength > Array.MaxLength)
            {
                newLength = _destinationLength == Array.MaxLength
                    ? Array.MaxLength + 1
                    : Array.MaxLength;
            }

            GrowCore(newLength);

            var success = ((IUtf8SpanFormattable)value).TryFormat(Destination, out bytesWritten, format, provider);
            Debug.Assert(success, "UTF-8への変換に失敗しました。");
        }

        Advance(bytesWritten);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void WriteSpanFormattable<TValue>(TValue value, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        Debug.Assert(value is ISpanFormattable, "ISpanFormattableではありません。");

        Span<char> destination = stackalloc char[256];

        if (((ISpanFormattable)value).TryFormat(destination, out var charsWritten, format, provider))
        {
            WriteString(MemoryMarshal.CreateReadOnlySpan(ref MemoryMarshal.GetReference(destination), charsWritten));
            return;
        }

        GrowAndWrite(ref this, value, destination.Length, format, provider);

        [MethodImpl(MethodImplOptions.NoInlining)]
        static void GrowAndWrite(scoped ref TemplateWriter<T> writer, TValue value, int length, ReadOnlySpan<char> format, IFormatProvider? provider)
        {
            Debug.Assert(value is ISpanFormattable, "ISpanFormattableではありません。");

            while (true)
            {
                var newLength = length * 2;

                if ((uint)newLength > Array.MaxLength)
                {
                    newLength = length == Array.MaxLength
                        ? Array.MaxLength + 1
                        : Array.MaxLength;
                }

                var array = ArrayPool<char>.Shared.Rent(newLength);
                var destination = MemoryMarshal.CreateSpan(ref MemoryMarshal.GetArrayDataReference(array), array.Length);

                try
                {
                    if (((ISpanFormattable)value).TryFormat(destination, out var charsWritten, format, provider))
                    {
                        writer.WriteString(MemoryMarshal.CreateReadOnlySpan(ref MemoryMarshal.GetReference(destination), charsWritten));
                        return;
                    }
                }
                finally
                {
                    ArrayPool<char>.Shared.Return(array);
                }
            }
        }
    }

    /// <summary>
    /// 指定されたバイト数書き込み完了したことを通知します。
    /// </summary>
    /// <param name="count">進めるバイト数</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void Advance(int count)
    {
        Debug.Assert(count >= 0, "バイト数は0以上の数値である必要があります。");

        _destination = ref Unsafe.AddByteOffset(ref _destination, (nint)(uint)count);
        _destinationLength -= count;

        Debug.Assert(_destinationLength >= 0, "出力先サイズは0以上の数値である必要があります。");
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    void GrowCore(int length)
    {
        var writtenCount = WrittenCount;

        if (writtenCount > 0)
        {
            _bufferWriter.Advance(writtenCount);
        }

        var span = _bufferWriter.GetSpan(length);
        _destination = ref MemoryMarshal.GetReference(span);

        var destinationLength = span.Length;
        _bufferLength = destinationLength;
        _destinationLength = destinationLength;
    }
}
