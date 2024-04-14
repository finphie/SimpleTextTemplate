using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using SimpleTextTemplate.Helpers;

#if NET8_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

namespace SimpleTextTemplate;

/// <summary>
/// UTF-8でエンコードされたテンプレート識別子を読み込みます。
/// </summary>
public ref struct TemplateIdentifierReader
{
#if NET8_0_OR_GREATER
    ref byte _buffer;

    [SuppressMessage("Style", "IDE0032:自動プロパティを使用する", Justification = "誤検知")]
    int _length;
#else
    ReadOnlySpan<byte> _buffer;
#endif

    /// <summary>
    /// <see cref="TemplateIdentifierReader"/>構造体の新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="input">処理対象にするUTF-8のテンプレート文字列</param>
    public TemplateIdentifierReader(ReadOnlySpan<byte> input)
    {
        Debug.Assert(input.Length > 0, "識別子の長さは0より大きい値である必要があります。");
        Debug.Assert(input[0] != ' ', "バイト列先頭には空白以外の文字が必要です。");
        Debug.Assert(input[^1] != ' ', "バイト列末尾には空白以外の文字が必要です。");

#if NET8_0_OR_GREATER
        _buffer = ref MemoryMarshal.GetReference(input);
        _length = input.Length;
#else
        _buffer = input;
#endif
    }

    readonly ref byte Buffer =>
#if NET8_0_OR_GREATER
        ref _buffer;
#else
        ref MemoryMarshal.GetReference(_buffer);
#endif

    readonly int Length =>
#if NET8_0_OR_GREATER
        _length;
#else
        _buffer.Length;
#endif

    /// <summary>
    /// 識別子を読み込みます。
    /// </summary>
    /// <param name="value">識別子名</param>
    /// <param name="format">書式指定</param>
    /// <param name="culture">カルチャー指定</param>
    /// <exception cref="TemplateException">識別子名の取得に失敗しました。</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Read(out ReadOnlySpan<byte> value, out string? format, out string? culture)
    {
        if (!TryRead(out value, out format, out culture))
        {
            ThrowHelper.ThrowInvalidIdentifierException();
        }
    }

    /// <summary>
    /// 識別子を読み込みます。
    /// </summary>
    /// <param name="value">識別子名</param>
    /// <param name="format">書式指定</param>
    /// <param name="culture">カルチャー指定</param>
    /// <returns>
    /// 識別子を取得できた場合は<see langword="true"/>、
    /// それ以外の場合は<see langword="false"/>。
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe bool TryRead(out ReadOnlySpan<byte> value, out string? format, out string? culture)
    {
        Debug.Assert(Length > 0, "バッファーの長さは0より大きい値である必要があります。");

        if (Buffer == (byte)':')
        {
            value = default;
            format = null;
            culture = null;
            return false;
        }

        var formatIndex = BinaryHelper.IndexOf(ref Buffer, Length, (byte)':');

        if (formatIndex <= 0)
        {
            value = BinaryHelper.CreateReadOnlySpan(ref Buffer, Length);
            format = null;
            culture = null;
            return true;
        }

        value = BinaryHelper.CreateReadOnlySpan(ref Buffer, formatIndex);
        Advance(formatIndex + 1);

        var cultureIndex = BinaryHelper.IndexOf(ref Buffer, Length, (byte)':');

        if (cultureIndex < 0)
        {
            format = Encoding.UTF8.GetString((byte*)Unsafe.AsPointer(ref Buffer), Length);
            culture = null;
            return true;
        }

        format = Encoding.UTF8.GetString((byte*)Unsafe.AsPointer(ref Buffer), cultureIndex);
        Advance(cultureIndex + 1);

        culture = Encoding.UTF8.GetString((byte*)Unsafe.AsPointer(ref Buffer), Length);
        return true;
    }

    /// <summary>
    /// 指定されたバイト数読み取り完了したことを通知します。
    /// </summary>
    /// <param name="count">進めるバイト数</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void Advance(int count)
    {
#if NET8_0_OR_GREATER
        _buffer = ref Unsafe.AddByteOffset(ref _buffer, (nint)(uint)count);
        _length -= count;
#else
        _buffer = BinaryHelper.CreateReadOnlySpan(ref Unsafe.AddByteOffset(ref MemoryMarshal.GetReference(_buffer), (nint)(uint)count), _buffer.Length - count);
#endif
    }
}
