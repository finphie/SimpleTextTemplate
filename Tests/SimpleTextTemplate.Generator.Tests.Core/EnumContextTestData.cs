namespace SimpleTextTemplate.Generator.Tests.Core;

public sealed record EnumContextTestData
{
    public const EnumTestData EnumConstantField = EnumTestData.Test1;
    public const EnumTestData EnumConstantFieldInvalidNumber = (EnumTestData)99;
    public const FlagEnumTestData FlagEnumConstantField = FlagEnumTestData.Test1 | FlagEnumTestData.Test2;

    public static readonly EnumTestData EnumStaticField = EnumTestData.Test2;
    public readonly EnumTestData EnumField = EnumTestData.Test3;

    public static EnumTestData EnumStaticProperty => EnumTestData.Test4;

    public EnumTestData EnumProperty => EnumTestData.Test5;
}
