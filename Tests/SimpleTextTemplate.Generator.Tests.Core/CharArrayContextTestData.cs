namespace SimpleTextTemplate.Generator.Tests.Core;

public sealed record CharArrayContextTestData
{
    public static readonly char[] CharsStaticField = [.. "_CharsStaticField"];
    public readonly char[] CharsField = [.. "_CharsField"];

    public static ReadOnlySpan<char> CharsSpanStaticProperty => "_CharsSpanStaticProperty";

    public ReadOnlySpan<char> CharsSpanProperty => "_CharsSpanProperty";

    public static char[] CharsStaticProperty => [.. "_CharsStaticProperty"];

    public char[] CharsProperty => [.. "_CharsProperty"];
}
