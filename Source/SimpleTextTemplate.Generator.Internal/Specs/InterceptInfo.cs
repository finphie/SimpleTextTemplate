namespace SimpleTextTemplate.Generator.Specs;

/// <summary>
/// インターセプターに必要な情報を格納するクラスです。
/// </summary>
/// <param name="LocationInfo">InterceptsLocation属性の情報</param>
/// <param name="WriteInfoList">Writeメソッドの種類</param>
/// <param name="WriterTypeName">TemplateWriterの型名</param>
/// <param name="ContextTypeName">コンテキストの型名</param>
sealed record InterceptInfo(InterceptsLocationInfo LocationInfo, IReadOnlyList<TemplateWriterWriteInfo> WriteInfoList, string WriterTypeName, string? ContextTypeName);
