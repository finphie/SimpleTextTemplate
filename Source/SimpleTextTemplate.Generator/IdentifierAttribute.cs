using System.Diagnostics;

namespace SimpleTextTemplate;

/// <summary>
/// テンプレート文字列と指定されたコンテキストを利用して、バッファに書き込むメソッドを作成することを示す属性です。
/// </summary>
/// <param name="format">カスタム文字列</param>
[Conditional("SIMPLETEXTTEMPLATE_KEEP_SOURCE_GENERATOR_ATTRIBUTES")]
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class IdentifierAttribute(string format) : Attribute
{
    /// <summary>
    /// カスタム文字列を取得します。
    /// </summary>
    public string Format { get; } = format;
}
