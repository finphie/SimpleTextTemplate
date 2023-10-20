using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using SimpleTextTemplate.Extensions;

#if NET8_0_OR_GREATER
using System.Buffers;
using System.Runtime.InteropServices;
using SimpleTextTemplate.Helpers;
#endif

namespace SimpleTextTemplate;

/// <summary>
/// テンプレートを解析・レンダリングする構造体です。
/// </summary>
[SuppressMessage("Performance", "CA1815:equals および operator equals を値型でオーバーライドします", Justification = "不要なため。")]
public readonly struct Template
{
    readonly byte[] _source;
    readonly List<(BlockType Type, TextRange Range)> _blocks;

    Template(byte[] source)
    {
        _source = source;
        _blocks = new(16);
    }

    /// <summary>
    /// ブロック単位のバッファを取得します。
    /// </summary>
    /// <value>
    /// ブロック単位のバッファ
    /// </value>
    internal ReadOnlySpan<(BlockType Type, TextRange Range)> Blocks => _blocks.AsSpan();

    /// <summary>
    /// テンプレート文字列を解析します。
    /// </summary>
    /// <param name="source">テンプレート文字列</param>
    /// <param name="template"><see cref="Template"/>構造体のインスタンス</param>
    /// <param name="consumed">読み取ったバイト数</param>
    /// <returns>
    /// 解析に成功した場合は<see langword="true"/>を返します。
    /// それ以外の場合は<see langword="false"/>を返します。
    /// </returns>
    /// <exception cref="ArgumentNullException">引数がnullの場合、この例外をスローします。</exception>
    public static bool TryParse(byte[] source, out Template template, out nuint consumed)
    {
#if NET8_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(source);
#endif

        template = new Template(source);
        var reader = new TemplateReader(source);
        var success = template.TryParseInternal(ref reader);

        consumed = reader.Consumed;
        return success;
    }

    /// <summary>
    /// テンプレート文字列を解析します。
    /// </summary>
    /// <param name="source">テンプレート文字列</param>
    /// <returns><see cref="Template"/>構造体のインスタンス</returns>
    /// <exception cref="ArgumentNullException">引数がnullの場合、この例外をスローします。</exception>
    /// <exception cref="TemplateException">テンプレートの解析に失敗した場合に、この例外をスローします。</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Template Parse(byte[] source)
    {
#if NET8_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(source);
#endif

        var template = new Template(source);
        template.ParseInternal();

        return template;
    }

#if NET8_0_OR_GREATER
    /// <summary>
    /// テンプレートをレンダリングして、<see cref="IBufferWriter{Byte}"/>に書き込みます。
    /// </summary>
    /// <param name="bufferWriter">ターゲットの<see cref="IBufferWriter{Byte}"/></param>
    /// <param name="context">コンテキスト</param>
    /// <exception cref="ArgumentNullException">引数がnullの場合、この例外をスローします。</exception>
    /// <exception cref="TemplateException">テンプレートの解析に失敗した場合に、この例外をスローします。</exception>
    public void Render(IBufferWriter<byte> bufferWriter, IContext context)
    {
        ArgumentNullException.ThrowIfNull(bufferWriter);
        ArgumentNullException.ThrowIfNull(context);

        var source = _source.AsSpan();
        ref var sourceStart = ref MemoryMarshal.GetReference(source);

        var blocks = Blocks;

        foreach (ref readonly var block in blocks)
        {
            var value = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AddByteOffset(ref sourceStart, (nint)(uint)block.Range.Start), block.Range.Length);

            switch (block.Type)
            {
                case BlockType.Raw:
                    bufferWriter.Write(value);
                    break;
                case BlockType.Identifier:
                    context.TryGetValue(value, out var x);
                    bufferWriter.Write(x.AsSpan());
                    break;
                case BlockType.None:
                default:
                    ThrowHelper.ThrowTemplateParserException((nuint)block.Range.Start);
                    break;
            }
        }
    }
#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool TryParseInternal(ref TemplateReader reader)
    {
        while (reader.TryRead(out var value) is var type)
        {
            if (type == BlockType.None)
            {
                return false;
            }

            _blocks.Add((type, value));
        }

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void ParseInternal()
    {
        var reader = new TemplateReader(_source);

        while (reader.Read(out var value) is var type)
        {
            _blocks.Add((type, value));
        }
    }
}
