using System.Diagnostics;

namespace SimpleTextTemplate;

/// <summary>
/// テンプレート文字列と指定されたコンテキストを利用して、バッファに書き込むメソッドを作成することを示す属性です。
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
[Conditional("SIMPLETEXTTEMPLATE_KEEP_SOURCE_GENERATOR_ATTRIBUTES")]
public sealed class TemplateAttribute : Attribute
{
    /// <summary>
    /// <see cref="TemplateAttribute"/>クラスの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="source">テンプレート文字列</param>
    public TemplateAttribute(string source)
        => Source = source;

    /// <summary>
    /// テンプレート文字列を取得します。
    /// </summary>
    /// <value>
    /// テンプレート文字列
    /// </value>
    public string Source { get; }
}
