namespace SimpleTextTemplate.Generator.Tests.Core;

public sealed record ByteArrayContextTestData
{
    public static readonly byte[] BytesStaticField = "_BytesStaticField"u8.ToArray();
    public readonly byte[] BytesField = "_BytesField"u8.ToArray();

    public static ReadOnlySpan<byte> BytesSpanStaticProperty => "_BytesSpanStaticProperty"u8;

    public ReadOnlySpan<byte> BytesSpanProperty => "_BytesSpanProperty"u8;

    public static byte[] BytesStaticProperty => "_BytesStaticProperty"u8.ToArray();

    public byte[] BytesProperty => "_BytesProperty"u8.ToArray();
}
