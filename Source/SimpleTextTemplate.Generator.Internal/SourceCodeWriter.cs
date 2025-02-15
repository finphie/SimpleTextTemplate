using System.Buffers;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using CommunityToolkit.HighPerformance;
using CommunityToolkit.HighPerformance.Buffers;

namespace SimpleTextTemplate.Generator;

/// <summary>
/// ソースコードを書き込むクラスです。
/// </summary>
sealed class SourceCodeWriter : IDisposable
{
    const byte Space = (byte)' ';
    const int IndentSize = 4;

    readonly ArrayPoolBufferWriter<byte> _bufferWriter = new();
    readonly List<byte[]> _indentations = [[]];

    int _currentIndentationLevel;

    /// <summary>
    /// 書き込み済みのバッファを取得します。
    /// </summary>
    public ReadOnlySpan<byte> WrittenSpan => _bufferWriter.WrittenSpan;

    static ReadOnlySpan<byte> DefaultNewLine => "\r\n"u8;

    ReadOnlySpan<byte> CurrentIndentation => _indentations[_currentIndentationLevel];

    /// <inheritdoc/>
    public void Dispose()
        => _bufferWriter.Dispose();

    /// <summary>
    /// インデントを増やします。
    /// </summary>
    public void IncreaseIndent()
    {
        if (++_currentIndentationLevel != _indentations.Count)
        {
            return;
        }

        var newIndentation = new byte[_indentations[^1].Length + IndentSize];
        newIndentation.AsSpan().Fill(Space);

        _indentations.Add(newIndentation);
    }

    /// <summary>
    /// インデントを減らします。
    /// </summary>
    public void DecreaseIndent()
        => _currentIndentationLevel--;

    /// <summary>
    /// ブロックを書き込みます。
    /// </summary>
    /// <returns>開いているブロックを閉じるために、<see cref="IDisposable"/>を実装した型のインスタンスを返します。</returns>
    public SourceCodeWriterBlock WriteBlock()
    {
        WriteLine("{"u8);
        IncreaseIndent();

        return new(this);
    }

    /// <summary>
    /// 改行を書き込みます。
    /// </summary>
    public void WriteLine()
        => _bufferWriter.Write(DefaultNewLine);

    /// <summary>
    /// 文字列を書き込みます。
    /// </summary>
    /// <param name="text">文字列</param>
    public void WriteLine(ReadOnlySpan<byte> text)
    {
        if (_bufferWriter.WrittenSpan.EndsWith(DefaultNewLine))
        {
            _bufferWriter.Write(CurrentIndentation);
        }

        _bufferWriter.Write(text);
        WriteLine();
    }

    /// <summary>
    /// 文字列を書き込みます。
    /// </summary>
    /// <param name="handler">文字列補間ハンドラー</param>
    public void WriteLine([InterpolatedStringHandlerArgument("")] ref WriteInterpolatedStringHandler handler)
    {
        _ = handler;
        WriteLine();
    }

    /// <summary>
    /// 文字列を書き込みます。
    /// </summary>
    /// <param name="handler">文字列補間ハンドラー</param>
    public void Write([InterpolatedStringHandlerArgument("")] ref WriteInterpolatedStringHandler handler)
    {
        _ = this;
        _ = handler;
    }

    /// <summary>
    /// 文字列を書き込みます。
    /// </summary>
    /// <param name="text">文字列</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal unsafe void Write(ReadOnlySpan<char> text)
    {
        if (_bufferWriter.WrittenSpan.EndsWith(DefaultNewLine))
        {
            _bufferWriter.Write(CurrentIndentation);
        }

        var bufferLength = Encoding.UTF8.GetMaxByteCount(text.Length);
        var buffer = _bufferWriter.GetSpan(bufferLength);

        fixed (byte* bytes = buffer)
        {
            fixed (char* chars = text)
            {
                var writtenCount = Encoding.UTF8.GetBytes(chars, text.Length, bytes, bufferLength);
                _bufferWriter.Advance(writtenCount);
            }
        }
    }

    /// <summary>
    /// ソースコードのブロックを表します。
    /// </summary>
    /// <param name="writer">ソースコードを書き込むクラスのインスタンス</param>
    internal readonly ref struct SourceCodeWriterBlock(SourceCodeWriter writer) : IDisposable
    {
        readonly SourceCodeWriter _writer = writer;

        /// <inheritdoc/>
        public void Dispose()
        {
            _writer.DecreaseIndent();
            _writer.WriteLine("}"u8);
        }
    }

    /// <summary>
    /// <see cref="SourceCodeWriter"/>で使用する文字列補間ハンドラーです。
    /// </summary>
    [InterpolatedStringHandler]
    internal readonly ref struct WriteInterpolatedStringHandler
    {
        readonly SourceCodeWriter _writer;

        /// <summary>
        /// <see cref="WriteInterpolatedStringHandler"/>の新しいインスタンスを生成します。
        /// </summary>
        /// <param name="literalLength">定数文字の数</param>
        /// <param name="formattedCount">補間式の数</param>
        /// <param name="writer">ソースコードを書き込むクラスのインスタンス</param>
        public WriteInterpolatedStringHandler(int literalLength, int formattedCount, SourceCodeWriter writer)
        {
            _ = literalLength;
            _ = formattedCount;
            _writer = writer;
        }

        /// <summary>
        /// 文字列を追加します。
        /// </summary>
        /// <param name="value">文字列</param>
        public void AppendLiteral(string value)
            => _writer.Write(value.AsSpan());

        /// <summary>
        /// 文字列を追加します。
        /// </summary>
        /// <param name="value">文字列</param>
        public void AppendFormatted(string value)
            => AppendLiteral(value);

        /// <summary>
        /// 文字列を追加します。
        /// </summary>
        /// <param name="value">文字列</param>
        public void AppendFormatted(int value)
             => AppendLiteral(value.ToString(CultureInfo.InvariantCulture));
    }
}
