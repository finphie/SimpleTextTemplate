using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;

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
        if (!Debugger.IsAttached)
        {
            // Debugger.Launch();
        }
#endif
        var provider = context.SyntaxProvider
            .CreateSyntaxProvider(
                static (node, _) => node is InvocationExpressionSyntax { Expression: MemberAccessExpressionSyntax { Name.Identifier.ValueText: "Write" } },
                static (context, cancellationToken) =>
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var invocationExpression = (context.Node as InvocationExpressionSyntax)!;
                    var memberAccessorExpression = (invocationExpression.Expression as MemberAccessExpressionSyntax)!;

                    if (context.SemanticModel.GetOperation(invocationExpression, cancellationToken) is not IInvocationOperation operation)
                    {
                        return null;
                    }

                    if (operation is not { TargetMethod.ContainingType: { Name: "TemplateWriter", ContainingNamespace: { Name: nameof(SimpleTextTemplate), ContainingNamespace.IsGlobalNamespace: true } } })
                    {
                        return null;
                    }

                    var arguments = invocationExpression.ArgumentList.Arguments;
                    return arguments.Count == 0 ? null : InterceptInfoCreator.Create(context, operation, cancellationToken);
                })
            .Where(static x => x is not null)
            .Select(static (x, _) => x!);

        var diagnostics = provider.Where(static x => x.Diagnostic is not null).Select(static (x, _) => x.Diagnostic!);
        context.RegisterSourceOutput(diagnostics, static (context, diagnostic) => context.ReportDiagnostic(diagnostic));

        var infoList = provider.Where(static x => x.Info is not null).Select(static (x, _) => x.Info!).Collect();
        context.RegisterSourceOutput(infoList, Emitter.Emit);
    }
}
