using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SimpleTextTemplate.Helpers;

#if NET8_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

namespace SimpleTextTemplate;

/// <summary>
/// UTF-8でエンコードされたテンプレートを読み込みます。
/// </summary>
public ref struct TemplateReader
{
#if NET8_0_OR_GREATER
    readonly ref readonly byte _start;
    ref byte _buffer;

    [SuppressMessage("Style", "IDE0032:自動プロパティを使用する", Justification = "誤検知")]
    int _length;
#else
    readonly ReadOnlySpan<byte> _start;
    ReadOnlySpan<byte> _buffer;
#endif

    /// <summary>
    /// <see cref="TemplateReader"/>構造体の新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="input">処理対象にするUTF-8のテンプレート文字列</param>
    public TemplateReader(ReadOnlySpan<byte> input)
    {
#if NET8_0_OR_GREATER
        _start = ref MemoryMarshal.GetReference(input);
        _buffer = ref MemoryMarshal.GetReference(input);
        _length = input.Length;
#else
        _start = input;
        _buffer = input;
#endif
    }

    /// <summary>
    /// 読み取ったバイト数
    /// </summary>
    public readonly nuint Consumed
#if NET8_0_OR_GREATER
        => (nuint)Unsafe.ByteOffset(ref Unsafe.AsRef(in _start), ref _buffer);
#else
        => (uint)Unsafe.ByteOffset(ref MemoryMarshal.GetReference(_start), ref MemoryMarshal.GetReference(_buffer));
#endif

    /// <summary>
    /// {{
    /// </summary>
    static ReadOnlySpan<byte> StartIdentifier => "{{"u8;

    /// <summary>
    /// }}
    /// </summary>
    static ReadOnlySpan<byte> EndIdentifier => "}}"u8;

    readonly ref byte Buffer =>
#if NET8_0_OR_GREATER
        ref _buffer;
#else
        ref MemoryMarshal.GetReference(_buffer);
#endif

#if NET8_0_OR_GREATER
    [SuppressMessage("Style", "IDE0032:自動プロパティを使用する", Justification = "誤検知")]
#endif
    readonly int Length =>
#if NET8_0_OR_GREATER
        _length;
#else
        _buffer.Length;
#endif

    /// <summary>
    /// 文字列または識別子を読み込みます。
    /// </summary>
    /// <param name="value">文字列または識別子。文字列や識別子ではない場合、<see cref="BlockType.None"/>を返す。</param>
    /// <returns>ブロックのタイプ</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BlockType TryRead(out ReadOnlySpan<byte> value)
    {
        if (Length <= 0)
        {
            value = default;
            return BlockType.End;
        }

        if (TryReadString(out value))
        {
            return BlockType.Raw;
        }

        if (TryReadIdentifier(out value))
        {
            return BlockType.Identifier;
        }

        value = default;
        return BlockType.None;
    }

    /// <summary>
    /// 文字列または識別子を読み込みます。
    /// </summary>
    /// <param name="value">文字列または識別子</param>
    /// <returns>ブロックのタイプ</returns>
    /// <exception cref="TemplateException">テンプレートの解析に失敗した場合に、この例外をスローします。</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public BlockType Read(out ReadOnlySpan<byte> value)
    {
        var result = TryRead(out value);

        if (result == BlockType.None)
        {
            ThrowHelper.ThrowTemplateParserException(Consumed);
        }

        return result;
    }

    /// <summary>
    /// 文字列を読み込みます。
    /// </summary>
    /// <param name="value">文字列</param>
    /// <returns>
    /// 現在位置の文字列が'{{'の場合は<see langword="true"/>、
    /// それ以外の場合は<see langword="false"/>。
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryReadString(out ReadOnlySpan<byte> value)
    {
        if (Length == 0)
        {
            value = default;
            return false;
        }

        // "{{"がある位置までは文字列となる。
        var index = BinaryHelper.IndexOf(ref Buffer, Length, StartIdentifier);

        if (index == 0)
        {
            value = default;
            return false;
        }

        if (index == -1)
        {
            index = Length;
        }

        value = BinaryHelper.CreateReadOnlySpan(ref Buffer, index);
        Advance(index);

        return true;
    }

    /// <summary>
    /// 識別子を読み込みます。
    /// </summary>
    /// <param name="value">識別子</param>
    /// <returns>
    /// 識別子を取得できた場合は<see langword="true"/>、
    /// それ以外の場合は<see langword="false"/>。
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryReadIdentifier(out ReadOnlySpan<byte> value)
    {
        // "{{"で始まらない場合
        if (Length < StartIdentifier.Length || Unsafe.ReadUnaligned<ushort>(ref Buffer) != MemoryMarshal.Read<ushort>(StartIdentifier))
        {
            value = default;
            return false;
        }

        Advance(StartIdentifier.Length);

        // "{{"に連続する空白を削除
        SkipSpace();

        // "}}"がある位置までは識別子となる。
        var index = BinaryHelper.IndexOf(ref Buffer, Length, EndIdentifier);

        // "{{"と"}}"の間に1文字もないか、"}}"が見つからない場合
        if (index <= 0)
        {
            value = default;
            return false;
        }

        ref var buffer = ref Buffer;
        Advance(index);

        // 識別子と"}}"の間にある連続する空白の位置を取得
        var endIndex = BinaryHelper.LastIndexOfAnyExcept(ref buffer, index, (byte)' ');

        // 識別子と"}}"の間に空白がない場合は、"{{"と"}}"の間の文字を識別子名とする。
        if (endIndex == -1)
        {
            endIndex = index;
        }

        value = BinaryHelper.CreateReadOnlySpan(ref Unsafe.SubtractByteOffset(ref Buffer, (nint)(uint)index), endIndex + 1);

        Advance(EndIdentifier.Length);
        return true;
    }

    /// <summary>
    /// 指定されたバイト数読み取り完了したことを通知します。
    /// </summary>
    /// <param name="count">進めるバイト数</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void Advance(int count)
    {
        Debug.Assert(count > 0, "バイト数は0より大きい数値である必要があります。");

#if NET8_0_OR_GREATER
        _buffer = ref Unsafe.AddByteOffset(ref _buffer, (nint)(uint)count);
        _length -= count;
#else
        _buffer = BinaryHelper.CreateReadOnlySpan(ref Unsafe.AddByteOffset(ref MemoryMarshal.GetReference(_buffer), (nint)(uint)count), _buffer.Length - count);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void SkipSpace()
    {
        var count = BinaryHelper.IndexOfAnyExcept(ref Buffer, Length, (byte)' ');

        // 末尾まで空白の場合は、末尾までスキップ対象とする。
        if (count == -1)
        {
            count = Length;
        }

        // 先頭が空白または既に末尾まで読み取り済みの場合
        if (count == 0)
        {
            return;
        }

        Advance(count);
    }
}
