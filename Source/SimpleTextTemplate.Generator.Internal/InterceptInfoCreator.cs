using System.Collections.Frozen;
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
    static readonly string FlagsAttributeFullName = typeof(FlagsAttribute).FullName;

    readonly GeneratorSyntaxContext _context;
    readonly SemanticModel _semanticModel;
    readonly Compilation _compilation;
    readonly IMethodSymbol _methodSymbol;

    readonly InvocationExpressionSyntax _invocationExpression;

    readonly ExpressionSyntax _templateArgument;
    readonly ExpressionSyntax? _contextArgument;
    readonly ExpressionSyntax? _providerArgument;

    readonly bool _isConstantTemplateString;
    readonly Dictionary<ITypeSymbol, EnumInfo> _enumList = new(SymbolEqualityComparer.Default);

    readonly List<TemplateWriterWriteInfo> _writeInfoList = [];
    readonly Dictionary<int, TemplateWriterGrowInfo> _growInfoList = [];
    readonly List<Diagnostic> _diagnostics = [];

    readonly INamedTypeSymbol _readOnlySpanByteSymbol;
    readonly INamedTypeSymbol _readOnlySpanCharSymbol;
    readonly INamedTypeSymbol _flagsAttributeSymbol;

#pragma warning disable RSEXPERIMENTAL002 // 種類は、評価の目的でのみ提供されています。将来の更新で変更または削除されることがあります。続行するには、この診断を非表示にします。
    InterceptableLocation? _interceptableLocation;
