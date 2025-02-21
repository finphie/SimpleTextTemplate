namespace SimpleTextTemplate.Generator.Tests.Core;

public sealed record DoubleContextTestData
{
    public const double DoubleConstantField = 1234.567;

    public static readonly double DoubleStaticField = 1234.567;
    public readonly double DoubleField = 1234.567;

    public static double DoubleStaticProperty => 1234.567;

    public double DoubleProperty => 1234.567;
}
