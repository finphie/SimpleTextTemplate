using System.Globalization;

namespace SimpleTextTemplate.Generator;

/// <summary>
/// ソースジェネレーターで出力するソースを生成するクラスです。
/// </summary>
partial class CodeTemplate
{
    readonly byte[] _source;

    /// <summary>
    /// <see cref="CodeTemplate"/>クラスの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="filePath">ファイルパス</param>
    public CodeTemplate(string filePath)
    {
        FilePath = filePath;
        _source = File.ReadAllBytes(FilePath);
    }

    /// <summary>
    /// ファイルパスを取得します。
    /// </summary>
    /// <value>
    /// ファイルパス
    /// </value>
    internal string FilePath { get; }

    /// <summary>
    /// ファイル名を取得します。
    /// </summary>
    /// <value>
    /// ファイル名
    /// </value>
    internal string FileName => Path.GetFileNameWithoutExtension(FilePath);

    ReadOnlySpan<(BlockType Type, TextRange Range)> GetBlocks()
    {
        var source = Template.Parse(_source);
        return source.Blocks;
    }

    string ToHexString(TextRange range)
        => string.Join(", ", _source.Skip(range.Start).Take(range.Length).Select(x => "0x" + x.ToString("x2", CultureInfo.InvariantCulture)));
}