#pragma warning restore RSEXPERIMENTAL002 // 種類は、評価の目的でのみ提供されています。将来の更新で変更または削除されることがあります。続行するには、この診断を非表示にします。

    bool _isInvariantCulture;
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

        var arguments = _invocationExpression.ArgumentList.Arguments;

        if (arguments.Count is < TemplateRenderParameterCount or > TemplateRenderParameterCountWithContext)
        {
            throw new ArgumentException("Template.Renderの引数は、2～4個である必要があります。");
        }

        _templateArgument = arguments[TemplateRenderTextIndex].Expression;
        _contextArgument = arguments.ElementAtOrDefault(TemplateRenderContextIndex)?.Expression;
        _providerArgument = arguments.ElementAtOrDefault(TemplateRenderIFormatProviderIndex)?.Expression;

        _isConstantTemplateString = arguments.Count == TemplateRenderParameterCount;

        _readOnlySpanByteSymbol = _compilation.GetReadOnlySpanTypeSymbol(SpecialType.System_Byte);
        _readOnlySpanCharSymbol = _compilation.GetReadOnlySpanTypeSymbol(SpecialType.System_Char);
        _flagsAttributeSymbol = _compilation.GetTypeByMetadataName(FlagsAttributeFullName)
            ?? throw new InvalidOperationException("Type System.FlagsAttribute is not found.");
    }

    /// <summary>
    /// <see cref="InterceptInfo"/>を取得します。
    /// </summary>
    public readonly InterceptInfo? Intercept
        => _success && _interceptableLocation is not null
        ? new(_interceptableLocation, [.. _writeInfoList], _growInfoList, _methodSymbol)
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

        if (_isConstantTemplateString)
        {
            AnalyzeTemplate(in template, cancellationToken);
            return;
        }

        _isInvariantCulture = IsInvariantCulture(cancellationToken);

        AnalyzeTemplateWithContext(in template, cancellationToken);
        AnalyzeDangerousMethod(cancellationToken);
    }

    readonly bool IsInvariantCulture(CancellationToken cancellationToken)
    {
        // provider指定なしの場合、InvariantCultureとする。
        if (_providerArgument is null)
        {
            return true;
        }

        // providerがnullの場合、InvariantCultureとする。
        if (_semanticModel.GetConstantValue(_providerArgument, cancellationToken) is { HasValue: true, Value: null })
        {
            return true;
        }

        var operation = _semanticModel.GetOperation(_providerArgument, cancellationToken);

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

            _writeInfoList.Add(new(WriteConstantLiteral, value, MethodAnnotation.None));
        }

        _success = true;
    }

    void AnalyzeTemplateWithContext(in Template template, CancellationToken cancellationToken)
    {
        if (_contextArgument is null)
        {
            throw new InvalidOperationException("コンテキストが取得できません。");
        }

        var contextType = _semanticModel.GetTypeInfo(_contextArgument, cancellationToken).Type
            ?? throw new InvalidOperationException("コンテキストの型情報が取得できません。");

        var contextMembers = contextType.GetFieldsAndProperties().ToDictionary(
            static x => x.Name,
            static x => (Symbol: x, Formattable: x.GetFieldOrPropertyType().Interfaces.GetFormattableType()));

        foreach (var (block, utf8Value, format, identifierProvider) in template.Blocks)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var value = Encoding.UTF8.GetString(utf8Value);

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
                continue;
            }

            AddIdentifier(identifier.Symbol, identifier.Formattable, value, format, identifierProvider);
        }

        _success = true;
    }

    void AddIdentifier(ISymbol symbol, FormattableType formattableType, string value, string? format, IFormatProvider? identifierProvider)
    {
        // 識別子が定数の場合
        if (TryAddConstantIdentifier(symbol, formattableType, format, identifierProvider))
        {
            return;
        }

        var provider = identifierProvider.GetFormatProvider(_isInvariantCulture);

        // 識別子がenumの場合
        if (TryAddEnumIdentifier(symbol, value, format, identifierProvider))
        {
            return;
        }

        // IFormattable/ISpanFormattable/IUtf8Formattableを実装していない識別子で、formatまたはproviderが設定されている場合
        if (formattableType == FormattableType.None && (format is not null || identifierProvider is not null))
        {
            _diagnostics.Add(Diagnostic.Create(DiagnosticDescriptors.InvalidIdentifierFormatProvider, _templateArgument.GetLocation()));
        }

        // 識別子がbyte配列の場合
        if (TryAddLiteralIdentifier(symbol, value, format, identifierProvider))
        {
            return;
        }

        // 識別子がchar配列の場合
        if (TryAddStringIdentifier(symbol, value, format, identifierProvider))
        {
            return;
        }

        var annotation = symbol.IsStatic ? MethodAnnotation.Static : MethodAnnotation.None;
        AddValue(new(WriteValue, value, annotation, format, provider));
    }

    bool TryAddConstantIdentifier(ISymbol symbol, FormattableType formattableType, string? format, IFormatProvider? identifierProvider)
    {
        // 識別子が定数かどうか
        if (symbol is not IFieldSymbol { HasConstantValue: true, ConstantValue: var constantValue } fieldSymbol)
        {
            return false;
        }

        var provider = identifierProvider.GetFormatProvider(_isInvariantCulture);

        if (fieldSymbol.Type.TypeKind != TypeKind.Enum)
        {
            // IFormattableを実装していない識別子で、formatまたはproviderが設定されている場合
            if (!formattableType.HasFlag(FormattableType.IFormattable) && (format is not null || identifierProvider is not null))
            {
                _diagnostics.Add(Diagnostic.Create(DiagnosticDescriptors.InvalidConstantIdentifierFormatProvider, _templateArgument.GetLocation()));
            }

            // IFormattableを実装している識別子で、IFormatProviderの指定がある場合のみ定数書き込み
            return TryAddConstantValue(constantValue, format, provider);
        }

        // FormatにD以外を指定している場合は定数展開しない
        if (format is not (null or "D" or "d"))
        {
            return false;
        }

        var enumType = fieldSymbol.Type;

        // enum関連のキャッシュがない場合は作成する
        if (!_enumList.TryGetValue(enumType, out var enumInfo))
        {
            // enumの各定数値から、該当する名前を取得するためのテーブルを作成
            var constantValueToNameTable = fieldSymbol.Type.GetMembers()
                .OfType<IFieldSymbol>()
                .Where(x => x.HasConstantValue)
                .ToFrozenDictionary(x => x.ConstantValue!, x => x.Name);

            // 属性を取得
            var enumAttributes = fieldSymbol.Type.GetAttributes();
            var flagsAttributeSymbol = _flagsAttributeSymbol;

            // Flags属性が設定されているか
            var isFlag = enumAttributes.Any(x => SymbolEqualityComparer.Default.Equals(x.AttributeClass, flagsAttributeSymbol));

            enumInfo = new(constantValueToNameTable, isFlag);
            _enumList.Add(enumType, enumInfo);
        }

        // Flags属性が設定されている場合、定数展開しない
        if (enumInfo.IsFlag)
        {
            return false;
        }

        // Format指定がない場合、メンバー名を定数値とする
        if (format is null && enumInfo.ConstantValueToNameTable.TryGetValue(constantValue, out var enumMemberName))
        {
            constantValue = enumMemberName;
        }

        return TryAddConstantValue(constantValue, null, provider);
    }

    bool TryAddEnumIdentifier(ISymbol symbol, string value, string? format, IFormatProvider? identifierProvider)
    {
        var type = symbol.GetFieldOrPropertyType();

        if (type.TypeKind != TypeKind.Enum)
        {
            return false;
        }

        if (identifierProvider is not null)
        {
            _diagnostics.Add(Diagnostic.Create(DiagnosticDescriptors.InvalidEnumIdentifierFormatProvider, _templateArgument.GetLocation()));
        }

        var annotation = symbol.IsStatic ? MethodAnnotation.Static : MethodAnnotation.None;
        var provider = identifierProvider.GetFormatProvider(_isInvariantCulture);

        AddValue(new(WriteEnum, value, annotation, format, provider));
        return true;
    }

    bool TryAddLiteralIdentifier(ISymbol symbol, string value, string? format, IFormatProvider? identifierProvider)
    {
        var type = symbol.GetFieldOrPropertyType();

        // 識別子の型をReadOnlySpan<byte>に暗黙的変換できるどうか
        // ReadOnlySpan<byte>やbyte[]などが一致
        if (!_compilation.ClassifyConversion(type, _readOnlySpanByteSymbol).IsImplicit)
        {
            return false;
        }

        var annotation = symbol.IsStatic ? MethodAnnotation.Static : MethodAnnotation.None;
        var provider = identifierProvider.GetFormatProvider(_isInvariantCulture);

        if (SymbolEqualityComparer.Default.Equals(type, _readOnlySpanByteSymbol))
        {
            annotation |= MethodAnnotation.Dangerous;
        }

        AddValue(new(WriteLiteral, value, annotation, format, provider));
        return true;
    }

    bool TryAddStringIdentifier(ISymbol symbol, string value, string? format, IFormatProvider? identifierProvider)
    {
        var type = symbol.GetFieldOrPropertyType();

        // 識別子の型をReadOnlySpan<char>に暗黙的変換できるどうか
        // ReadOnlySpan<char>やstring、char[]などが一致
        if (!_compilation.ClassifyConversion(type, _readOnlySpanCharSymbol).IsImplicit)
        {
            return false;
        }

        var annotation = symbol.IsStatic ? MethodAnnotation.Static : MethodAnnotation.None;
        var provider = identifierProvider.GetFormatProvider(_isInvariantCulture);

        if (SymbolEqualityComparer.Default.Equals(type, _readOnlySpanCharSymbol))
        {
            annotation |= MethodAnnotation.Dangerous;
        }

        AddValue(new(WriteString, value, annotation, format, provider));
        return true;
    }

    void AddConstantString(string value)
    {
        if (_isConstant)
        {
            _writeInfoList[^1] = new(WriteConstantLiteral, _writeInfoList[^1].Value + value, MethodAnnotation.Dangerous);
            return;
        }

        _writeInfoList.Add(new(WriteConstantLiteral, value, MethodAnnotation.Dangerous));
        _isConstant = true;
    }

    void AddValue(TemplateWriterWriteInfo info)
    {
        _writeInfoList.Add(info);
        _isConstant = false;
    }

    bool TryAddConstantValue(object value, string? format, IFormatProvider? provider)
    {
        // nullの場合はWriteConstantLiteralメソッドの呼び出しは不要。
        if (value is null)
        {
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

        if (value is string valueString)
        {
            if (valueString.Length != 0)
            {
                AddConstantString(valueString);
            }

            return true;
        }

        AddConstantString(value.ToString());
        return true;
    }

    readonly void AnalyzeDangerousMethod(CancellationToken cancellationToken)
    {
        var index = 0;

        while (index < _writeInfoList.Count)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var baseIndex = index;
            var baseWriterInfo = _writeInfoList[index++];

            if (!baseWriterInfo.Annotation.HasFlag(MethodAnnotation.Dangerous))
            {
                continue;
            }

            var constantCount = 0;
            var members = new List<ContextMember>();

            SetGrowLength(baseWriterInfo, ref constantCount, members);

            while (index < _writeInfoList.Count)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var writerInfo = _writeInfoList[index++];

                if (writerInfo.WriteType != baseWriterInfo.WriteType)
                {
                    break;
                }

                SetGrowLength(writerInfo, ref constantCount, members);
            }

            _growInfoList.Add(baseIndex, new(constantCount, members));
        }

        static void SetGrowLength(TemplateWriterWriteInfo writeInfo, ref int constantCount, List<ContextMember> members)
        {
            if (writeInfo.WriteType == WriteConstantLiteral)
            {
                constantCount += writeInfo.Value.Length;
                return;
            }

            if (writeInfo.WriteType is WriteLiteral or WriteString)
            {
                members.Add(new(writeInfo.Value, writeInfo.WriteType, writeInfo.Annotation));
                return;
            }

            throw new InvalidOperationException("予期しないWriteTypeです。");
        }
    }
}
