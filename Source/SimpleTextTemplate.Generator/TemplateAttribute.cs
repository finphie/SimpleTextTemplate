using System.Diagnostics;

namespace SimpleTextTemplate;

/// <summary>
/// テンプレート文字列と指定されたコンテキストを利用して、バッファに書き込むメソッドを作成することを示す属性です。
/// </summary>
/// <param name="source">テンプレート文字列</param>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
[Conditional("SIMPLETEXTTEMPLATE_KEEP_SOURCE_GENERATOR_ATTRIBUTES")]
public sealed class TemplateAttribute(string source) : Attribute
{
    /// <summary>
    /// テンプレート文字列を取得します。
    /// </summary>
    /// <value>
    /// テンプレート文字列
    /// </value>
    public string Source { get; } = source;
}
