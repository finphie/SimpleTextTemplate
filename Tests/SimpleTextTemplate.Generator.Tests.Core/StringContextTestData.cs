namespace SimpleTextTemplate.Generator.Tests.Core;

public sealed record StringContextTestData
{
    public const string StringConstantField = "_StringConstantField";
    public const string StringConstantEmptyField = "";
    public const string StringConstantNullField = null;

    public static readonly string StringStaticField = "_StringStaticField";
    public readonly string StringField = "_StringField";

    public static string StringStaticProperty => "_StringStaticProperty";

    public string StringProperty => "_StringProperty";
}
