using System.Globalization;
using SimpleTextTemplate.Generator.Tests.Extensions;

namespace SimpleTextTemplate.Generator.Tests;

static class Constants
{
    public const string DefaultKeyword = "default";
    public const string ProviderArgument = "provider";

    public const string NullCulture = "null";
    public const string DefaultCulture = "default";
    public const string InvariantCulture = "System.Globalization.CultureInfo.InvariantCulture";
    public const string GlobalInvariantCulture = $"global::{InvariantCulture}";

    public const string Utf8GetMaxByteCount = "global::System.Text.Encoding.UTF8.GetMaxByteCount";

    public const string WriteConstantLiteral = nameof(TemplateWriter<>.WriteConstantLiteral);
    public const string DangerousWriteConstantLiteral = nameof(TemplateWriter<>.DangerousWriteConstantLiteral);
    public const string WriteLiteral = nameof(TemplateWriter<>.WriteLiteral);
    public const string DangerousWriteLiteral = nameof(TemplateWriter<>.DangerousWriteLiteral);
    public const string WriteString = nameof(TemplateWriter<>.WriteString);
    public const string DangerousWriteString = nameof(TemplateWriter<>.DangerousWriteString);
    public const string WriteEnum = nameof(TemplateWriter<>.WriteEnum);
    public const string WriteValue = nameof(TemplateWriter<>.WriteValue);

    public const string Grow = nameof(TemplateWriter<>.Grow);

    public static readonly string InvariantInfo = "System.Globalization." + DateTimeFormatInfo.InvariantInfo.ToExpressionString();
    public static readonly string JaJpCulture = "System.Globalization." + CultureInfo.GetCultureInfo("ja-JP").ToExpressionString();
    public static readonly IReadOnlyList<string> InvariantCultureList = [InvariantCulture, NullCulture, DefaultCulture];

    public static string GetContextArgumentString<T>(string memberName, bool isStatic)
    {
        if (isStatic)
        {
            var typeName = typeof(T).FullName;
            return $"global::{typeName}.@{memberName}";
        }

        return $"global::System.Runtime.CompilerServices.Unsafe.AsRef(in context).@{memberName}";
    }
}
