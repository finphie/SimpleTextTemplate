namespace SimpleTextTemplate.Generator.Tests.Core;

public sealed record NullContextTestData
{
    public const string? NullStringConstantField = null;
    public const object? NullObjectConstantField = null;
    public const string EmptyStringConstantField = "";

    public static readonly string? NullStringStaticField;
    public static readonly object? NullObjectStaticField;
}
