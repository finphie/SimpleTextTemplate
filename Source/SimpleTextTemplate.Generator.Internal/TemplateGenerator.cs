using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SimpleTextTemplate.Generator.Specs;
using static SimpleTextTemplate.Generator.Constants;

namespace SimpleTextTemplate.Generator;

/// <summary>
/// 属性引数からテンプレート文字列を取得して、ソースコードを生成します。
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class TemplateGenerator : IIncrementalGenerator
{
    const string RenderMethodName = "Render";

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
            .CreateSyntaxProvider(IsPotentialRenderMethodInvocation, GetInterceptInfoOrDiagnostic)
            .Where(static x => x is not null)
            .Select(static (x, _) => x!);

        var diagnostics = provider.SelectMany(static (x, _) => x.Diagnostics);
        context.RegisterSourceOutput(diagnostics, static (context, diagnostic) => context.ReportDiagnostic(diagnostic));

        var infoList = provider.Where(static x => x.Info is not null).Select(static (x, _) => x.Info!).Collect();
        context.RegisterSourceOutput(
            infoList,
            static (context, infoList) =>
            {
                using var emitter = new Emitter(context, infoList);
                emitter.Emit();
            });
    }

    static bool IsPotentialRenderMethodInvocation(SyntaxNode node, CancellationToken cancellationToken)
    {
        return node is InvocationExpressionSyntax { ArgumentList.Arguments.Count: >= TemplateRenderParameterCount and <= TemplateRenderParameterCountWithContext } syntax
            && syntax switch
            {
                { Expression: MemberAccessExpressionSyntax { Name: IdentifierNameSyntax { Identifier.ValueText: RenderMethodName } } } => true,
                { Expression: IdentifierNameSyntax { Identifier.ValueText: RenderMethodName } } => true,
                _ => false
            };
    }

    static InterceptInfoOrDiagnostic? GetInterceptInfoOrDiagnostic(GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var invocation = (InvocationExpressionSyntax)context.Node;
        var model = context.SemanticModel;

        if (model.GetSymbolInfo(invocation, cancellationToken).Symbol is not IMethodSymbol { Name: RenderMethodName } methodSymbol)
        {
            return null;
        }

        var compilation = model.Compilation;

        if (!IsTemplateClass(compilation, methodSymbol))
        {
            return null;
        }

        if (!IsValidRenderMethodParameters(compilation, methodSymbol.Parameters))
        {
            return null;
        }

        var creator = new InterceptInfoCreator(context, methodSymbol);
        creator.Parse(cancellationToken);

        return new InterceptInfoOrDiagnostic(creator.Intercept, creator.Diagnostics);
    }

    static bool IsTemplateClass(Compilation compilation, IMethodSymbol methodSymbol)
    {
        var targetType = compilation.GetTypeByMetadataName("SimpleTextTemplate.TemplateRenderer");
        return targetType is not null && methodSymbol.ContainingType.Equals(targetType, SymbolEqualityComparer.Default);
    }

    static bool IsValidRenderMethodParameters(Compilation compilation, ImmutableArray<IParameterSymbol> parameters)
    {
        if (parameters.Length is not (TemplateRenderParameterCount or TemplateRenderParameterCountWithContext))
        {
            throw new InvalidOperationException("Invalid parameters.");
        }

        if (parameters is not [var firstParameter, var secondParameter, ..])
        {
            return false;
        }

        // TemplateWriter<T>
        if (!IsTemplateWriterType(compilation, firstParameter))
        {
            return false;
        }

        // string
        if (secondParameter.Type.SpecialType != SpecialType.System_String)
        {
            return false;
        }

        if (parameters.Length == TemplateRenderParameterCount)
        {
            return true;
        }

        // context
        if (!IsContextType(parameters[TemplateRenderContextIndex]))
        {
            return false;
        }

        // IFormatProvider
        return IsIFormatProviderType(compilation, parameters[TemplateRenderIFormatProviderIndex]);
    }

    static bool IsTemplateWriterType(Compilation compilation, IParameterSymbol parameter)
    {
        if (!parameter.RefKind.Equals(RefKind.Ref))
        {
            return false;
        }

        if (parameter.Type is not INamedTypeSymbol parameterType)
        {
            return false;
        }

        var templateWriterType = compilation.GetTypeByMetadataName("SimpleTextTemplate.TemplateWriter`1");
        return templateWriterType is not null && parameterType.ConstructedFrom.Equals(templateWriterType, SymbolEqualityComparer.Default);
    }

    static bool IsContextType(IParameterSymbol parameter) => parameter.RefKind.Equals(RefKind.In);

    static bool IsIFormatProviderType(Compilation compilation, IParameterSymbol parameter)
    {
        if (!parameter.HasExplicitDefaultValue || parameter.ExplicitDefaultValue is not null || parameter.NullableAnnotation != NullableAnnotation.Annotated)
        {
            return false;
        }

        var providerType = compilation.GetTypeByMetadataName("System.IFormatProvider");
        return providerType is not null && parameter.Type.Equals(providerType, SymbolEqualityComparer.Default);
    }
}
