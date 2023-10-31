#if NET8_0_OR_GREATER
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SimpleTextTemplate.Extensions;

/// <summary>
/// <see cref="IBufferWriter{T}"/>関連の拡張メソッド集です。
/// </summary>
static class IBufferWriterExtensions
{
    /// <summary>
    /// 指定されたバッファーライターにデータを書き込みます。
    /// </summary>
    /// <typeparam name="T">使用するバッファーライターの型</typeparam>
    /// <param name="bufferWriter">バッファーライター</param>
    /// <param name="source">データ</param>
    /// <param name="length">長さ</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write<T>(this T bufferWriter, scoped ref readonly byte source, int length)
        where T : notnull, IBufferWriter<byte>
    {
        var span = bufferWriter.GetSpan(length);

        Unsafe.CopyBlockUnaligned(ref MemoryMarshal.GetReference(span), ref Unsafe.AsRef(in source), (uint)length);
        bufferWriter.Advance(length);
    }
}
#endif
