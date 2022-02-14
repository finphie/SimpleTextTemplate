using System.Diagnostics;

namespace SimpleTextTemplate;

/// <summary>
/// テンプレートファイルと指定されたコンテキストを利用して、バッファに書き込むメソッドを作成することを示す属性です。
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
[Conditional("SIMPLETEXTTEMPLATE_KEEP_SOURCE_GENERATOR_ATTRIBUTES")]
public sealed class TemplateFileAttribute : Attribute
{
    /// <summary>
    /// <see cref="TemplateFileAttribute"/>クラスの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="path">パス</param>
    public TemplateFileAttribute(string path)
        => Path = path;

    /// <summary>
    /// ファイルパスを取得します。
    /// </summary>
    /// <value>
    /// ファイルパス
    /// </value>
    public string Path { get; }
}
