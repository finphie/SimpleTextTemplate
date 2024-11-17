using System.Buffers;
using System.Diagnostics;

namespace SimpleTextTemplate;

/// <summary>
/// テンプレート
/// </summary>
public static class TemplateRenderer
{
    /// <summary>
    /// テンプレートをレンダリングします。
    /// </summary>
    /// <typeparam name="T">バッファーライターの型</typeparam>
    /// <param name="writer">文字列などをバッファーに書き込む構造体のインスタンス</param>
    /// <param name="text">テンプレート文字列</param>
    public static void Render<T>(ref TemplateWriter<T> writer, string text)
        where T : notnull, IBufferWriter<byte>, allows ref struct
        => throw new UnreachableException();

    /// <summary>
    /// テンプレートをレンダリングします。
    /// </summary>
    /// <typeparam name="TWriter">バッファーライターの型</typeparam>
    /// <typeparam name="TContext">コンテキストの型</typeparam>
    /// <param name="writer">文字列などをバッファーに書き込む構造体のインスタンス</param>
    /// <param name="text">テンプレート文字列</param>
    /// <param name="context">コンテキスト</param>
    /// <param name="provider">カルチャー指定</param>
    public static void Render<TWriter, TContext>(ref TemplateWriter<TWriter> writer, string text, in TContext context, IFormatProvider? provider = null)
        where TWriter : notnull, IBufferWriter<byte>, allows ref struct
        where TContext : notnull, allows ref struct
        => throw new UnreachableException();
}
