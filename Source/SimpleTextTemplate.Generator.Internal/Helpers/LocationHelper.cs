using Microsoft.CodeAnalysis;

namespace SimpleTextTemplate.Generator.Helpers;

/// <summary>
/// ソースコードの位置に関するヘルパークラスです。
/// </summary>
static class LocationHelper
{
    /// <summary>
    /// 指定されたメソッドのソースコードでの位置を取得します。
    /// </summary>
    /// <param name="symbol">メソッドシンボル</param>
    /// <returns>ソースコードでの位置</returns>
    public static Location GetLocation(IMethodSymbol symbol)
    {
        var node = symbol.DeclaringSyntaxReferences[0].GetSyntax();
        var location = Location.Create(node.SyntaxTree, new(node.SpanStart, default));

        return location;
    }
}
