namespace SimpleTextTemplate.Generator.Tests.Core;

public sealed record DateTimeOffsetContextTestData
{
    public static readonly DateTimeOffset DateTimeOffsetStaticField = new(2000, 1, 1, 0, 0, 0, TimeSpan.FromHours(9));
    public readonly DateTimeOffset DateTimeOffsetField = new(2000, 1, 1, 0, 0, 0, TimeSpan.FromHours(9));

    public static DateTimeOffset DateTimeOffsetStaticProperty => new(2000, 1, 1, 0, 0, 0, TimeSpan.FromHours(9));

    public DateTimeOffset DateTimeOffsetProperty => new(2000, 1, 1, 0, 0, 0, TimeSpan.FromHours(9));
}
