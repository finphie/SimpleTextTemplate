using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace SimpleTextTemplate.Generator.Specs;

/// <summary>
/// インターセプターに必要な情報を格納するクラスです。
/// </summary>
/// <param name="LocationInfo">InterceptsLocation属性の情報</param>
/// <param name="WriteInfoList">Writeメソッドの種類</param>
/// <param name="GrowInfoList">Growメソッドの種類</param>
/// <param name="MethodSymbol">メソッドシンボル</param>
#pragma warning disable RSEXPERIMENTAL002 // 種類は、評価の目的でのみ提供されています。将来の更新で変更または削除されることがあります。続行するには、この診断を非表示にします。
sealed record InterceptInfo(
    InterceptableLocation LocationInfo,
    IReadOnlyList<TemplateWriterWriteInfo> WriteInfoList,
    IReadOnlyDictionary<int, TemplateWriterGrowInfo> GrowInfoList,
    IMethodSymbol MethodSymbol);
#pragma warning restore RSEXPERIMENTAL002 // 種類は、評価の目的でのみ提供されています。将来の更新で変更または削除されることがあります。続行するには、この診断を非表示にします。
