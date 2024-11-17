using System.Buffers;
using System.Runtime.CompilerServices;

namespace SimpleTextTemplate;

/// <summary>
/// <see cref="TemplateWriter{T}"/>構造体関連のクラスです。
/// </summary>
public static class TemplateWriter
{
    /// <summary>
    /// <see cref="TemplateWriter{T}"/>構造体の新しいインスタンスを作成します。
    /// </summary>
    /// <typeparam name="T">バッファライターの型</typeparam>
    /// <param name="bufferWriter">バッファライター</param>
    /// <returns><see cref="IBufferWriter{T}"/>に書き込みを行う、<see cref="TemplateWriter{T}"/>構造体のインスタンスを返します。</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TemplateWriter<T> Create<T>(T bufferWriter)
        where T : notnull, IBufferWriter<byte>
        => new(bufferWriter);
}
