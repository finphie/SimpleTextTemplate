using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SimpleTextTemplate.Extensions;
using SimpleTextTemplate.Helpers;
using static SimpleTextTemplate.TemplateException;

namespace SimpleTextTemplate;

/// <summary>
/// UTF-8でエンコードされたテンプレートを読み込みます。
/// </summary>
#if !IsGenerator
public
#endif
ref struct TemplateReader
{
    /// <summary>
    /// '{{'
    /// </summary>
    const ushort StartIdentifier = 0x7b7b;

    /// <summary>
    /// '}}'
    /// </summary>
    const ushort EndIdentifier = 0x7d7d;

    readonly ReadOnlySpan<byte> _buffer;

    int _position;

    /// <summary>
    /// <see cref="TemplateReader"/>構造体の新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="input">処理対象にするUTF-8のテンプレート文字列</param>
    public TemplateReader(ReadOnlySpan<byte> input)
    {
        _buffer = input;
        _position = 0;
    }

#if NET6_0_OR_GREATER
    /// <summary>
    /// '{{'
    /// </summary>
    static ReadOnlySpan<byte> StartIdentifierSpan => "{{"u8;
#endif

    /// <summary>
    /// テンプレート文字列を読み込みます。
    /// </summary>
    /// <param name="range">テンプレートまたはオブジェクトの位置</param>
    /// <returns>ブロックのタイプ</returns>
    /// <exception cref="TemplateException">テンプレートの解析に失敗した場合に、この例外をスローします。</exception>
    public BlockType Read(out TextRange range)
    {
        if (_position >= _buffer.Length)
        {
            range = default;
            return BlockType.None;
        }

        if (TryReadTemplate(out range))
        {
            return BlockType.Raw;
        }

        ReadIdentifier(out range);
        return BlockType.Identifier;
    }

    /// <summary>
    /// テンプレート文字列を読み込みます。
    /// </summary>
    /// <param name="range">テンプレートの位置</param>
    /// <returns>
    /// 現在位置の文字列が'{{'の場合は<see langword="true"/>、
    /// それ以外の場合は<see langword="false"/>。
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryReadTemplate(out TextRange range)
    {
        if (_position >= _buffer.Length)
        {
            goto NotFound;
        }

        var startPosition = _position;

#if NET6_0_OR_GREATER
        var buffer = _buffer[_position..];
        var index = buffer.IndexOf(StartIdentifierSpan);

        if (index == -1)
        {
            _position = _buffer.Length;
            goto Found;
        }

        if (index == 0)
        {
            goto NotFound;
        }

        _position += index;
        goto Found;
#else
        ref var bufferStart = ref MemoryMarshal.GetReference(_buffer);

        while (_position + 1 < _buffer.Length)
        {
            // '{{'で始まらない場合
            if (!IsStartIdentifierBlockInternal(ref bufferStart))
            {
                _position++;
                continue;
            }

            // 出力対象文字が1文字もない場合
            if (startPosition == _position)
            {
                goto NotFound;
            }

            goto Found;
        }

        _position++;
        goto Found;
#endif
    Found:
        range = new TextRange(startPosition, _position);
        return true;
    NotFound:
        range = default;
        return false;
    }

    /// <summary>
    /// 識別子を読み込みます。
    /// </summary>
    /// <param name="range">オブジェクトの位置</param>
    /// <exception cref="TemplateException">テンプレートの解析に失敗した場合に、この例外をスローします。</exception>
    public void ReadIdentifier(out TextRange range)
    {
        ref var bufferStart = ref MemoryMarshal.GetReference(_buffer);

        // '{{'で始まらない場合
        if (!IsStartIdentifierBlockInternal(ref bufferStart))
        {
            // 2文字の内、最初の1文字が'{'の場合
            if (_position + 1 < _buffer.Length && Unsafe.Add(ref bufferStart, (nint)(uint)_position) == (byte)'{')
            {
                _position++;
            }

            goto ExpectedStartToken;
        }

        _position += sizeof(ushort);

        // '{'に連続するスペースを削除
        SkipSpaceInternal(ref bufferStart);

        var startPosition = _position;

        while (_position + 1 < _buffer.Length)
        {
            // '}}'で終わらない場合
            if (!IsEndIdentifierBlockInternal(ref bufferStart))
            {
                _position++;
                continue;
            }

            var endPosition = _position;

            // '{{'と'}}'の間に1文字もない場合
            if (startPosition == endPosition)
            {
                goto InvalidIdentifierFormat;
            }

            // 末尾のスペースを削除
            for (; endPosition - 1 > startPosition; endPosition--)
            {
                if (!Unsafe.Add(ref bufferStart, (nint)(uint)(endPosition - 1)).IsSpace())
                {
                    break;
                }
            }

            _position += sizeof(ushort);

            range = new TextRange(startPosition, endPosition);
            return;
        }

        if (_position == _buffer.Length)
        {
            --_position;
        }

        goto ExpectedEndToken;

    InvalidIdentifierFormat:
        ThrowHelper.ThrowTemplateParserException(ParserError.InvalidIdentifierFormat, _position);

    ExpectedStartToken:
        ThrowHelper.ThrowTemplateParserException(ParserError.ExpectedStartToken, _position);

    ExpectedEndToken:
        ThrowHelper.ThrowTemplateParserException(ParserError.ExpectedEndToken, _position);

        // 直前のコードで例外を出すはずなので、ここには到達しない。
        Unsafe.SkipInit(out range);
    }

    /// <summary>
    /// 現在位置の文字列が'{{'であるかどうかを示す値を返します。
    /// </summary>
    /// <returns>
    /// 現在位置の文字列が'{{'の場合は<see langword="true"/>、
    /// それ以外の場合は<see langword="false"/>。
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool IsStartIdentifierBlock()
        => IsStartIdentifierBlockInternal(ref MemoryMarshal.GetReference(_buffer));

    /// <summary>
    /// 現在位置の文字列が'}}'であるかどうかを示す値を返します。
    /// </summary>
    /// <returns>
    /// 現在位置の文字列が'}}'の場合は<see langword="true"/>、
    /// それ以外の場合は<see langword="false"/>。
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool IsEndIdentifierBlock()
        => IsEndIdentifierBlockInternal(ref MemoryMarshal.GetReference(_buffer));

    /// <summary>
    /// 空白文字列をスキップします。
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SkipSpace()
        => SkipSpaceInternal(ref MemoryMarshal.GetReference(_buffer));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly bool IsStartIdentifierBlockInternal(ref byte bufferStart)
        => _position + 1 < _buffer.Length && Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref bufferStart, (nint)(uint)_position)) == StartIdentifier;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly bool IsEndIdentifierBlockInternal(ref byte bufferStart)
        => _position + 1 < _buffer.Length && Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref bufferStart, (nint)(uint)_position)) == EndIdentifier;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void SkipSpaceInternal(ref byte bufferStart)
    {
        while (_position < _buffer.Length)
        {
            if (!Unsafe.Add(ref bufferStart, (nint)(uint)_position).IsSpace())
            {
                return;
            }

            _position++;
        }
    }
}
