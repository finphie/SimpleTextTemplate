using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using SimpleTextTemplate.Helpers;

namespace SimpleTextTemplate;

/// <summary>
/// UTF-8でエンコードされたテンプレート識別子を読み込みます。
/// </summary>
public ref struct TemplateIdentifierReader
{
    ref byte _buffer;

    [SuppressMessage("Style", "IDE0032:自動プロパティを使用する", Justification = "誤検知")]
    int _length;

    /// <summary>
    /// <see cref="TemplateIdentifierReader"/>構造体の新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="input">処理対象にするUTF-8のテンプレート文字列</param>
    public TemplateIdentifierReader(ReadOnlySpan<byte> input)
    {
        Debug.Assert(input.Length > 0, "識別子の長さは0より大きい値である必要があります。");
        Debug.Assert(input[0] != ' ', "バイト列先頭には空白以外の文字が必要です。");
        Debug.Assert(input[^1] != ' ', "バイト列末尾には空白以外の文字が必要です。");

        _buffer = ref MemoryMarshal.GetReference(input);
        _length = input.Length;
    }

    readonly ref byte Buffer =>
        ref _buffer;

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
        if (TryRead(out value, out format, out culture))
        {
            return;
        }

        ThrowHelper.ThrowInvalidIdentifierException();
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
        Debug.Assert(_length > 0, "バッファーの長さは0より大きい値である必要があります。");

        if (Buffer == (byte)':')
        {
            value = default;
            format = null;
            culture = null;
            return false;
        }

        var formatIndex = BinaryHelper.IndexOf(ref Buffer, _length, (byte)':');

        if (formatIndex <= 0)
        {
            value = MemoryMarshal.CreateReadOnlySpan(ref Buffer, _length);
            format = null;
            culture = null;
            return true;
        }

        value = MemoryMarshal.CreateReadOnlySpan(ref Buffer, formatIndex);
        Advance(formatIndex + 1);

        var cultureIndex = BinaryHelper.IndexOf(ref Buffer, _length, (byte)':');

        if (cultureIndex < 0)
        {
            format = _length != 0
                ? Encoding.UTF8.GetString((byte*)Unsafe.AsPointer(ref Buffer), _length)
                : null;
            culture = null;
            return true;
        }

        format = cultureIndex != 0 ? Encoding.UTF8.GetString((byte*)Unsafe.AsPointer(ref Buffer), cultureIndex) : null;
        Advance(cultureIndex + 1);

        culture = _length != 0
            ? Encoding.UTF8.GetString((byte*)Unsafe.AsPointer(ref Buffer), _length)
            : null;

        return true;
    }

    /// <summary>
    /// 指定されたバイト数読み取り完了したことを通知します。
    /// </summary>
    /// <param name="count">進めるバイト数</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void Advance(int count)
    {
        _buffer = ref Unsafe.AddByteOffset(ref _buffer, (nint)(uint)count);
        _length -= count;
    }
}
