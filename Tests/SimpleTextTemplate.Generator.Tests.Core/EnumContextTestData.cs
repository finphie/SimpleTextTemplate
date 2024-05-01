namespace SimpleTextTemplate.Generator.Tests.Core;

public sealed record EnumContextTestData
{
    public static readonly EnumTestData EnumStaticField = EnumTestData.A;
    public readonly EnumTestData EnumField = EnumTestData.B;

    public static EnumTestData EnumStaticProperty => EnumTestData.C;

    public EnumTestData EnumProperty => EnumTestData.D;
}
