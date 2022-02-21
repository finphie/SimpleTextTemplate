using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using SimpleTextTemplate.Generator.Extensions;

namespace SimpleTextTemplate.Generator;

/// <summary>
/// テンプレートを生成するソースジェネレーターです。
/// </summary>
public abstract class IncrementalGenerator : IIncrementalGenerator
{
    /// <inheritdoc/>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var methods = GetProvider(context);

        context.RegisterSourceOutput(methods, (context, method) =>
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            if (!TryGetTemplateSource(context, method, out var source))
            {
                return;
            }

            var template = new SourceCodeTemplate(method.Symbol, source);
            var result = template.TryParse();

            if (result == ParseResult.InvalidIdentifier)
            {
                context.ReportDiagnostic(DiagnosticDescriptors.InvalidIdentifierError, method.Symbol);
                return;
            }

            if (result != ParseResult.Success)
            {
                context.ReportDiagnostic(DiagnosticDescriptors.UnknownError, method.Symbol);
                return;
            }

            var fileName = $"__{GetGeneratorName()}.{template.ClassName}.{template.MethodName}.Generated.cs";
            context.AddSource(fileName, SourceText.From(template.TransformText(), Encoding.UTF8));
        });
    }

    /// <summary>
    /// ソースジェネレーター名を取得します。
    /// </summary>
    /// <returns>ソースジェネレーター名を返します。</returns>
    protected abstract string GetGeneratorName();

    /// <summary>
    /// プロバイダーを取得します。
    /// </summary>
    /// <param name="context">コンテキスト</param>
    /// <returns>プロバイダーを返します。</returns>
    protected abstract IncrementalValuesProvider<MethodSymbolWithArgument> GetProvider(IncrementalGeneratorInitializationContext context);

    /// <summary>
    /// <see cref="SourceCodeTemplate"/>クラスで使用するテンプレート文字列を取得します。
    /// </summary>
    /// <param name="context">コンテキスト</param>
    /// <param name="method">メソッドシンボルと属性の引数データ</param>
    /// <param name="template">テンプレート文字列</param>
    /// <returns>
    /// テンプレート文字列を取得できた場合は<see langword="true"/>、
    /// それ以外の場合は<see langword="false"/>を返します。
    /// </returns>
    protected virtual bool TryGetTemplateSource(SourceProductionContext context, MethodSymbolWithArgument method, [MaybeNullWhen(false)] out byte[] template)
    {
        template = Encoding.UTF8.GetBytes(method.Argument);
        return true;
    }
}
