using System.Diagnostics;
using System.Globalization;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using SimpleTextTemplate.Generator.Extensions;
using SimpleTextTemplate.Generator.Specs;
using static SimpleTextTemplate.Generator.Constants;
using static SimpleTextTemplate.Generator.Specs.TemplateWriterWriteType;

namespace SimpleTextTemplate.Generator;

/// <summary>
/// <see cref="InterceptInfo"/>を作成する構造体です。
/// </summary>
ref struct InterceptInfoCreator
{
    readonly GeneratorSyntaxContext _context;
    readonly SemanticModel _semanticModel;
    readonly Compilation _compilation;
    readonly IMethodSymbol _methodSymbol;

    readonly InvocationExpressionSyntax _invocationExpression;

    readonly SeparatedSyntaxList<ArgumentSyntax> _arguments;
    readonly ExpressionSyntax _templateArgument;

    readonly List<TemplateWriterWriteInfo> _writerInfoList = [];
    readonly List<Diagnostic> _diagnostics = [];

    readonly INamedTypeSymbol _readOnlySpanByteSymbol;
    readonly INamedTypeSymbol _readOnlySpanCharSymbol;

#pragma warning disable RSEXPERIMENTAL002 // 種類は、評価の目的でのみ提供されています。将来の更新で変更または削除されることがあります。続行するには、この診断を非表示にします。
    InterceptableLocation? _interceptableLocation;
#pragma warning restore RSEXPERIMENTAL002 // 種類は、評価の目的でのみ提供されています。将来の更新で変更または削除されることがあります。続行するには、この診断を非表示にします。

    bool _success;
    bool _isConstant;

    /// <summary>
    /// <see cref="InterceptInfoCreator"/>構造体の新しいインスタンスを取得します。
    /// </summary>
    /// <param name="context">コンテキスト</param>
    /// <param name="methodSymbol">メソッドシンボル</param>
    public InterceptInfoCreator(GeneratorSyntaxContext context, IMethodSymbol methodSymbol)
    {
        _context = context;
        _semanticModel = _context.SemanticModel;
        _compilation = _semanticModel.Compilation;
        _methodSymbol = methodSymbol;

        _invocationExpression = _context.Node as InvocationExpressionSyntax
            ?? throw new ArgumentException($"{nameof(context)}.{nameof(context.Node)}が{nameof(InvocationExpressionSyntax)}ではありません。");

        _arguments = _invocationExpression.ArgumentList.Arguments;

        if (_arguments.Count is < TemplateRenderParameterCount or > TemplateRenderParameterCountWithContext)
        {
            throw new ArgumentException("Template.Renderの引数は、2～4個である必要があります。");
        }

        _templateArgument = _arguments[TemplateRenderTextIndex].Expression;

        _readOnlySpanByteSymbol = _compilation.GetReadOnlySpanTypeSymbol(SpecialType.System_Byte);
        _readOnlySpanCharSymbol = _compilation.GetReadOnlySpanTypeSymbol(SpecialType.System_Char);
    }

    /// <summary>
    /// <see cref="InterceptInfo"/>を取得します。
    /// </summary>
    public readonly InterceptInfo? Intercept
        => _success && _interceptableLocation is not null
        ? new(_interceptableLocation, [.. _writerInfoList], _methodSymbol)
        : null;

    /// <summary>
    /// <see cref="Diagnostic"/>のリストを取得します。
    /// </summary>
    public readonly IReadOnlyList<Diagnostic> Diagnostics => [.. _diagnostics];

    /// <summary>
    /// テンプレート文字列を解析します。
    /// </summary>
    /// <param name="cancellationToken">キャンセル要求を行うためのトークン</param>
    public void Parse(CancellationToken cancellationToken)
    {
#pragma warning disable RSEXPERIMENTAL002 // 種類は、評価の目的でのみ提供されています。将来の更新で変更または削除されることがあります。続行するには、この診断を非表示にします。
        _interceptableLocation ??= _semanticModel.GetInterceptableLocation(_invocationExpression, cancellationToken)
            ?? throw new InvalidOperationException("インターセプト可能な場所を取得できませんでした。");
#pragma warning restore RSEXPERIMENTAL002 // 種類は、評価の目的でのみ提供されています。将来の更新で変更または削除されることがあります。続行するには、この診断を非表示にします。

        if (_semanticModel.GetConstantValue(_templateArgument, cancellationToken).Value is not string templateString)
        {
            // string.Emptyは定数として扱う。
            if (_semanticModel.GetOperation(_templateArgument, cancellationToken) is not IFieldReferenceOperation { Field: { Name: nameof(string.Empty), ContainingType.SpecialType: SpecialType.System_String } })
            {
                _diagnostics.Add(Diagnostic.Create(DiagnosticDescriptors.TemplateStringMustBeConstant, _templateArgument.GetLocation()));
                return;
            }

            templateString = string.Empty;
        }

        if (!Template.TryParse(Encoding.UTF8.GetBytes(templateString), out var template, out var consumed))
        {
            _diagnostics.Add(Diagnostic.Create(DiagnosticDescriptors.ParserError, _templateArgument.GetLocation(), consumed + 1));
            return;
        }

        if (_arguments.Count == TemplateRenderParameterCount)
        {
            AnalyzeTemplate(in template, cancellationToken);
            return;
        }

        AnalyzeTemplateWithContext(in template, cancellationToken);
    }

    readonly bool IsInvariantCulture(CancellationToken cancellationToken)
    {
        // IFormatProvider指定なしの場合、InvariantCultureとする。
        if (_arguments.Count == 3)
        {
            return true;
        }

        var providerExpression = _arguments[TemplateRenderIFormatProviderIndex].Expression;

        // IFormatProviderがnullの場合、InvariantCultureとする。
        if (_semanticModel.GetConstantValue(providerExpression, cancellationToken) is { HasValue: true, Value: var constantProvider } && constantProvider is null)
        {
            return true;
        }

        var operation = _semanticModel.GetOperation(providerExpression, cancellationToken);

        return operation is IPropertyReferenceOperation { Property.ContainingNamespace: { Name: nameof(System.Globalization), ContainingNamespace: { Name: nameof(System), ContainingNamespace.IsGlobalNamespace: true } } } providerOperation
            && providerOperation switch
            {
                { Property: { Name: nameof(CultureInfo.InvariantCulture), ContainingType.Name: nameof(CultureInfo) } } or
                { Property: { Name: nameof(DateTimeFormatInfo.InvariantInfo), ContainingType.Name: nameof(DateTimeFormatInfo) } } => true,
                _ => false
            };
    }

    void AnalyzeTemplate(in Template template, CancellationToken cancellationToken)
    {
        foreach (var (block, utf8Value, _, _) in template.Blocks)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var value = Encoding.UTF8.GetString(utf8Value);

            if (block != BlockType.Raw)
            {
                Debug.Assert(block == BlockType.Identifier, "BlockType.Identifierではない場合、テンプレート解析に不具合があります。");

                _diagnostics.Add(Diagnostic.Create(DiagnosticDescriptors.RequiredContext, _invocationExpression.GetLocation()));
                return;
            }

            _writerInfoList.Add(new(WriteConstantLiteral, value));
        }

        _success = true;
    }

    void AnalyzeTemplateWithContext(in Template template, CancellationToken cancellationToken)
    {
        var contextArgument = _arguments[TemplateRenderContextIndex].Expression;
        var contextType = _semanticModel.GetTypeInfo(contextArgument, cancellationToken).Type;

        if (contextArgument is null || contextType is null)
        {
            throw new InvalidOperationException("コンテキストの型情報が取得できません。");
        }

        var isInvariantCulture = IsInvariantCulture(cancellationToken);
        var contextMembers = contextType.GetFieldsAndProperties().ToDictionary(
            static x => x.Name,
            static x => (Symbol: x, Formattable: x.GetFieldOrPropertyType().Interfaces.GetFormattableType()));

        foreach (var (block, utf8Value, format, identifierProvider) in template.Blocks)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var value = Encoding.UTF8.GetString(utf8Value);
            var provider = GetFormatProvider(identifierProvider, isInvariantCulture);

            if (block == BlockType.Raw)
            {
                AddConstantString(value);
                continue;
            }

            Debug.Assert(block == BlockType.Identifier, "BlockType.Identifierではない場合、テンプレート解析に不具合があります。");

            // 識別子がコンテキストに存在しない
            if (!contextMembers.TryGetValue(value, out var identifier))
            {
                _diagnostics.Add(Diagnostic.Create(DiagnosticDescriptors.InvalidIdentifier, contextArgument.GetLocation(), value));
                continue;
            }

            // 識別子が定数かつIFormatProviderの指定がある場合は、定数書き込み
            if (identifier.Symbol is IFieldSymbol { HasConstantValue: true, ConstantValue: var constantValue } && TryAddConstantValue(constantValue, format, provider))
            {
                // IFormattableを実装していない識別子で、formatまたはproviderが設定されている場合
                if (!identifier.Formattable.HasFlag(FormattableType.IFormattable) && (format is not null || identifierProvider is not null))
                {
                    _diagnostics.Add(Diagnostic.Create(DiagnosticDescriptors.InvalidConstantIdentifierFormatProvider, _templateArgument.GetLocation()));
                }

                continue;
            }

            var type = identifier.Symbol.GetFieldOrPropertyType();

            if (type.TypeKind == TypeKind.Enum)
            {
                if (identifierProvider is not null)
                {
                    _diagnostics.Add(Diagnostic.Create(DiagnosticDescriptors.InvalidEnumIdentifierFormatProvider, _templateArgument.GetLocation()));
                }

                AddValue(new(identifier.Symbol.IsStatic ? WriteStaticEnum : WriteEnum, value, format, provider));
                continue;
            }

            // IFormattable/ISpanFormattable/IUtf8Formattableを実装していない識別子で、formatまたはproviderが設定されている場合
            if (identifier.Formattable == FormattableType.None && (format is not null || identifierProvider is not null))
            {
                _diagnostics.Add(Diagnostic.Create(DiagnosticDescriptors.InvalidIdentifierFormatProvider, _templateArgument.GetLocation()));
            }

            // 識別子の型をReadOnlySpan<byte>に暗黙的変換できるどうか
            // ReadOnlySpan<byte>やbyte[]などが一致
            if (_compilation.ClassifyConversion(type, _readOnlySpanByteSymbol).IsImplicit)
            {
                AddValue(new(identifier.Symbol.IsStatic ? WriteStaticLiteral : WriteLiteral, value, format, provider));
                continue;
            }

            // 識別子の型をReadOnlySpan<char>に暗黙的変換できるどうか
            // ReadOnlySpan<char>やstring、char[]などが一致
            if (_compilation.ClassifyConversion(type, _readOnlySpanCharSymbol).IsImplicit)
            {
                AddValue(new(identifier.Symbol.IsStatic ? WriteStaticString : WriteString, value, format, provider));
                continue;
            }

            AddValue(new(identifier.Symbol.IsStatic ? WriteStaticValue : WriteValue, value, format, provider));
        }

        _success = true;

        static IFormatProvider? GetFormatProvider(IFormatProvider? provider, bool isDefaultInvariantCulture)
        {
            return provider is not null ? provider : GetDefaultFormatProvider(isDefaultInvariantCulture);

            static IFormatProvider? GetDefaultFormatProvider(bool isDefaultInvariantCulture)
                => isDefaultInvariantCulture ? CultureInfo.InvariantCulture : null;
        }
    }

    void AddConstantString(string value)
    {
        if (_isConstant)
        {
            _writerInfoList[^1] = new(WriteConstantLiteral, _writerInfoList[^1].Value + value);
            return;
        }

        _writerInfoList.Add(new(WriteConstantLiteral, value));
        _isConstant = true;
    }

    void AddValue(TemplateWriterWriteInfo info)
    {
        _writerInfoList.Add(info);
        _isConstant = false;
    }

    bool TryAddConstantValue(object value, string? format, IFormatProvider? provider)
    {
        if (value is IFormattable formattableValue)
        {
            if (provider is null)
            {
                return false;
            }

            AddConstantString(formattableValue.ToString(format, provider));
            return true;
        }

        if (value is string valueString)
        {
            if (valueString.Length != 0)
            {
                AddConstantString(valueString);
            }

            return true;
        }

        // nullの場合はWriteConstantLiteralメソッドの呼び出しは不要。
        if (value is null)
        {
            return true;
        }

        AddConstantString(value.ToString());
        return true;
    }
}
