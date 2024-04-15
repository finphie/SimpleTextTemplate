using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;

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
    public ReadOnlySpan<Block> Blocks => _blocks.AsSpan();

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
    public static bool TryParse(byte[] source, out Template template, out nuint consumed)
    {
#if NET8_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(source);
#endif
        var reader = new TemplateReader(source);
        var list = new List<Block>();

        while (reader.TryRead(out var value) is var type && type != BlockType.End)
        {
            if (type == BlockType.None)
            {
                template = new([]);
                consumed = reader.Consumed;
                return false;
            }

            if (type != BlockType.Identifier)
            {
                list.Add((type, value.ToArray(), null, null));
                continue;
            }

            var identifierReader = new TemplateIdentifierReader(value);

            if (!identifierReader.TryRead(out var identifier, out var format, out var culture))
            {
                template = new([]);
                consumed = reader.Consumed;
                return false;
            }

            var cultureInfo = culture is null ? null : CultureInfo.GetCultureInfo(culture);
            list.Add((type, identifier.ToArray(), format, cultureInfo));
        }

        template = new([.. list]);
        consumed = reader.Consumed;
        return true;
    }

    /// <summary>
    /// テンプレート文字列を解析します。
    /// </summary>
    /// <param name="source">テンプレート文字列</param>
    /// <returns><see cref="Template"/>構造体のインスタンス</returns>
    /// <exception cref="ArgumentNullException">引数がnullです。</exception>
    /// <exception cref="TemplateException">テンプレートの解析に失敗しました。</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Template Parse(byte[] source)
    {
#if NET8_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(source);
#endif
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

            var cultureInfo = culture is null ? null : CultureInfo.GetCultureInfo(culture);
            list.Add((type, identifier.ToArray(), format, cultureInfo));
        }

        return new([.. list]);
    }
}
