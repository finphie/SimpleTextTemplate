using System.Buffers;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using Utf8Utility;

namespace SimpleTextTemplate;

/// <summary>
/// <see cref="Template"/>構造体の拡張メソッド集です。
/// </summary>
public static class TemplateExtensions
{
    /// <summary>
    /// テンプレートをレンダリングして、バッファーライターに書き込みます。
    /// </summary>
    /// <typeparam name="TWriter">バッファーライターの型</typeparam>
    /// <typeparam name="TContext">コンテキストの型</typeparam>
    /// <param name="template">テンプレート構造</param>
    /// <param name="bufferWriter">バッファーライター</param>
    /// <param name="context">コンテキスト</param>
    /// <param name="provider">カルチャー指定</param>
    /// <exception cref="ArgumentNullException">引数がnullです。</exception>
    public static void Render<TWriter, TContext>(this in Template template, TWriter bufferWriter, TContext context, IFormatProvider? provider = null)
        where TWriter : notnull, IBufferWriter<byte>
        where TContext : notnull, IContext
    {
        ArgumentNullException.ThrowIfNull(bufferWriter);
        ArgumentNullException.ThrowIfNull(context);

        provider ??= CultureInfo.InvariantCulture;
        using var writer = TemplateWriter.Create(bufferWriter);

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
                        var cultureInfo = culture is null ? provider : culture;
                        writer.WriteValue(value, format, cultureInfo);
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
