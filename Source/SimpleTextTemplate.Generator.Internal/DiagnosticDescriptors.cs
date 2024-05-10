using Microsoft.CodeAnalysis;

namespace SimpleTextTemplate.Generator;

/// <summary>
/// 診断情報
/// </summary>
static class DiagnosticDescriptors
{
    /// <summary>
    /// テンプレート文字列が定数ではない場合に出力する診断情報です。
    /// </summary>
    public static readonly DiagnosticDescriptor TemplateStringMustBeConstant = new(
        "STT1000",
        "テンプレート文字列が不正な形式です。",
        "テンプレート文字列はnull以外の定数にする必要があります。",
        nameof(SimpleTextTemplate),
        DiagnosticSeverity.Error,
        true);

    /// <summary>
    /// テンプレート文字列に識別子があり、コンテキストを指定していない場合に出力する診断情報です。
    /// </summary>
    public static readonly DiagnosticDescriptor RequiredContext = new(
        "STT1001",
        "テンプレート文字列が不正な形式です。",
        "テンプレート文字列に変数を埋め込む場合、コンテキストを指定する必要があります。",
        nameof(SimpleTextTemplate),
        DiagnosticSeverity.Error,
        true);

    /// <summary>
    /// コンテキストに識別子が存在しない場合に出力する診断情報です。
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidIdentifier = new(
        "STT1002",
        "コンテキストが不正な形式です。",
        "コンテキストに識別子({0})が存在しません。",
        nameof(SimpleTextTemplate),
        DiagnosticSeverity.Error,
        true);

    /// <summary>
    /// テンプレート解析に失敗した場合に出力する診断情報です。
    /// </summary>
    public static readonly DiagnosticDescriptor ParserError = new(
        "STT1003",
        "テンプレート解析時にエラーが発生しました。",
        "テンプレート解析時にエラーが発生しました。位置:{0}",
        nameof(SimpleTextTemplate),
        DiagnosticSeverity.Error,
        true);

    /// <summary>
    /// <see cref="IFormattable"/>を実装していない定数識別子に対して、書式指定や<see cref="IFormatProvider"/>の指定がされている場合に出力する診断情報です。
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidConstantIdentifierFormatProvider = new(
        "STT1004",
        "IFormattableが実装されていない識別子に、formatまたはproviderが指定されています。",
        "IFormattableが実装されていない識別子には、formatやproviderの指定は無効です。",
        nameof(SimpleTextTemplate),
        DiagnosticSeverity.Error,
        true);

    /// <summary>
    /// <see cref="Enum"/>に<see cref="IFormatProvider"/>の指定がされている場合に出力する診断情報です。
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidEnumIdentifierFormatProvider = new(
        "STT1005",
        "列挙型識別子にproviderが指定されています。",
        "列挙型識別子には、providerの指定は無効です。",
        nameof(SimpleTextTemplate),
        DiagnosticSeverity.Error,
        true);

    /// <summary>
    /// <see cref="IFormattable"/>/ISpanFormattable/IUtf8Formattableのいずれかのインターフェイスを実装していない識別子に対して、書式指定や<see cref="IFormatProvider"/>の指定がされている場合に出力する診断情報です。
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidIdentifierFormatProvider = new(
        "STT1006",
        "IFormattable/ISpanFormattable/IUtf8Formattableのいずれかのインターフェイスを実装していない識別子に、formatまたはproviderが指定されています。",
        "IFormattable/ISpanFormattable/IUtf8Formattableのいずれかのインターフェイスを実装していない識別子には、formatやproviderの指定は無効です。",
        nameof(SimpleTextTemplate),
        DiagnosticSeverity.Error,
        true);
}
