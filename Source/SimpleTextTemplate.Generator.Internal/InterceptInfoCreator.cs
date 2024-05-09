using System.Diagnostics;
using System.Globalization;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using SimpleTextTemplate.Generator.Extensions;
using SimpleTextTemplate.Generator.Specs;
using static Microsoft.CodeAnalysis.SymbolDisplayFormat;
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
    readonly CancellationToken _cancellationToken;

    readonly IInvocationOperation _operation;
    readonly InvocationExpressionSyntax _invocationExpression;

    readonly SeparatedSyntaxList<ArgumentSyntax> _arguments;
    readonly ExpressionSyntax _templateArgument;
    readonly ExpressionSyntax? _contextArgument;

    readonly InterceptsLocationInfo _interceptsLocationInfo;
    readonly INamedTypeSymbol _writerType;
    readonly ITypeSymbol? _contextType;

    readonly List<TemplateWriterWriteInfo> _writerInfoList = [];
    readonly List<Diagnostic> _diagnostics = [];

    readonly INamedTypeSymbol _readOnlySpanByteSymbol;
    readonly INamedTypeSymbol _readOnlySpanCharSymbol;

    bool _success;
    bool _isConstant;

    /// <summary>
    /// <see cref="InterceptInfoCreator"/>構造体の新しいインスタンスを取得します。
    /// </summary>
    /// <param name="context">コンテキスト</param>
    /// <param name="operation">メソッド呼び出し</param>
    /// <param name="cancellationToken">キャンセル要求を行うためのトークン</param>
    /// <exception cref="ArgumentNullException"><paramref name="operation"/>がnullです。</exception>
    /// <exception cref="ArgumentException"><see cref="SyntaxNode"/>がインターセプターの対象外です。</exception>
    public InterceptInfoCreator(GeneratorSyntaxContext context, IInvocationOperation operation, CancellationToken cancellationToken)
    {
        _context = context;
        _semanticModel = _context.SemanticModel;
        _compilation = _semanticModel.Compilation;
        _cancellationToken = cancellationToken;

        _operation = operation ?? throw new ArgumentNullException(nameof(operation));

        _invocationExpression = _context.Node as InvocationExpressionSyntax
            ?? throw new ArgumentException($"{nameof(context)}.{nameof(context.Node)}が{nameof(InvocationExpressionSyntax)}ではありません。");

        _arguments = _invocationExpression.ArgumentList.Arguments;

        if (_arguments.Count is < 1 or > 3)
        {
            throw new ArgumentException("TemplateWriter.Writeのコンテキスト指定オーバーロードの引数は、1～3個である必要があります。");
        }

        _templateArgument = _arguments[0].Expression;

        if (_arguments.Count > 1)
        {
            _contextArgument = _arguments[1].Expression;
            _contextType = _semanticModel.GetTypeInfo(_contextArgument, cancellationToken).Type;
        }

        _interceptsLocationInfo = _operation.GetInterceptsLocationInfo(cancellationToken);
        _writerType = _semanticModel.GetSymbolInfo(_invocationExpression.Expression, _cancellationToken).Symbol!.ContainingType;

        _readOnlySpanByteSymbol = _compilation.GetReadOnlySpanTypeSymbol(SpecialType.System_Byte);
        _readOnlySpanCharSymbol = _compilation.GetReadOnlySpanTypeSymbol(SpecialType.System_Char);
    }

    /// <summary>
    /// <see cref="InterceptInfo"/>を取得します。
    /// </summary>
    public readonly InterceptInfo? Intercept
        => _success
        ? new(_interceptsLocationInfo, [.. _writerInfoList], _writerType.ToDisplayString(FullyQualifiedFormat), _contextType?.ToDisplayString(FullyQualifiedFormat))
        : null;

    /// <summary>
    /// <see cref="Diagnostic"/>のリストを取得します。
    /// </summary>
    public readonly IReadOnlyList<Diagnostic> Diagnostics => [.. _diagnostics];

    /// <summary>
    /// テンプレート文字列を解析します。
    /// </summary>
    public void Parse()
    {
        if (_semanticModel.GetConstantValue(_templateArgument, _cancellationToken).Value is not string templateString)
        {
            // string.Emptyは定数として扱う。
            if (_semanticModel.GetOperation(_templateArgument, _cancellationToken) is not IFieldReferenceOperation { Field: { Name: nameof(string.Empty), ContainingType.SpecialType: SpecialType.System_String } })
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

        if (_contextArgument is null)
        {
            AnalyzeTemplate(in template);
            return;
        }

        AnalyzeTemplateWithContext(in template);
    }

    readonly bool IsInvariantCulture()
    {
        // IFormatProvider指定なしの場合、InvariantCultureとする。
        if (_arguments.Count == 2)
        {
            return true;
        }

        var providerExpression = _arguments[2].Expression;

        // IFormatProviderがnullの場合、InvariantCultureとする。
        if (_semanticModel.GetConstantValue(providerExpression, _cancellationToken) is { HasValue: true, Value: var constantProvider } && constantProvider is null)
        {
            return true;
        }

        if (providerExpression is not MemberAccessExpressionSyntax { Name.Identifier.ValueText: "InvariantCulture" } providerArgument)
        {
            return false;
        }

        var symbolInfo = _semanticModel.GetSymbolInfo(providerArgument, _cancellationToken);
        var providerTypeName = symbolInfo.Symbol?.ContainingType.ToDisplayString(FullyQualifiedFormat);

        return providerTypeName == "global::System.Globalization.CultureInfo";
    }

    void AnalyzeTemplate(in Template template)
    {
        foreach (var (block, utf8Value, _, _) in template.Blocks)
        {
            _cancellationToken.ThrowIfCancellationRequested();
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

    void AnalyzeTemplateWithContext(in Template template)
    {
        if (_contextArgument is null || _contextType is null)
        {
            throw new InvalidOperationException("コンテキストの型情報が取得できません。");
        }

        var isInvariantCulture = IsInvariantCulture();
        var contextMembers = _contextType.GetFieldsAndProperties().ToDictionary(static x => x.Name);

        foreach (var (block, utf8Value, format, identifierProvider) in template.Blocks)
        {
            _cancellationToken.ThrowIfCancellationRequested();

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
                _diagnostics.Add(Diagnostic.Create(DiagnosticDescriptors.InvalidIdentifier, _contextArgument.GetLocation(), value));
                return;
            }

            // 識別子が定数かつIFormatProviderの指定がある場合は、定数書き込み
            if (identifier is IFieldSymbol { HasConstantValue: true, ConstantValue: var constantValue } && TryAddConstantValue(constantValue, format, provider))
            {
                continue;
            }

            var type = identifier.GetMemberType();

            // 識別子の型をReadOnlySpan<byte>に暗黙的変換できるどうか
            // ReadOnlySpan<byte>やbyte[]などが一致
            if (_compilation.ClassifyConversion(type, _readOnlySpanByteSymbol).IsImplicit)
            {
                AddValue(new(identifier.IsStatic ? WriteStaticLiteral : WriteLiteral, value, format, provider));
                continue;
            }

            // 識別子の型をReadOnlySpan<char>に暗黙的変換できるどうか
            // ReadOnlySpan<char>やstring、char[]などが一致
            if (_compilation.ClassifyConversion(type, _readOnlySpanCharSymbol).IsImplicit)
            {
                AddValue(new(identifier.IsStatic ? WriteStaticString : WriteString, value, format, provider));
                continue;
            }

            if (type.TypeKind == TypeKind.Enum)
            {
                AddValue(new(identifier.IsStatic ? WriteStaticEnum : WriteEnum, value, format, provider));
                continue;
            }

            AddValue(new(identifier.IsStatic ? WriteStaticValue : WriteValue, value, format, provider));
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
        if (value is string valueString)
        {
            AddConstantString(valueString);
            return true;
        }

        if (value is IFormattable formattableValue)
        {
            if (provider is null)
            {
                return false;
            }

            AddConstantString(formattableValue.ToString(format, provider));
            return true;
        }

        AddConstantString(value.ToString());
        return true;
    }
}
