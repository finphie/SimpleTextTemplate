using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using SimpleTextTemplate.Generator.Specs;

namespace SimpleTextTemplate.Generator;

/// <summary>
/// 属性引数からテンプレート文字列を取得して、ソースコードを生成します。
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class TemplateGenerator : IIncrementalGenerator
{
    /// <inheritdoc/>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
#if DEBUG
        if (!System.Diagnostics.Debugger.IsAttached)
        {
            // System.Diagnostics.Debugger.Launch();
        }
#endif
        var provider = context.SyntaxProvider
            .CreateSyntaxProvider(
                static (node, _) => node is InvocationExpressionSyntax { ArgumentList.Arguments.Count: 1 or 2 or 3, Expression: MemberAccessExpressionSyntax { Name.Identifier.ValueText: "Write" } },
                static (context, cancellationToken) =>
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (context.SemanticModel.GetOperation(context.Node, cancellationToken) is not IInvocationOperation operation)
                    {
                        return null;
                    }

                    if (operation is not { TargetMethod.ContainingType: { Name: "TemplateWriter", ContainingNamespace: { Name: nameof(SimpleTextTemplate), ContainingNamespace.IsGlobalNamespace: true } } })
                    {
                        return null;
                    }

                    var creator = new InterceptInfoCreator(context, operation, cancellationToken);
                    creator.Parse();

                    return new InterceptInfoOrDiagnostic(creator.Intercept, creator.Diagnostics);
                })
            .Where(static x => x is not null)
            .Select(static (x, _) => x!);

        var diagnostics = provider.SelectMany(static (x, _) => x.Diagnostics);
        context.RegisterSourceOutput(diagnostics, static (context, diagnostic) => context.ReportDiagnostic(diagnostic));

        var infoList = provider.Where(static x => x.Info is not null).Select(static (x, _) => x.Info!).Collect();
        context.RegisterSourceOutput(infoList, Emitter.Emit);
    }
}
