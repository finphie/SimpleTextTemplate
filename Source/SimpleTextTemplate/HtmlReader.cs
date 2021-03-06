using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SimpleTextTemplate.Extensions;
using SimpleTextTemplate.Helpers;
using static SimpleTextTemplate.HtmlParserException;

namespace SimpleTextTemplate
{
    /// <summary>
    /// UTF-8でエンコードされたHTMLを読み込みます。
    /// </summary>
    public ref struct HtmlReader
    {
        /// <summary>
        /// '{{'
        /// </summary>
        const ushort StartObject = 0x7b7b;

        /// <summary>
        /// '}}'
        /// </summary>
        const ushort EndObject = 0x7d7d;

        readonly ReadOnlySpan<byte> _buffer;

        int _position;

        /// <summary>
        /// <see cref="HtmlReader"/>構造体の新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="input">処理対象にするUTF-8のHTML文字列</param>
        public HtmlReader(ReadOnlySpan<byte> input)
        {
            _buffer = input;
            _position = 0;
        }

        /// <summary>
        /// HTML文字列を読み込みます。
        /// </summary>
        /// <param name="range">HTMLまたはオブジェクトの位置</param>
        /// <returns>ブロックのタイプ</returns>
        /// <exception cref="HtmlParserException">HTMLの解析に失敗した場合に、対象の例外をスローします。</exception>
        public BlockType Read(out Range range)
        {
            if (_position >= _buffer.Length)
            {
                range = default;
                return BlockType.None;
            }

            if (TryReadHtml(out range))
            {
                return BlockType.Html;
            }

            ReadObject(out range);
            return BlockType.Object;
        }

        /// <summary>
        /// HTML文字列を読み込みます。
        /// </summary>
        /// <param name="range">HTMLの位置</param>
        /// <returns>
        /// 現在位置の文字列が'{{'の場合は<see langword="true"/>、
        /// それ以外の場合は<see langword="false"/>。
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryReadHtml(out Range range)
        {
            if (_position >= _buffer.Length)
            {
                range = default;
                return false;
            }

            var startPosition = _position;
            ref var bufferStart = ref MemoryMarshal.GetReference(_buffer);

            while (_position + 1 < _buffer.Length)
            {
                // '{{'で始まらない場合
                if (!IsStartObjectInternal(ref bufferStart))
                {
                    _position++;
                    continue;
                }

                // 出力対象文字が1文字もない場合
                if (startPosition == _position)
                {
                    // TODO
                    range = default;
                    return false;
                }

                range = startPosition.._position;
                return true;
            }

            range = startPosition..++_position;
            return true;
        }

        /// <summary>
        /// オブジェクトを読み込みます。
        /// </summary>
        /// <param name="range">オブジェクトの位置</param>
        /// <exception cref="HtmlParserException">HTMLの解析に失敗した場合に、対象の例外をスローします。</exception>
        public void ReadObject(out Range range)
        {
            if (_position >= _buffer.Length)
            {
                ThrowHelper.ThrowHtmlParserException(ParserError.ExpectedStartObject, _position);
            }

            ref var bufferStart = ref MemoryMarshal.GetReference(_buffer);

            // '{{'で始まらない場合
            if (!IsStartObjectInternal(ref bufferStart))
            {
                // 2文字の内、最初の1文字が'{'の場合
                if (_position + 1 < _buffer.Length && Unsafe.Add(ref bufferStart, _position) == (byte)'{')
                {
                    _position++;
                }

                ThrowHelper.ThrowHtmlParserException(ParserError.ExpectedStartObject, _position);
            }

            _position += sizeof(ushort);

            if (_position >= _buffer.Length)
            {
                ThrowHelper.ThrowHtmlParserException(ParserError.ExpectedEndObject, --_position);
            }

            // '{'が3つ以上連続している場合
            if (Unsafe.Add(ref bufferStart, _position) == (byte)'{')
            {
                ThrowHelper.ThrowHtmlParserException(ParserError.ExpectedStartObject, _position);
            }

            // '{'に連続するスペースを削除
            SkipWhiteSpaceInternal(ref bufferStart);

            // スペースを削除後、bufferの末尾に到達した場合
            if (_position >= _buffer.Length)
            {
                ThrowHelper.ThrowHtmlParserException(ParserError.ExpectedEndObject, --_position);
            }

            var startPosition = _position;

            while (_position + 1 < _buffer.Length)
            {
                // '}}'で終わる場合
                if (IsEndObjectInternal(ref bufferStart))
                {
                    var endPosition = _position;

                    // '{{'と'}}'の間に1文字もない場合
                    if (startPosition == endPosition)
                    {
                        ThrowHelper.ThrowHtmlParserException(ParserError.InvalidObjectFormat, _position);
                    }

                    // 末尾のスペースを削除
                    for (; endPosition - 1 > startPosition; endPosition--)
                    {
                        if (!Unsafe.Add(ref bufferStart, endPosition - 1).IsWhiteSpace())
                        {
                            break;
                        }
                    }

                    _position += sizeof(ushort);

                    // '}'が3つ以上連続している場合
                    if (_position < _buffer.Length && Unsafe.Add(ref bufferStart, _position) == (byte)'}')
                    {
                        ThrowHelper.ThrowHtmlParserException(ParserError.ExpectedEndObject, _position);
                    }

                    range = startPosition..endPosition;
                    return;
                }

                _position++;
            }

            ThrowHelper.ThrowHtmlParserException(ParserError.ExpectedEndObject, _position);
            range = default;
        }

        /// <summary>
        /// 現在位置の文字列が'{{'であるかどうかを示す値を返します。
        /// </summary>
        /// <returns>
        /// 現在位置の文字列が'{{'の場合は<see langword="true"/>、
        /// それ以外の場合は<see langword="false"/>。
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsStartObject()
            => IsStartObjectInternal(ref MemoryMarshal.GetReference(_buffer));

        /// <summary>
        /// 現在位置の文字列が'}}'であるかどうかを示す値を返します。
        /// </summary>
        /// <returns>
        /// 現在位置の文字列が'}}'の場合は<see langword="true"/>、
        /// それ以外の場合は<see langword="false"/>。
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsEndObject()
            => IsEndObjectInternal(ref MemoryMarshal.GetReference(_buffer));

        /// <summary>
        /// 空白文字列をスキップします。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SkipWhiteSpace()
            => SkipWhiteSpaceInternal(ref MemoryMarshal.GetReference(_buffer));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool IsStartObjectInternal(ref byte bufferStart)
        {
            Debug.Assert(_buffer[0] == bufferStart, "Invalid position");
            return _position + 1 < _buffer.Length && Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref bufferStart, _position)) == StartObject;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool IsEndObjectInternal(ref byte bufferStart)
        {
            Debug.Assert(_buffer[0] == bufferStart, "Invalid position");
            return _position + 1 < _buffer.Length && Unsafe.ReadUnaligned<ushort>(ref Unsafe.Add(ref bufferStart, _position)) == EndObject;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void SkipWhiteSpaceInternal(ref byte bufferStart)
        {
            Debug.Assert(_buffer[0] == bufferStart, "Invalid position");

            while (_position < _buffer.Length)
            {
                if (!Unsafe.Add(ref bufferStart, _position).IsWhiteSpace())
                {
                    return;
                }

                _position++;
                continue;
            }
        }
    }
}