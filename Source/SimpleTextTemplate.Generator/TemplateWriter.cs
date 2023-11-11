using System.Buffers;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace SimpleTextTemplate;

/// <summary>
/// テンプレート文字列のレンダリングを行います。
/// </summary>
/// <typeparam name="T">バッファーライターの型</typeparam>
public ref struct TemplateWriter<T>
    where T : notnull, IBufferWriter<byte>
{
    readonly ref T _bufferWriter;

    ref readonly byte _destinationStart;
    ref byte _destination;
    int _destinationLength;

    /// <summary>
    /// 新しい<see cref="TemplateWriter{T}"/>構造体を初期化します。
    /// </summary>
    /// <param name="bufferWriter">バッファーライター</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TemplateWriter(ref T bufferWriter)
    {
        _bufferWriter = ref bufferWriter;

        var span = _bufferWriter.GetSpan();
        _destinationStart = ref MemoryMarshal.GetReference(span);
        _destination = ref MemoryMarshal.GetReference(span);
        _destinationLength = span.Length;
    }

    readonly Span<byte> Destination => MemoryMarshal.CreateSpan(ref _destination, _destinationLength);

    /// <summary>
    /// バッファーに文字列を書き込みます。
    /// </summary>
    /// <param name="text">テンプレート文字列</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(string text) => throw new UnreachableException();

    /// <summary>
    /// バッファーにコンテキストを書き込みます。
    /// </summary>
    /// <typeparam name="TContext">コンテキストの型</typeparam>
    /// <param name="text">テンプレート文字列</param>
    /// <param name="context">コンテキスト</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write<TContext>(string text, in TContext context)
        where TContext : notnull
        => throw new UnreachableException();

    /// <summary>
    /// 書き込み処理を反映します。
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        var writtenCount = Unsafe.ByteOffset(ref Unsafe.AsRef(in _destinationStart), ref _destination);
        Debug.Assert(writtenCount >= 0, "書き込みバイト数は0以上の数値である必要があります。");

        _bufferWriter.Advance((int)writtenCount);
    }

    /// <summary>
    /// バッファーにUTF-8文字列を書き込みます。
    /// </summary>
    /// <param name="value">UTF-8文字列</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteLiteral(scoped ReadOnlySpan<byte> value)
    {
        Grow(value.Length);
        Unsafe.CopyBlockUnaligned(ref _destination, ref MemoryMarshal.GetReference(value), (uint)value.Length);
        Advance(value.Length);
    }

    /// <summary>
    /// バッファーに文字列を書き込みます。
    /// </summary>
    /// <param name="value">文字列</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteString(scoped ReadOnlySpan<char> value)
    {
        var maxCount = Encoding.UTF8.GetMaxByteCount(value.Length);
        Grow(maxCount);

        var success = Encoding.UTF8.TryGetBytes(value, Destination, out var bytesWritten);
        Debug.Assert(success, "UTF-8への変換に失敗しました。");

        Advance(bytesWritten);
    }

    /// <summary>
    /// バッファーに列挙型の値に対応する名前を書き込みます。
    /// </summary>
    /// <typeparam name="TValue">列挙型</typeparam>
    /// <param name="value">列挙型の値</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteEnum<TValue>(TValue value)
        where TValue : struct, Enum
    {
        Span<char> destination = stackalloc char[256];

        if (Enum.TryFormat(value, destination, out var charsWritten))
        {
            WriteString(MemoryMarshal.CreateReadOnlySpan(ref MemoryMarshal.GetReference(destination), charsWritten));
            return;
        }

        GrowAndWrite(ref this, value, destination.Length);

        [MethodImpl(MethodImplOptions.NoInlining)]
        static void GrowAndWrite(scoped ref TemplateWriter<T> writer, TValue value, int length)
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
                    if (Enum.TryFormat(value, array, out var charsWritten))
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
    [EditorBrowsable(EditorBrowsableState.Never)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteValue<TValue>(TValue value)
    {
        if (value is IUtf8SpanFormattable)
        {
            WriteUtf8SpanFormattable(value);
            return;
        }

        if (value is ISpanFormattable)
        {
            WriteSpanFormattable(value);
            return;
        }

        if (value is IFormattable formattableValue)
        {
            WriteString(formattableValue.ToString(null, CultureInfo.InvariantCulture));
            return;
        }

        WriteString(value?.ToString());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void WriteUtf8SpanFormattable<TValue>(TValue value)
    {
        Debug.Assert(value is IUtf8SpanFormattable, "IUtf8SpanFormattableではありません。");

        if (!((IUtf8SpanFormattable)value).TryFormat(Destination, out var bytesWritten, default, CultureInfo.InvariantCulture))
        {
            var newLength = _destinationLength * 2;

            if ((uint)newLength > Array.MaxLength)
            {
                newLength = _destinationLength == Array.MaxLength
                    ? Array.MaxLength + 1
                    : Array.MaxLength;
            }

            GrowCore(newLength);

            var success = ((IUtf8SpanFormattable)value).TryFormat(Destination, out bytesWritten, default, CultureInfo.InvariantCulture);
            Debug.Assert(success, "UTF-8への変換に失敗しました。");
        }

        Advance(bytesWritten);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void WriteSpanFormattable<TValue>(TValue value)
    {
        Debug.Assert(value is ISpanFormattable, "ISpanFormattableではありません。");

        Span<char> destination = stackalloc char[256];

        if (((ISpanFormattable)value).TryFormat(destination, out var charsWritten, default, CultureInfo.InvariantCulture))
        {
            WriteString(MemoryMarshal.CreateReadOnlySpan(ref MemoryMarshal.GetReference(destination), charsWritten));
            return;
        }

        GrowAndWrite(ref this, value, destination.Length);

        [MethodImpl(MethodImplOptions.NoInlining)]
        static void GrowAndWrite(scoped ref TemplateWriter<T> writer, TValue value, int length)
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
                    if (((ISpanFormattable)value).TryFormat(destination, out var charsWritten, default, CultureInfo.InvariantCulture))
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

        Debug.Assert(!Unsafe.IsAddressGreaterThan(ref Unsafe.AsRef(in _destinationStart), ref _destination), "出力先範囲の境界を超えました。");
        Debug.Assert(_destinationLength >= 0, "出力先サイズは0以上の数値である必要があります。");
    }

    /// <summary>
    /// 指定されたサイズ以上にバッファーサイズを拡張します。
    /// </summary>
    /// <param name="length">最小バッファーサイズ</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void Grow(int length)
    {
        if (_destinationLength >= length)
        {
            return;
        }

        GrowCore(length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void GrowCore(int length)
    {
        var writtenCount = Unsafe.ByteOffset(ref Unsafe.AsRef(in _destinationStart), ref _destination);
        Debug.Assert(writtenCount >= 0, "書き込みバイト数は0以上の数値である必要があります。");

        if (writtenCount > 0)
        {
            _bufferWriter.Advance((int)writtenCount);
        }

        var span = _bufferWriter.GetSpan(length);
        _destinationStart = ref MemoryMarshal.GetReference(span);
        _destination = ref MemoryMarshal.GetReference(span);
        _destinationLength = span.Length;
    }
}
