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
    static readonly string GeneratorName = $"global::{GeneratorType.FullName}";
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

        // トップレベルステートメントでは、名前空間の設定は不要。
        NamespaceText = _symbol.ContainingNamespace.CanBeReferencedByName
            ? $"namespace {_symbol.ContainingNamespace.ToDisplayString()};"
            : string.Empty;

        var type = _symbol.ContainingType;

#pragma warning disable IDE0072 // 欠落しているケースの追加
        ClassOrStructText = (type.IsReferenceType, type.IsRecord) switch
        {
            (true, true) => RecordText,
            (true, false) => ClassText,
            (false, true) => RecordStructText,
            (false, false) => StructText
        };
#pragma warning restore IDE0072 // 欠落しているケースの追加

        ClassName = _symbol.ContainingType.Name;
        MethodAccessibilityText = SyntaxFacts.GetText(_symbol.DeclaredAccessibility);
        MethodName = _symbol.Name;

        var parameters = _symbol.Parameters;
        BufferWriterParameterName = parameters.Length == 0 ? string.Empty : parameters[0].Name;

        var contextParameter = parameters.Length == 2 ? parameters[1] : null;
        ContextTypeName = contextParameter is null
            ? string.Empty
            : contextParameter.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        ContextParameterName = contextParameter?.Name ?? string.Empty;
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
    /// 名前空間を設定する文を取得します。
    /// </summary>
    /// <value>
    /// トップレベルステートメントでは空文字列、
    /// それ以外では"<see langword="namespace"/> <![CDATA[<名前空間名>;"]]>を返します。
    /// </value>
    public string NamespaceText { get; }

    /// <summary>
    /// クラスや構造体、レコードのキーワード名を取得します。
    /// </summary>
    /// <value>"class"または"struct"、"record"、"record struct"のいずれか</value>
    public string ClassOrStructText { get; }

    /// <summary>
    /// クラス名を取得します。
    /// </summary>
    /// <value>クラス名</value>
    public string ClassName { get; private set; }

    /// <summary>
    /// アクセシビリティ名を取得します。
    /// </summary>
    /// <value>"public"や"private"などのアクセシビリティ名</value>
    public string MethodAccessibilityText { get; }

    /// <summary>
    /// メソッド名を取得します。
    /// </summary>
    /// <value>
    /// メソッド名
    /// </value>
    public string MethodName { get; }

    /// <summary>
    /// <![CDATA[IBufferWriter<byte>]]>型が設定されている引数の名前を取得します。
    /// </summary>
    /// <value>
    /// 引数名
    /// </value>
    public string BufferWriterParameterName { get; }

    /// <summary>
    /// コンテキストの型名を取得します。
    /// </summary>
    /// <value>
    /// コンテキストの型名
    /// </value>
    public string ContextTypeName { get; }

    /// <summary>
    /// コンテキストの引数名を取得します。
    /// </summary>
    /// <value>
    /// コンテキストの引数名
    /// </value>
    public string ContextParameterName { get; }

    ReadOnlySpan<(BlockType Type, TextRange Range)> Blocks => _template.Blocks;

    /// <summary>
    /// ソースコードを解析します。
    /// </summary>
    /// <returns>解析結果</returns>
    public ParseResult TryParse()
    {
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

    string GetArrayText(TextRange range)
        => string.Join(", ", _source.AsSpan(range.Start, range.Length).ToArray());

    string GetText(TextRange range)
        => Encoding.UTF8.GetString(_source, range.Start, range.Length);
}
