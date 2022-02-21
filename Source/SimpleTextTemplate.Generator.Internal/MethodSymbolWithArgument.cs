using Microsoft.CodeAnalysis;

namespace SimpleTextTemplate.Generator;

/// <summary>
/// メソッドシンボルと属性の引数データを表す構造体です。
/// </summary>
/// <param name="Symbol">メソッドシンボル</param>
/// <param name="Argument">属性の引数データ</param>
public readonly record struct MethodSymbolWithArgument(IMethodSymbol Symbol, string Argument);
