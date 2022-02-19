using Microsoft.CodeAnalysis;

namespace SimpleTextTemplate.Generator;

static class DiagnosticDescriptors
{
    public static readonly DiagnosticDescriptor STT1000Rule = new(
        "STT1000",
        "テンプレート解析時にエラーが発生しました。",
        "テンプレート解析時に未知のエラーが発生しました。",
        typeof(TemplateGenerator).FullName,
        DiagnosticSeverity.Error,
        true);

    public static readonly DiagnosticDescriptor STT1001Rule = new(
        "STT1001",
        "識別子に無効な文字が含まれています。",
        "テンプレート解析時にエラーが発生しました。識別子に無効な文字が含まれています。",
        "Generator",
        DiagnosticSeverity.Error,
        true);

    public static readonly DiagnosticDescriptor STT1002Rule = new(
        "STT1002",
        "指定されたファイルが存在しません。",
        "テンプレート解析時にエラーが発生しました。指定されたファイルが存在しません。Path: {0}",
        "Generator",
        DiagnosticSeverity.Error,
        true);
}
