namespace SimpleTextTemplate.Generator.Tests.Core;

public sealed record DoubleContextTestData
{
    public const double DoubleConstantField = 1234.567;

    public static readonly double DoubleStaticField = 2345.678;
    public readonly double DoubleField = 3456.789;

    public static double DoubleStaticProperty => 4567.891;

    public double DoubleProperty => 5678.912;
}
