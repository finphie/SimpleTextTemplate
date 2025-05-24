namespace SimpleTextTemplate.Generator.Tests.Core;

public sealed record ContextTestData
{
    public const string ConstantValue = "_ConstantValue";
    public static readonly string StringValue = "_StringValue";
    public static readonly double DoubleValue = 1.0;

    public static ReadOnlySpan<byte> Utf8Value => "_Utf8Value"u8;

    public ReadOnlySpan<char> Utf16Value => "_Utf16Value";
}
