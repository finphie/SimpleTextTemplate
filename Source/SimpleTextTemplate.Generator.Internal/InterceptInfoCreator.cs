using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using SimpleTextTemplate.Generator.Extensions;
using SimpleTextTemplate.Generator.Specs;
using static SimpleTextTemplate.Generator.Specs.TemplateWriterWriteType;

namespace SimpleTextTemplate.Generator;

/// <summary>
/// <see cref="InterceptInfo"/>を作成するクラスです。
/// </summary>
static class InterceptInfoCreator
{
    /// <summary>
    /// <see cref="InterceptInfo"/>または<see cref="Diagnostic"/>を作成します。
    /// </summary>
    /// <param name="context">コンテキスト</param>
    /// <param name="operation">メソッド呼び出し</param>
    /// <param name="cancellationToken">キャンセル要求を行うためのトークン</param>
    /// <returns>
    /// <see cref="InterceptInfo"/>インスタンスを作成できた場合はそのインスタンスを返します。
    /// 作成できなかった場合は<see cref="Diagnostic"/>のインスタンスを返します。
    /// </returns>
    public static InterceptInfoOrDiagnostic Create(GeneratorSyntaxContext context, IInvocationOperation operation, CancellationToken cancellationToken)
    {
        var invocationExpression = (context.Node as InvocationExpressionSyntax)!;
        var arguments = invocationExpression.ArgumentList.Arguments;
        var templateArgument = arguments[0].Expression;

        if (context.SemanticModel.GetConstantValue(templateArgument, cancellationToken).Value is not string templateString)
        {
            return new(Diagnostic: Diagnostic.Create(DiagnosticDescriptors.TemplateStringMustBeConstant, templateArgument.GetLocation()));
        }

        if (!Template.TryParse(Encoding.UTF8.GetBytes(templateString), out var template, out var consumed))
        {
            return new(Diagnostic: Diagnostic.Create(DiagnosticDescriptors.ParserError, templateArgument.GetLocation(), consumed + 1));
        }

        TemplateWriterWriteInfo[]? writeInfo;
        string? contextTypeName = null;

        if (arguments.Count == 1)
        {
            if (!TryGetTemplateWriterWriteInfo(in template, out writeInfo, out var descriptor, cancellationToken))
            {
                return new(Diagnostic: Diagnostic.Create(descriptor, invocationExpression.GetLocation()));
            }
        }
        else
        {
            var contextArgument = arguments[1].Expression;
            var contextType = context.SemanticModel.GetTypeInfo(contextArgument, cancellationToken).Type!;

            if (!TryGetTemplateWriterWriteInfo(in template, context.SemanticModel.Compilation, contextType, out writeInfo, out var diagnostic, cancellationToken))
            {
                var (descriptor, value) = diagnostic.Value;
                return new(Diagnostic: Diagnostic.Create(descriptor, contextArgument.GetLocation(), value));
            }

            contextTypeName = contextType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        }

        var interceptsLocationInfo = operation.GetInterceptsLocationInfo(cancellationToken);
        var writerType = context.SemanticModel.GetSymbolInfo(invocationExpression.Expression, cancellationToken).Symbol!.ContainingType;

        var info = new InterceptInfo(interceptsLocationInfo, writeInfo.ToArray(), writerType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat), contextTypeName);
        return new(info);
    }

    static bool TryGetTemplateWriterWriteInfo(in Template template, [NotNullWhen(true)] out TemplateWriterWriteInfo[]? info, [NotNullWhen(false)] out DiagnosticDescriptor? descriptor, CancellationToken cancellationToken)
    {
        var infoList = new List<TemplateWriterWriteInfo>();

        foreach (var (block, utf8Value) in template.Blocks)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var value = Encoding.UTF8.GetString(utf8Value);

            if (block == BlockType.Raw)
            {
                infoList.Add(new(WriteConstantLiteral, value));
                continue;
            }

            Debug.Assert(block == BlockType.Identifier, "BlockType.Identifierではない場合、テンプレート解析に不具合があります。");

            info = null;
            descriptor = DiagnosticDescriptors.RequiredContext;
            return false;
        }

#pragma warning disable IDE0305 // コレクションの初期化を簡略化します
        info = infoList.ToArray();
#pragma warning restore IDE0305 // コレクションの初期化を簡略化します

        descriptor = null;
        return true;
    }

    static bool TryGetTemplateWriterWriteInfo(in Template template, Compilation compilation, ITypeSymbol contextType, [NotNullWhen(true)] out TemplateWriterWriteInfo[]? info, [NotNullWhen(false)] out (DiagnosticDescriptor Descriptor, string Value)? diagnostic, CancellationToken cancellationToken)
    {
        var readOnlySpanByteSymbol = compilation.GetReadOnlySpanTypeSymbol(SpecialType.System_Byte);
        var readOnlySpanCharSymbol = compilation.GetReadOnlySpanTypeSymbol(SpecialType.System_Char);

        var contextMembers = contextType.GetFieldsAndProperties().ToDictionary(static x => x.Name, static x => (Type: x.GetMemberType(), x.IsStatic));
        var infoList = new List<TemplateWriterWriteInfo>();

        foreach (var (block, utf8Value) in template.Blocks)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var value = Encoding.UTF8.GetString(utf8Value);

            if (block == BlockType.Raw)
            {
                infoList.Add(new(WriteConstantLiteral, value));
                continue;
            }

            Debug.Assert(block == BlockType.Identifier, "BlockType.Identifierではない場合、テンプレート解析に不具合があります。");

            // 識別子がコンテキストに存在しない
            if (!contextMembers.TryGetValue(value, out var identifier))
            {
                info = null;
                diagnostic = (DiagnosticDescriptors.InvalidIdentifier, value);
                return false;
            }

            if (identifier.Type.TypeKind == TypeKind.Enum)
            {
                infoList.Add(new(identifier.IsStatic ? WriteStaticEnum : WriteEnum, value));
                continue;
            }

            // 識別子の型をReadOnlySpan<byte>に暗黙的変換できるどうか
            // ReadOnlySpan<byte>やbyte[]などが一致
            if (compilation.ClassifyConversion(identifier.Type, readOnlySpanByteSymbol).IsImplicit)
            {
                infoList.Add(new(identifier.IsStatic ? WriteStaticLiteral : WriteLiteral, value));
                continue;
            }

            // 識別子の型をReadOnlySpan<char>に暗黙的変換できるどうか
            // ReadOnlySpan<char>やstring、char[]などが一致
            if (compilation.ClassifyConversion(identifier.Type, readOnlySpanCharSymbol).IsImplicit)
            {
                infoList.Add(new(identifier.IsStatic ? WriteStaticString : WriteString, value));
                continue;
            }

            infoList.Add(new(identifier.IsStatic ? WriteStaticValue : WriteValue, value));
        }

#pragma warning disable IDE0305 // コレクションの初期化を簡略化します
        info = infoList.ToArray();
#pragma warning restore IDE0305 // コレクションの初期化を簡略化します

        diagnostic = null;
        return true;
    }
}
