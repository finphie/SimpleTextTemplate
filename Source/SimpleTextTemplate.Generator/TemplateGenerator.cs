using System.IO;
using System.Linq;
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
        const string AbstractionsNamespace = "SimpleTextTemplate.Abstractions";
        const string STT1000RuleMessage = $"{AbstractionsNamespace}が参照されていません。";

        static readonly DiagnosticDescriptor STT1000Rule = new(
            "STT1000",
            STT1000RuleMessage,
            STT1000RuleMessage,
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
            if (!context.Compilation.ReferencedAssemblyNames.Any(x => x.Name == AbstractionsNamespace))
            {
                context.ReportDiagnostic(Diagnostic.Create(STT1000Rule, null));
                return;
            }

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