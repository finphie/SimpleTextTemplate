#if NET9_0_OR_GREATER
using System.IO.Hashing;
using System.Runtime.InteropServices;

namespace SimpleTextTemplate;

/// <summary>
/// UTF-8エンコードされたバイト配列の比較を提供します。
/// </summary>
sealed class Utf8StringEqualityComparer : IEqualityComparer<byte[]>, IAlternateEqualityComparer<ReadOnlySpan<byte>, byte[]>
{
    /// <summary>
    /// <see cref="Utf8StringEqualityComparer"/>クラスのデフォルトインスタンスを取得します。
    /// </summary>
    public static IEqualityComparer<byte[]> Default { get; } = new Utf8StringEqualityComparer();

    /// <inheritdoc/>
    public bool Equals(byte[]? x, byte[]? y)
    {
        return (x, y) switch
        {
            (null, null) => true,
            (null, _) or (_, null) => false,
            _ => MemoryMarshal.CreateReadOnlySpan(ref MemoryMarshal.GetArrayDataReference(x), x.Length)
                .SequenceEqual(MemoryMarshal.CreateReadOnlySpan(ref MemoryMarshal.GetArrayDataReference(y), y.Length))
        };
    }

    /// <inheritdoc/>
    public bool Equals(ReadOnlySpan<byte> alternate, byte[] other)
        => other.AsSpan().SequenceEqual(alternate);

    /// <inheritdoc/>
    public int GetHashCode(byte[] obj)
        => GetHashCode(obj.AsSpan());

    /// <inheritdoc/>
    public int GetHashCode(ReadOnlySpan<byte> alternate)
        => (int)XxHash3.HashToUInt64(alternate);

    /// <inheritdoc/>
    public byte[] Create(ReadOnlySpan<byte> alternate)
        => alternate.ToArray();
}
#endif
