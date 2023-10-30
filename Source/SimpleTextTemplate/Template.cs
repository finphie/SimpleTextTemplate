using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using SimpleTextTemplate.Extensions;

#if NET8_0_OR_GREATER
using System.Buffers;
using System.Diagnostics;
using System.Runtime.InteropServices;
#endif

namespace SimpleTextTemplate;

/// <summary>
/// テンプレートを解析・レンダリングする構造体です。
/// </summary>
[SuppressMessage("Performance", "CA1815:equals および operator equals を値型でオーバーライドします", Justification = "不要なため。")]
public readonly struct Template
{
    readonly (BlockType Type, byte[] Value)[] _blocks;

    Template((BlockType Type, byte[] Value)[] blocks) => _blocks = blocks;

    /// <summary>
    /// ブロック単位のバッファを取得します。
    /// </summary>
    /// <value>
    /// ブロック単位のバッファ
    /// </value>
    public ReadOnlySpan<(BlockType Type, byte[] Value)> Blocks => _blocks.AsSpan();

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
    [SuppressMessage("Style", "IDE0305:コレクションの初期化を簡略化します", Justification = "https://github.com/dotnet/roslyn/issues/69988")]
    public static bool TryParse(byte[] source, out Template template, out nuint consumed)
    {
#if NET8_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(source);
#endif
        var reader = new TemplateReader(source);
        var list = new List<(BlockType Type, byte[] Value)>();

        while (reader.TryRead(out var value) is var type && type != BlockType.End)
        {
            if (type == BlockType.None)
            {
                template = new([]);
                consumed = reader.Consumed;
                return false;
            }

            list.Add((type, value.ToArray()));
        }

        template = new(list.ToArray());
        consumed = reader.Consumed;
        return true;
    }

    /// <summary>
    /// テンプレート文字列を解析します。
    /// </summary>
    /// <param name="source">テンプレート文字列</param>
    /// <returns><see cref="Template"/>構造体のインスタンス</returns>
    /// <exception cref="ArgumentNullException">引数がnullの場合、この例外をスローします。</exception>
    /// <exception cref="TemplateException">テンプレートの解析に失敗した場合に、この例外をスローします。</exception>
    [SuppressMessage("Style", "IDE0305:コレクションの初期化を簡略化します", Justification = "https://github.com/dotnet/roslyn/issues/69988")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Template Parse(byte[] source)
    {
#if NET8_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(source);
#endif
        var reader = new TemplateReader(source);
        var list = new List<(BlockType Type, byte[] Value)>();

        while (reader.Read(out var value) is var type && type != BlockType.End)
        {
            list.Add((type, value.ToArray()));
        }

        return new(list.ToArray());
    }

#if NET8_0_OR_GREATER
    /// <summary>
    /// テンプレートをレンダリングして、<see cref="IBufferWriter{Byte}"/>に書き込みます。
    /// </summary>
    /// <param name="bufferWriter">ターゲットの<see cref="IBufferWriter{Byte}"/></param>
    /// <param name="context">コンテキスト</param>
    /// <exception cref="ArgumentNullException">引数がnullの場合、この例外をスローします。</exception>
    public void Render(IBufferWriter<byte> bufferWriter, IContext context)
    {
        ArgumentNullException.ThrowIfNull(bufferWriter);
        ArgumentNullException.ThrowIfNull(context);

        foreach (var (type, stringOrIdentifier) in Blocks)
        {
            var span = MemoryMarshal.CreateReadOnlySpan(ref MemoryMarshal.GetArrayDataReference(stringOrIdentifier), stringOrIdentifier.Length);

            switch (type)
            {
                case BlockType.Raw:
                    bufferWriter.Write(span);
                    break;
                case BlockType.Identifier:
                    context.TryGetValue(span, out var value);
                    bufferWriter.Write(value.AsSpan());
                    break;
                case BlockType.None:
                case BlockType.End:
                default:
                    throw new UnreachableException();
            }
        }
    }
#endif
}
