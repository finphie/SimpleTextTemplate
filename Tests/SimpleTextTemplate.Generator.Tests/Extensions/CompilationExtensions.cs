using System.Buffers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SimpleTextTemplate.Generator.Tests.Extensions;

/// <summary>
/// <see cref="Compilation"/>クラス関連の拡張メソッド集です。
/// </summary>
static class CompilationExtensions
{
    /// <summary>
    /// インターセプター情報を取得します。
    /// </summary>
    /// <param name="compilation">コンパイル情報</param>
    /// <returns>インターセプター情報を返します。</returns>
    public static InterceptInfo[] GetInterceptInfo(this Compilation compilation)
    {
        var generatedSyntaxTrees = compilation.SyntaxTrees.Last();
        var root = generatedSyntaxTrees.GetCompilationUnitRoot();
        var namespaceDeclaration = (NamespaceDeclarationSyntax)root.Members.First();
        var interceptDeclaration = (ClassDeclarationSyntax)namespaceDeclaration.Members.Single();
        var blocks = interceptDeclaration.Members
            .Where(static x => x is MethodDeclarationSyntax { AttributeLists: [{ Attributes: [{ Name: var attribute }] }] } && attribute.ToFullString() == "global::System.Runtime.CompilerServices.InterceptsLocation")
            .Cast<MethodDeclarationSyntax>()
            .Select(static member =>
            {
                var block = member.Body!;
                var statements = block.Statements.Cast<ExpressionStatementSyntax>()
                    .Select(static statement =>
                    {
                        var writer = (InvocationExpressionSyntax)statement.Expression;
                        var writerMethod = (MemberAccessExpressionSyntax)writer.Expression;
                        var identifier = ((IdentifierNameSyntax)writerMethod.Name).Identifier.ValueText;
                        var arguments = writer.ArgumentList.Arguments.Select(static x => x.ToFullString()).ToArray();

                        return new MethodInfo(
                            identifier,
                            arguments[0],
                            arguments.Length > 1 ? arguments[1] : null,
                            arguments.Length > 2 ? arguments[2] : null);
                    });

                return new InterceptInfo(statements.ToArray());
            });

        return blocks.ToArray();
    }
}
