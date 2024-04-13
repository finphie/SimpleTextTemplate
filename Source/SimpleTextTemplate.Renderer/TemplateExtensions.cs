using System.Buffers;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Utf8Utility;

namespace SimpleTextTemplate.Renderer;

public static class TemplateExtensions
{
    /// <summary>
    /// テンプレートをレンダリングして、バッファーライターに書き込みます。
    /// </summary>
    /// <typeparam name="TWriter">使用するバッファーライターの型</typeparam>
    /// <typeparam name="TContext">コンテキストの型</typeparam>
    /// <param name="bufferWriter">バッファーライター</param>
    /// <param name="context">コンテキスト</param>
    /// <exception cref="ArgumentNullException">引数がnullです。</exception>
    public static void Render<TWriter, TContext>(this in Template template, TWriter bufferWriter, TContext context)
        where TWriter : notnull, IBufferWriter<byte>
        where TContext : notnull, IContext
    {
        ArgumentNullException.ThrowIfNull(bufferWriter);
        ArgumentNullException.ThrowIfNull(context);

        using var writer = new TemplateWriter<TWriter>(ref bufferWriter);

        foreach (var (type, stringOrIdentifier, format, culture) in template.Blocks)
        {
            ref var stringOrIdentifierStart = ref MemoryMarshal.GetArrayDataReference(stringOrIdentifier);
            var span = MemoryMarshal.CreateReadOnlySpan(ref stringOrIdentifierStart, stringOrIdentifier.Length);

            switch (type)
            {
                case BlockType.Raw:
                    writer.WriteLiteral(span);
                    break;
                case BlockType.Identifier:
                    context.TryGetValue(span, out var value);

                    if (value is byte[] utf8Value)
                    {
                        writer.WriteLiteral(utf8Value);
                    }
                    else if (value is char[] utf16Value)
                    {
                        writer.WriteString(utf16Value);
                    }
                    else if (value is Utf8Array utf8ArrayValue)
                    {
                        writer.WriteLiteral(utf8ArrayValue.AsSpan());
                    }
                    else
                    {
                        writer.WriteValue(value);
                    }

                    break;
                case BlockType.None:
                case BlockType.End:
                default:
                    throw new UnreachableException();
            }
        }
    }
}
