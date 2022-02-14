using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace SimpleTextTemplate.Generator;

/// <summary>
/// ソースジェネレーターで出力するソースコードを生成するクラスです。
/// </summary>
partial class SourceCodeTemplate
{
    static readonly Type GeneratorType = typeof(TemplateGenerator);
    static readonly string GeneratorName = GeneratorType.FullName;
    static readonly string GeneratorVersion = GeneratorType.Assembly.GetName().Version.ToString();

    static readonly string ClassText = SyntaxFacts.GetText(SyntaxKind.ClassKeyword);
    static readonly string StructText = SyntaxFacts.GetText(SyntaxKind.StructKeyword);
    static readonly string RecordText = SyntaxFacts.GetText(SyntaxKind.RecordKeyword);
    static readonly string RecordStructText = RecordText + " " + StructText;

    readonly IMethodSymbol _symbol;
    readonly byte[] _source;

    Template _template;

    /// <summary>
    /// <see cref="SourceCodeTemplate"/>クラスの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="symbol">シンボル</param>
    /// <param name="utf8Source">UTF-8でエンコードされたテンプレート文字列</param>
    public SourceCodeTemplate(IMethodSymbol symbol, byte[] utf8Source)
    {
        _symbol = symbol;
        _source = utf8Source;
    }

    /// <summary>
    /// <see cref="SourceCodeTemplate"/>クラスの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="symbol">シンボル</param>
    /// <param name="source">テンプレート文字列</param>
    public SourceCodeTemplate(IMethodSymbol symbol, string source)
        : this(symbol, Encoding.UTF8.GetBytes(source))
    {
    }

    /// <summary>
    /// 名前空間を設定する文を取得、設定します。
    /// </summary>
    /// <value>
    /// トップレベルステートメントでは空文字列、
    /// それ以外では"<see langword="namespace"/> <![CDATA[<名前空間名>;"]]>を返します。
    /// </value>
    public string? NamespaceText { get; private set; }

    /// <summary>
    /// クラスや構造体、レコードのキーワード名を取得、設定します。
    /// </summary>
    /// <value>"class"または"struct"、"record"、"record struct"のいずれか</value>
    public string? ClassOrStructText { get; private set; }

    /// <summary>
    /// クラス名を取得、設定します。
    /// </summary>
    /// <value>クラス名</value>
    public string? ClassName { get; private set; }

    /// <summary>
    /// アクセシビリティ名を取得、設定します。
    /// </summary>
    /// <value>"public"や"private"などのアクセシビリティ名</value>
    public string? MethodAccessibilityText { get; private set; }

    /// <summary>
    /// メソッド名を取得、設定します。
    /// </summary>
    /// <value>
    /// メソッド名
    /// </value>
    public string? MethodName { get; private set; }

    /// <summary>
    /// <![CDATA[IBufferWriter<byte>]]>型が設定されている引数の名前を取得、設定します。
    /// </summary>
    /// <value>
    /// 引数名
    /// </value>
    public string? BufferWriterParameterName { get; private set; }

    /// <summary>
    /// コンテキストの型名を取得、設定します。
    /// </summary>
    /// <value>
    /// コンテキストの型名
    /// </value>
    public string? ContextTypeName { get; private set; }

    /// <summary>
    /// コンテキストの引数名を取得、設定します。
    /// </summary>
    /// <value>
    /// コンテキストの引数名
    /// </value>
    public string? ContextParameterName { get; private set; }

    /// <summary>
    /// ソースコードを解析します。
    /// </summary>
    /// <returns>解析結果</returns>
    [MemberNotNull(nameof(NamespaceText))]
    [MemberNotNull(nameof(ClassOrStructText))]
    [MemberNotNull(nameof(ClassName))]
    [MemberNotNull(nameof(MethodAccessibilityText))]
    [MemberNotNull(nameof(MethodName))]
    [MemberNotNull(nameof(BufferWriterParameterName))]
    [MemberNotNull(nameof(ContextTypeName))]
    [MemberNotNull(nameof(ContextParameterName))]
    public ParseResult TryParse()
    {
        NamespaceText = GetNamespaceText();
        ClassOrStructText = GetClassOrStructText();
        ClassName = GetClassName();
        MethodAccessibilityText = GetMethodAccessibilityText();
        MethodName = GetMethodName();
        BufferWriterParameterName = GetBufferWriterParameterName();
        ContextTypeName = GetContextTypeName();
        ContextParameterName = GetContextParameterName();

        _template = Template.Parse(_source);
        return IsValidIdentifier() ? ParseResult.Success : ParseResult.InvalidIdentifier;
    }

    bool IsValidIdentifier()
    {
        foreach (var (type, range) in _template.Blocks)
        {
            if (type != BlockType.Identifier)
            {
                continue;
            }

            if (!SyntaxFacts.IsValidIdentifier(Encoding.UTF8.GetString(_source, range.Start, range.Length)))
            {
                return false;
            }
        }

        return true;
    }

    string GetNamespaceText()
    {
        // トップレベルステートメントでは、名前空間の設定は不要。
        if (_symbol.ContainingNamespace.CanBeReferencedByName)
        {
            var result = _symbol.ContainingNamespace.ToDisplayString();
            return "namespace " + result + ";";
        }

        return string.Empty;
    }

    [SuppressMessage("Style", "IDE0046:条件式に変換します", Justification = "可読性のため")]
    string GetClassOrStructText()
    {
        var type = _symbol.ContainingType;

        if (type.IsReferenceType)
        {
            return type.IsRecord ? RecordText : ClassText;
        }

        return type.IsRecord ? RecordStructText : StructText;
    }

    string GetClassName() => _symbol.ContainingType.Name;

    string GetMethodAccessibilityText()
        => SyntaxFacts.GetText(_symbol.DeclaredAccessibility);

    string GetMethodName() => _symbol.Name;

    string GetBufferWriterParameterName()
    {
        var parameters = _symbol.Parameters;
        return parameters.Length == 0 ? string.Empty : parameters[0].Name;
    }

    string GetContextTypeName()
    {
        var contextParameter = GetContextParameter();

        if (contextParameter is null)
        {
            return string.Empty;
        }

        var contextTypeName = contextParameter.ToDisplayString();

        return contextTypeName;
    }

    string GetContextParameterName()
        => GetContextParameter()?.Name ?? string.Empty;

    IParameterSymbol? GetContextParameter()
    {
        var parameters = _symbol.Parameters;
        return parameters.Length == 2 ? parameters[1] : null;
    }

    ReadOnlySpan<(BlockType Type, TextRange Range)> GetBlocks() => _template.Blocks;

    string GetArrayText(TextRange range)
        => string.Join(", ", _source.AsSpan(range.Start, range.Length).ToArray());

    string GetText(TextRange range)
        => Encoding.UTF8.GetString(_source, range.Start, range.Length);
}
