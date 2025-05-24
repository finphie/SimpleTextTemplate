namespace SimpleTextTemplate.Generator.Tests.Core;

public sealed record NullContextTestData
{
    public const string? NullStringConstantField = null;
    public const object? NullObjectConstantField = null;

    public static readonly byte[]? NullBytesStaticField;
    public static readonly char[]? NullCharsStaticField;
    public static readonly string? NullStringStaticField;
    public static readonly object? NullObjectStaticField;

    public readonly byte[]? NullBytesField;
    public readonly char[]? NullCharsField;
    public readonly string? NullStringField;
    public readonly object? NullObjectField;

    public static byte[]? NullBytesStaticProperty => null;

    public static char[]? NullCharsStaticProperty => null;

    public static string? NullStringStaticProperty => null;

    public static object? NullObjectStaticProperty => null;

    public byte[]? NullBytesProperty => null;

    public char[]? NullCharsProperty => null;

    public string? NullStringProperty => null;

    public object? NullObjectProperty => null;
}
