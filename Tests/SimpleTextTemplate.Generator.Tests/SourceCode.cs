using System.Text;
using Microsoft.CodeAnalysis.CSharp;

namespace SimpleTextTemplate.Generator.Tests;

/// <summary>
/// ソースコードを取得するクラスです。
/// </summary>
static class SourceCode
{
    /// <summary>
    /// ソースコードを取得します。
    /// </summary>
    /// <param name="templateText">テンプレート文字列</param>
    /// <param name="context">コンテキスト名</param>
    /// <param name="provider">カルチャー指定</param>
    /// <returns>ソースコードを返します。</returns>
    public static string Get(string? templateText, string? context = null, string? provider = null)
        => Get([templateText], context, provider);

    /// <summary>
    /// ソースコードを取得します。
    /// </summary>
    /// <param name="templateTextList">テンプレート文字列のリスト</param>
    /// <param name="context">コンテキスト名</param>
    /// <param name="provider">カルチャー指定</param>
    /// <returns>ソースコードを返します。</returns>
    public static string Get(string?[] templateTextList, string? context = null, string? provider = null)
    {
        var builder = new StringBuilder();
        builder.AppendLine(value: """
            using System.Buffers;
            using SimpleTextTemplate;
            
            namespace MyCode;
            
            public class Program
            {
                public static void Main()
                {
                    var bufferWriter = new ArrayBufferWriter<byte>();
                    var writer = TemplateWriter.Create(bufferWriter);
            """);

        if (context is not null)
        {
            builder.AppendLine(value: $"        var context = new SimpleTextTemplate.Generator.Tests.Core.{context}();");
        }

        foreach (var templateText in templateTextList)
        {
            var source = GetLiteralText(templateText);

            if (context is null)
            {
                builder.AppendLine(value: $"        TemplateRenderer.Render(ref writer, {source});");
                continue;
            }

            if (provider is null)
            {
                builder.AppendLine(value: $"        TemplateRenderer.Render(ref writer, {source}, in context);");
                continue;
            }

            builder.AppendLine(value: $"        TemplateRenderer.Render(ref writer, {source}, in context, {provider});");
        }

        builder.AppendLine("""
                }
            }
            """);

        return builder.ToString();
    }

    static string GetLiteralText(string? value)
        => value is null ? "null" : SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(value)).ToFullString();
}
