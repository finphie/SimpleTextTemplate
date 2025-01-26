namespace SimpleTextTemplate.Generator.Tests.Core;

public readonly ref struct RefStructTestData()
{
    public static readonly byte[] BytesStaticField = "_BytesStaticField"u8.ToArray();
    public readonly byte[] BytesField = "_BytesField"u8.ToArray();
    public readonly ReadOnlySpan<byte> BytesSpanField = "_BytesSpanField"u8;

    public static byte[] BytesStaticProperty => "_BytesStaticProperty"u8.ToArray();

    public byte[] BytesProperty => "_BytesProperty"u8.ToArray();
}
