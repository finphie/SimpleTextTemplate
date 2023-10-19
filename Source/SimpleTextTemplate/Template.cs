using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using SimpleTextTemplate.Extensions;
#if !IsGenerator
using System.Buffers;
using SimpleTextTemplate.Helpers;
using static SimpleTextTemplate.TemplateException;
#endif

namespace SimpleTextTemplate;

/// <summary>
/// テンプレートを解析・レンダリングする構造体です。
/// </summary>
[SuppressMessage("Performance", "CA1815:equals および operator equals を値型でオーバーライドします", Justification = "不要なため。")]
#if NET8_0_OR_GREATER
public
#endif
readonly struct Template
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
    /// <returns><see cref="Template"/>構造体のインスタンス</returns>
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
    /// <exception cref="TemplateException">テンプレートの解析に失敗した場合に、この例外をスローします。</exception>
    public void Render(IBufferWriter<byte> bufferWriter, IContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var source = _source.AsSpan();
        var blocks = Blocks;

        foreach (ref readonly var block in blocks)
        {
            var value = source.Slice(block.Range.Start, block.Range.Length);

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

    void ParseInternal()
    {
        var reader = new TemplateReader(_source);

        while (reader.Read(out var value) is var type)
        {
            _blocks.Add((type, value));
        }
    }
}
