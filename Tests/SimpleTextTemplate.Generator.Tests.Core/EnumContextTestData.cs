namespace SimpleTextTemplate.Generator.Tests.Core;

public sealed record EnumContextTestData
{
    public static readonly EnumTestData EnumStaticField = EnumTestData.A;
    public readonly EnumTestData EnumField = EnumTestData.A;

    public static EnumTestData EnumStaticProperty => EnumTestData.A;

    public EnumTestData EnumProperty => EnumTestData.A;
}
