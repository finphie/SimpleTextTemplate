namespace SimpleTextTemplate.Generator.Specs;

/// <summary>
/// <see cref="Enum"/>関連の情報を格納するクラスです。
/// </summary>
/// <param name="ConstantValueToNameTable">定数値からメンバー名への変換テーブル</param>
/// <param name="IsFlag"><see cref="FlagsAttribute"/>属性が設定されているか</param>
sealed record EnumInfo(
    IReadOnlyDictionary<object, string> ConstantValueToNameTable,
    bool IsFlag);
