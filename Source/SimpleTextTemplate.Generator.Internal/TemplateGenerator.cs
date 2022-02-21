using Microsoft.CodeAnalysis;
using SimpleTextTemplate.Generator.Extensions;

namespace SimpleTextTemplate.Generator;

/// <summary>
/// 属性引数からテンプレート文字列を取得して、ソースコードを生成します。
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class TemplateGenerator : IncrementalGenerator
{
    const string TemplateAttributeName = $"global::{nameof(SimpleTextTemplate)}.TemplateAttribute";

    /// <inheritdoc/>
    protected override string GetGeneratorName() => nameof(TemplateGenerator);

    /// <inheritdoc/>
    protected override IncrementalValuesProvider<MethodSymbolWithArgument> GetProvider(IncrementalGeneratorInitializationContext context)
        => context.GetMethodSymbolsWithAttributeArgumentProvider(TemplateAttributeName);
}
