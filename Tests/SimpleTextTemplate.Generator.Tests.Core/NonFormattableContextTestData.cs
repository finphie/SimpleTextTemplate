namespace SimpleTextTemplate.Generator.Tests.Core;

public sealed record NonFormattableContextTestData
{
    public static readonly NonFormattable NonFormattableStaticField = new("_NonFormattableStaticField");
    public readonly NonFormattable NonFormattableField = new("_NonFormattableField");

    public static NonFormattable NonFormattableStaticProperty => new("_NonFormattableStaticProperty");

    public NonFormattable NonFormattableProperty => new("_NonFormattableProperty");
}
