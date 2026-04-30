using System.Text;
using Microsoft.CodeAnalysis.CSharp;

namespace SimpleTextTemplate.Generator.Tests;

/// <summary>
/// ソースコードを取得するクラスです。
/// </summary>
static class SourceCode
{
    public static string Get(params ReadOnlySpan<string?> templateTextList)
        => GetInternal(templateTextList, null, null);

    public static string Get<T>(params ReadOnlySpan<string?> templateTextList)
        => GetInternal(templateTextList, typeof(T).Name, null);

    public static string GetWithCulture<T>(string? provider, params ReadOnlySpan<string?> templateTextList)
        => GetInternal(templateTextList, typeof(T).Name, provider);

    static string GetInternal(ReadOnlySpan<string?> templateTextList, string? context = null, string? provider = null)
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
            builder.AppendLine(value: $"        var context = new SimpleTextTemplate.Tests.TestData.{context}();");
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
