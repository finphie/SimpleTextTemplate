using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
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

        var interceptsLocationInfo = operation.GetInterceptsLocationInfo(cancellationToken);
        var writerType = context.SemanticModel.GetSymbolInfo(invocationExpression.Expression, cancellationToken).Symbol!.ContainingType;

        if (arguments.Count == 1)
        {
            return !TryGetTemplateWriterWriteInfo(in template, out var writeInfo, out var descriptor, cancellationToken)
                ? new(Diagnostic: Diagnostic.Create(descriptor, invocationExpression.GetLocation()))
                : new(CreateInterceptInfo(interceptsLocationInfo, writeInfo, writerType));
        }

        var isInvariantCulture = IsInvariantCulture(context, arguments, cancellationToken);

        var contextArgument = arguments[1].Expression;
        var contextType = context.SemanticModel.GetTypeInfo(contextArgument, cancellationToken).Type!;

        if (!TryGetTemplateWriterWriteInfo(in template, context.SemanticModel.Compilation, contextType, isInvariantCulture, out var writeIdentifierInfo, out var diagnostic, cancellationToken))
        {
            var (descriptor, value) = diagnostic.Value;
            return new(Diagnostic: Diagnostic.Create(descriptor, contextArgument.GetLocation(), value));
        }

        return new(CreateInterceptInfo(interceptsLocationInfo, writeIdentifierInfo, writerType, contextType));
    }

    static bool IsInvariantCulture(GeneratorSyntaxContext context, SeparatedSyntaxList<ArgumentSyntax> arguments, CancellationToken cancellationToken)
    {
        Debug.Assert(arguments.Count is 2 or 3, "TemplateWriter.Writeのコンテキスト指定オーバーロードの引数は、2個または3個である必要があります。");

        // IFormatProvider指定なしの場合、InvariantCultureとする。
        if (arguments.Count == 2)
        {
            return true;
        }

        var providerExpression = arguments[2].Expression;

        // IFormatProviderがnullの場合、InvariantCultureとする。
        if (context.SemanticModel.GetConstantValue(providerExpression, cancellationToken) is { HasValue: true, Value: var constantProvider } && constantProvider is null)
        {
            return true;
        }

        if (providerExpression is not MemberAccessExpressionSyntax { Name.Identifier.ValueText: "InvariantCulture" } providerArgument)
        {
            return false;
        }

        var symbolInfo = context.SemanticModel.GetSymbolInfo(providerArgument, cancellationToken);
        var providerTypeName = symbolInfo.Symbol?.ContainingType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        return providerTypeName == "global::System.Globalization.CultureInfo";
    }

    static InterceptInfo CreateInterceptInfo(InterceptsLocationInfo interceptsLocationInfo, IReadOnlyList<TemplateWriterWriteInfo> writeInfo, INamedTypeSymbol writerTypeSymbol, ITypeSymbol? contextTypeSymbol = null)
    {
        var writerTypeDisplayString = writerTypeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var contextTypeName = contextTypeSymbol?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        return new(interceptsLocationInfo, writeInfo, writerTypeDisplayString, contextTypeName);
    }

    static bool TryGetTemplateWriterWriteInfo(in Template template, [NotNullWhen(true)] out TemplateWriterWriteInfo[]? info, [NotNullWhen(false)] out DiagnosticDescriptor? descriptor, CancellationToken cancellationToken)
    {
        var infoList = new List<TemplateWriterWriteInfo>();

        foreach (var (block, utf8Value, _, _) in template.Blocks)
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

        info = [.. infoList];
        descriptor = null;
        return true;
    }

    static bool TryGetTemplateWriterWriteInfo(in Template template, Compilation compilation, ITypeSymbol contextType, bool isInvariantCulture, [NotNullWhen(true)] out TemplateWriterWriteInfo[]? info, [NotNullWhen(false)] out (DiagnosticDescriptor Descriptor, string Value)? diagnostic, CancellationToken cancellationToken)
    {
        var readOnlySpanByteSymbol = compilation.GetReadOnlySpanTypeSymbol(SpecialType.System_Byte);
        var readOnlySpanCharSymbol = compilation.GetReadOnlySpanTypeSymbol(SpecialType.System_Char);

        var contextMembers = contextType.GetFieldsAndProperties().ToDictionary(static x => x.Name);
        var infoList = new List<TemplateWriterWriteInfo>();
        var isConstant = false;

        foreach (var (block, utf8Value, format, identifierProvider) in template.Blocks)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var value = Encoding.UTF8.GetString(utf8Value);
            var provider = GetFormatProvider(identifierProvider, isInvariantCulture);

            if (block == BlockType.Raw)
            {
                AddConstantString(infoList, ref isConstant, value);
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

            // 識別子が定数かつIFormatProviderの指定がある場合は、定数書き込み
            if (identifier is IFieldSymbol { HasConstantValue: true, ConstantValue: var constantValue } && TryAddConstantValue(infoList, ref isConstant, constantValue, format, provider))
            {
                continue;
            }

            var type = identifier.GetMemberType();

            // 識別子の型をReadOnlySpan<byte>に暗黙的変換できるどうか
            // ReadOnlySpan<byte>やbyte[]などが一致
            if (compilation.ClassifyConversion(type, readOnlySpanByteSymbol).IsImplicit)
            {
                AddValue(infoList, ref isConstant, new(identifier.IsStatic ? WriteStaticLiteral : WriteLiteral, value, format, provider));
                continue;
            }

            // 識別子の型をReadOnlySpan<char>に暗黙的変換できるどうか
            // ReadOnlySpan<char>やstring、char[]などが一致
            if (compilation.ClassifyConversion(type, readOnlySpanCharSymbol).IsImplicit)
            {
                AddValue(infoList, ref isConstant, new(identifier.IsStatic ? WriteStaticString : WriteString, value, format, provider));
                continue;
            }

            if (type.TypeKind == TypeKind.Enum)
            {
                AddValue(infoList, ref isConstant, new(identifier.IsStatic ? WriteStaticEnum : WriteEnum, value, format, provider));
                continue;
            }

            AddValue(infoList, ref isConstant, new(identifier.IsStatic ? WriteStaticValue : WriteValue, value, format, provider));
        }

        info = [.. infoList];
        diagnostic = null;
        return true;

        static IFormatProvider? GetFormatProvider(IFormatProvider? provider, bool isDefaultInvariantCulture)
        {
            return provider is not null ? provider : GetDefaultFormatProvider(isDefaultInvariantCulture);

            static IFormatProvider? GetDefaultFormatProvider(bool isDefaultInvariantCulture)
                => isDefaultInvariantCulture ? CultureInfo.InvariantCulture : null;
        }

        static void AddConstantString(List<TemplateWriterWriteInfo> infoList, ref bool isConstant, string value)
        {
            if (isConstant)
            {
                infoList[^1] = new(WriteConstantLiteral, infoList[^1].Value + value);
                return;
            }

            infoList.Add(new(WriteConstantLiteral, value));
            isConstant = true;
        }

        static void AddValue(List<TemplateWriterWriteInfo> infoList, ref bool isConstant, TemplateWriterWriteInfo info)
        {
            infoList.Add(info);
            isConstant = false;
        }

        static bool TryAddConstantValue(List<TemplateWriterWriteInfo> infoList, ref bool isConstant, object value, string? format, IFormatProvider? provider)
        {
            if (value is string valueString)
            {
                AddConstantString(infoList, ref isConstant, valueString);
                return true;
            }

            if (provider is not null && value is IFormattable formattableValue)
            {
                AddConstantString(infoList, ref isConstant, formattableValue.ToString(format, provider));
                return true;
            }

            return false;
        }
    }
}
