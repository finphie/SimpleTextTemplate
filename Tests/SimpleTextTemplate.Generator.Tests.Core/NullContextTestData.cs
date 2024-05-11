namespace SimpleTextTemplate.Generator.Tests.Core;

public sealed record NullContextTestData
{
    public const string? NullStringConstantField = null;
    public const object? NullObjectConstantField = null;
    public const string EmptyStringConstantField = "";

    public static readonly string? NullStringStaticField;
    public static readonly object? NullObjectStaticField;
    public static readonly string EmptyStringStaticField = string.Empty;

    public readonly string? NullStringField;
    public readonly object? NullObjectField;

    public static string? NullStringStaticProperty => null;

    public static object? NullObjectStaticProperty => null;

    public static string? NullStringProperty => null;

    public static object? NullObjectProperty => null;
}
