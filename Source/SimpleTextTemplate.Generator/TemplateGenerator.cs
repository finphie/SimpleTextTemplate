using System.IO;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace SimpleTextTemplate.Generator
{
    /// <summary>
    /// テンプレートを生成するソースジェネレータークラスです。
    /// </summary>
    [Generator]
    sealed class TemplateGenerator : ISourceGenerator
    {
        static readonly DiagnosticDescriptor STT1001Rule = new(
            "STT1001",
            "存在しないディレクトリが指定されています。",
            "存在しないディレクトリが指定されています。 SimpleTextTemplatePath: {0}",
            "Generator",
            DiagnosticSeverity.Error,
            true);

        /// <inheritdoc/>
        public void Initialize(GeneratorInitializationContext context)
        {
#if DEBUG
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                // System.Diagnostics.Debugger.Launch();
            }
#endif
        }

        /// <inheritdoc/>
        public void Execute(GeneratorExecutionContext context)
        {
            if (!context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.SimpleTextTemplatePath", out var path) || string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            if (!context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.SimpleTextTemplateSearchPattern", out var searchPattern) || string.IsNullOrWhiteSpace(searchPattern))
            {
                searchPattern = "*";
            }

            if (!Directory.Exists(path))
            {
                context.ReportDiagnostic(Diagnostic.Create(STT1001Rule, null, path));
                return;
            }

            var files = Directory.EnumerateFiles(path, searchPattern, SearchOption.AllDirectories);

            foreach (var file in files)
            {
                if (!File.Exists(file))
                {
                    continue;
                }

                var template = new CodeTemplate(file);
                var text = template.TransformText();

                context.AddSource($"Template.{template.FileName}.Generated.cs", SourceText.From(text, Encoding.UTF8));
            }
        }
    }
}