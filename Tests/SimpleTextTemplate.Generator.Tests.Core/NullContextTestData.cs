namespace SimpleTextTemplate.Generator.Tests.Core;

public sealed record NullContextTestData
{
    public const string? NullStringConstantField = null;
    public const object? NullObjectConstantField = null;

    public static readonly string? NullStringStaticField;
    public static readonly object? NullObjectStaticField;

    public readonly string? NullStringField;
    public readonly object? NullObjectField;

    public static string? NullStringStaticProperty => null;

    public static object? NullObjectStaticProperty => null;

    public string? NullStringProperty => null;

    public object? NullObjectProperty => null;
}
