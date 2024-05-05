namespace SimpleTextTemplate.Generator.Tests.Core;

public sealed record IntContextTestData
{
    public const int IntConstantField = 1234;

    public static readonly int IntStaticField = 1234;
    public readonly int IntField = 1234;

    public static int IntStaticProperty => 1234;

    public int IntProperty => 1234;
}
