using System.Diagnostics;

namespace SimpleTextTemplate;

/// <summary>
/// テンプレートファイルと指定されたコンテキストを利用して、バッファに書き込むメソッドを作成することを示す属性です。
/// </summary>
/// <param name="path">パス</param>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
[Conditional("SIMPLETEXTTEMPLATE_KEEP_SOURCE_GENERATOR_ATTRIBUTES")]
public sealed class TemplateFileAttribute(string path) : Attribute
{
    /// <summary>
    /// ファイルパスを取得します。
    /// </summary>
    /// <value>
    /// ファイルパス
    /// </value>
    public string Path { get; } = path;
}
