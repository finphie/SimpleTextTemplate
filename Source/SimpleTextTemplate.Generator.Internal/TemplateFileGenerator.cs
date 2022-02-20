using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using SimpleTextTemplate.Generator.Extensions;

namespace SimpleTextTemplate.Generator;

/// <summary>
/// ファイルからテンプレート文字列を取得して、ソースコードを生成します。
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class TemplateFileGenerator : IncrementalGenerator
{
    const string TemplateFileAttributeName = $"global::{nameof(SimpleTextTemplate)}.TemplateFileAttribute";

    /// <inheritdoc/>
    protected override string GetGeneratorName() => nameof(TemplateFileGenerator);

    /// <inheritdoc/>
    protected override IncrementalValuesProvider<MethodSymbolWithArgument> GetProvider(IncrementalGeneratorInitializationContext context)
    {
        var methods = context.GetMethodSymbolsWithAttributeArgumentProvider(TemplateFileAttributeName);
        var options = context.AnalyzerConfigOptionsProvider
            .Select(static (provider, token) =>
            {
                token.ThrowIfCancellationRequested();

                var globalOptions = provider.GlobalOptions;

                if (globalOptions.TryGetValue("build_property.SimpleTextTemplatePath", out var path) && path is not null)
                {
                    return path;
                }

                if (globalOptions.TryGetValue("build_property.ProjectDir", out var projectDirectory) && projectDirectory is not null)
                {
                    return projectDirectory;
                }

                return string.Empty;
            })
            .WithComparer(EqualityComparer<string>.Default);

        return methods.Combine(options).Select(static (info, token) =>
        {
            token.ThrowIfCancellationRequested();

            var (method, directory) = info;
            var path = Path.Combine(directory, method.Argument);

            return method with
            {
                Argument = path
            };
        });
    }

    /// <inheritdoc/>
    protected override bool TryGetTemplateSource(SourceProductionContext context, MethodSymbolWithArgument method, [MaybeNullWhen(false)] out byte[] template)
    {
        var path = method.Argument;

        if (!File.Exists(path))
        {
            context.ReportDiagnostic(DiagnosticDescriptors.FileNotFoundError, method.Symbol, path);

            template = null;
            return false;
        }

        template = File.ReadAllBytes(path);
        return true;
    }
}
