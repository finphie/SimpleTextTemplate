namespace SimpleTextTemplate.Generator.Tests.Core;

public sealed record EmptyContextTestData
{
    public const string EmptyStringConstantField = "";

    public static readonly string EmptyStringStaticField = string.Empty;

    public readonly string EmptyStringField = string.Empty;

    public static string EmptyStringStaticProperty => string.Empty;

    public string EmptyStringProperty => string.Empty;
}
