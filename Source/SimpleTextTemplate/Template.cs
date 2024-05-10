using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;

#if NET8_0_OR_GREATER
using System.Runtime.InteropServices;
#endif

using Block = (SimpleTextTemplate.BlockType Type, byte[] Value, string? Format, System.IFormatProvider? Culture);

namespace SimpleTextTemplate;

/// <summary>
/// テンプレート構造を保存する構造体です。
/// </summary>
[SuppressMessage("Performance", "CA1815:equals および operator equals を値型でオーバーライドします", Justification = "不要なため。")]
public readonly struct Template
{
    readonly Block[] _blocks;

    Template(Block[] blocks) => _blocks = blocks;

    /// <summary>
    /// ブロック単位のバッファを取得します。
    /// </summary>
    /// <value>
    /// ブロック単位のバッファ
    /// </value>
    public ReadOnlySpan<Block> Blocks
#if NET8_0_OR_GREATER
        => MemoryMarshal.CreateReadOnlySpan(ref MemoryMarshal.GetArrayDataReference(_blocks), _blocks.Length);
#else
        => _blocks.AsSpan();
#endif

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
    /// <exception cref="ArgumentNullException">引数がnullです。</exception>
    public static bool TryParse(ReadOnlySpan<byte> source, out Template template, out nuint consumed)
    {
        var reader = new TemplateReader(source);
        var list = new List<Block>();

        while (reader.TryRead(out var value) is var type && type != BlockType.End)
        {
            if (type == BlockType.None)
            {
                goto Error;
            }

            if (type != BlockType.Identifier)
            {
                list.Add((type, value.ToArray(), null, null));
                continue;
            }

            var identifierReader = new TemplateIdentifierReader(value);

            if (!identifierReader.TryRead(out var identifier, out var format, out var culture))
            {
                goto Error;
            }

            CultureInfo? cultureInfo = null;

            try
            {
                if (culture is not null)
                {
#if NET8_0_OR_GREATER
                    cultureInfo = CultureInfo.GetCultureInfo(culture, true);
#else
                    cultureInfo = CultureInfo.GetCultureInfo(culture);
#endif
                }
            }
            catch (CultureNotFoundException)
            {
                goto Error;
            }

            list.Add((type, identifier.ToArray(), format, cultureInfo));
        }

        template = new([.. list]);
        consumed = reader.Consumed;
        return true;

    Error:
        template = new([]);
        consumed = reader.Consumed;
        return false;
    }

    /// <summary>
    /// テンプレート文字列を解析します。
    /// </summary>
    /// <param name="source">テンプレート文字列</param>
    /// <returns><see cref="Template"/>構造体のインスタンス</returns>
    /// <exception cref="ArgumentNullException">引数がnullです。</exception>
    /// <exception cref="CultureNotFoundException">カルチャーが見つかりません。</exception>
    /// <exception cref="TemplateException">テンプレートの解析に失敗しました。</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Template Parse(ReadOnlySpan<byte> source)
    {
        var reader = new TemplateReader(source);
        var list = new List<Block>();

        while (reader.Read(out var value) is var type && type != BlockType.End)
        {
            if (type != BlockType.Identifier)
            {
                list.Add((type, value.ToArray(), null, null));
                continue;
            }

            var identifierReader = new TemplateIdentifierReader(value);
            identifierReader.Read(out var identifier, out var format, out var culture);

            var cultureInfo = culture is null
                ? null
#if NET8_0_OR_GREATER
                : CultureInfo.GetCultureInfo(culture, true);
#else
                : CultureInfo.GetCultureInfo(culture);
#endif

            list.Add((type, identifier.ToArray(), format, cultureInfo));
        }

        return new([.. list]);
    }
}
