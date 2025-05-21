using System.Buffers;
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

    public const string WriteConstantLiteral = nameof(TemplateWriter<IBufferWriter<byte>>.WriteConstantLiteral);
    public const string DangerousWriteConstantLiteral = nameof(TemplateWriter<IBufferWriter<byte>>.DangerousWriteConstantLiteral);
    public const string WriteLiteral = nameof(TemplateWriter<IBufferWriter<byte>>.WriteLiteral);
    public const string DangerousWriteLiteral = nameof(TemplateWriter<IBufferWriter<byte>>.DangerousWriteLiteral);
    public const string WriteString = nameof(TemplateWriter<IBufferWriter<byte>>.WriteString);
    public const string DangerousWriteString = nameof(TemplateWriter<IBufferWriter<byte>>.DangerousWriteString);
    public const string WriteEnum = nameof(TemplateWriter<IBufferWriter<byte>>.WriteEnum);
    public const string WriteValue = nameof(TemplateWriter<IBufferWriter<byte>>.WriteValue);

    public const string Grow = nameof(TemplateWriter<IBufferWriter<byte>>.Grow);

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
