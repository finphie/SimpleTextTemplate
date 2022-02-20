using Microsoft.CodeAnalysis;

namespace SimpleTextTemplate.Generator;

/// <summary>
/// 診断情報
/// </summary>
static class DiagnosticDescriptors
{
    /// <summary>
    /// 未知のエラーの場合に出力する診断情報です。
    /// </summary>
    public static readonly DiagnosticDescriptor UnknownError = new(
        "STT1000",
        "テンプレート解析時にエラーが発生しました。",
        "テンプレート解析時に未知のエラーが発生しました。",
        nameof(SimpleTextTemplate),
        DiagnosticSeverity.Error,
        true);

    /// <summary>
    /// 無効な文字が含まれている場合に出力する診断情報です。
    /// </summary>
    public static readonly DiagnosticDescriptor InvalidIdentifierError = new(
        "STT1001",
        "識別子に無効な文字が含まれています。",
        "テンプレート解析時にエラーが発生しました。識別子に無効な文字が含まれています。",
        nameof(SimpleTextTemplate),
        DiagnosticSeverity.Error,
        true);

    /// <summary>
    /// ファイルが存在しない場合に出力する診断情報です。
    /// </summary>
    public static readonly DiagnosticDescriptor FileNotFoundError = new(
        "STT1002",
        "指定されたファイルが存在しません。",
        "テンプレート解析時にエラーが発生しました。指定されたファイルが存在しません。Path: {0}",
        nameof(SimpleTextTemplate),
        DiagnosticSeverity.Error,
        true);
}
