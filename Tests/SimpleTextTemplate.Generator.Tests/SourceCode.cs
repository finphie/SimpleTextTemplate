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
    /// <param name="templateTextList">テンプレート文字列のリスト</param>
    /// <param name="context">コンテキスト名</param>
    /// <returns>ソースコードを返します。</returns>
    public static string Get(string[] templateTextList, string? context = null)
    {
        var builder = new StringBuilder();
        builder.AppendLine(value: $$"""
            using System.Buffers;
            using SimpleTextTemplate;
            using SimpleTextTemplate.Generator.Tests.Core;
            
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
            builder.AppendLine(value: $"""        var context = new {context}();""");
        }

        foreach (var templateText in templateTextList)
        {
            var source = GetLiteralText(templateText);

            if (context is null)
            {
                builder.AppendLine(value: $"""        writer.Write({source});""");
                continue;
            }

            builder.AppendLine(value: $"""        writer.Write({source}, in context);""");
        }

        builder.AppendLine($$"""
                }
            }
            """);

        return builder.ToString();
    }

    static string GetLiteralText(string? value)
        => value is null ? "null" : SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(value)).ToFullString();
}
