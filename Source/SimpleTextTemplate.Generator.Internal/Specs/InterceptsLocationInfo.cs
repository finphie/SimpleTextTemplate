namespace SimpleTextTemplate.Generator.Specs;

/// <summary>
/// InterceptsLocation属性の情報を格納するクラスです。
/// </summary>
/// <param name="FilePath">ファイルパス</param>
/// <param name="Line">メソッド名がある行</param>
/// <param name="Column">メソッド名の開始位置</param>
sealed record InterceptsLocationInfo(string FilePath, int Line, int Column);
