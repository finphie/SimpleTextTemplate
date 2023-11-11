using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using FluentAssertions;
using Xunit;

namespace SimpleTextTemplate.Generator.Tests;

public sealed class TemplateWriterWriteTest
{
    public enum TestData
    {
        EnumStaticField,
        EnumStaticProperty,
        EnumField,
        EnumProperty
    }

    [Fact]
    public void Writeメソッドを1回だけ呼び出す()
    {
        var context = new TestContext();
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = new TemplateWriter<ArrayBufferWriter<byte>>(ref bufferWriter))
        {
            writer.Write(
                """
                {{ StringConstantField }}
                {{ BytesStaticField }}
                {{ CharsStaticField }}
                {{ StringStaticField }}
                {{ EnumStaticField }}
                {{ DateTimeOffsetStaticField }}
                {{ BytesField }}
                {{ CharsField }}
                {{ StringField }}
                {{ EnumField }}
                {{ DateTimeOffsetField }}
                {{ BytesSpanStaticProperty }}
                {{ BytesStaticProperty }}
                {{ CharsSpanStaticProperty }}
                {{ CharsStaticProperty }}
                {{ StringStaticProperty }}
                {{ EnumStaticProperty }}
                {{ DateTimeOffsetStaticProperty }}
                {{ BytesSpanProperty }}
                {{ BytesProperty }}
                {{ CharsSpanProperty }}
                {{ CharsProperty }}
                {{ StringProperty }}
                {{ EnumProperty }}
                {{ DateTimeOffsetProperty }}
                """,
                context);
        }

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("""
            StringConstantField
            BytesStaticField
            CharsStaticField
            StringStaticField
            EnumStaticField
            01/01/2000 00:00:00 +09:00
            BytesField
            CharsField
            StringField
            EnumField
            01/02/2000 00:00:00 +09:00
            BytesSpanStaticProperty
            BytesStaticProperty
            CharsSpanStaticProperty
            CharsStaticProperty
            StringStaticProperty
            EnumStaticProperty
            01/03/2000 00:00:00 +09:00
            BytesSpanProperty
            BytesProperty
            CharsSpanProperty
            CharsProperty
            StringProperty
            EnumProperty
            01/04/2000 00:00:00 +09:00
            """);
    }

    [Fact]
    public void Writeメソッドを複数回呼び出す()
    {
        var context = new TestContext();
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = new TemplateWriter<ArrayBufferWriter<byte>>(ref bufferWriter))
        {
            writer.Write("{{ StringConstantField }}", context);
            writer.Write("{{ BytesStaticField }}", context);
        }

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("StringConstantFieldBytesStaticField");
    }

    [SuppressMessage("Design", "CA1051:参照可能なインスタンス フィールドを宣言しません", Justification = "テストに必要")]
    [SuppressMessage("Performance", "CA1802:適切な場所にリテラルを使用します", Justification = "テストに必要")]
    [SuppressMessage("Performance", "CA1819:プロパティは配列を返すことはできません", Justification = "テストに必要")]
    [SuppressMessage("Performance", "CA1822:メンバーを static に設定します", Justification = "テストに必要")]
    internal sealed record TestContext()
    {
        public const string StringConstantField = nameof(StringConstantField);

        public static readonly byte[] BytesStaticField = Encoding.UTF8.GetBytes(nameof(BytesStaticField));
        public static readonly char[] CharsStaticField = nameof(CharsStaticField).ToCharArray();
        public static readonly string StringStaticField = nameof(StringStaticField);
        public static TestData EnumStaticField = TestData.EnumStaticField;
        public static DateTimeOffset DateTimeOffsetStaticField = new(2000, 1, 1, 0, 0, 0, TimeSpan.FromHours(9));

        public readonly byte[] BytesField = Encoding.UTF8.GetBytes(nameof(BytesField));
        public readonly char[] CharsField = nameof(CharsField).ToCharArray();
        public readonly string StringField = nameof(StringField);
        public readonly TestData EnumField = TestData.EnumField;
        public DateTimeOffset DateTimeOffsetField = new(2000, 1, 2, 0, 0, 0, TimeSpan.FromHours(9));

        public static ReadOnlySpan<byte> BytesSpanStaticProperty => Encoding.UTF8.GetBytes(nameof(BytesSpanStaticProperty));

        public static byte[] BytesStaticProperty => Encoding.UTF8.GetBytes(nameof(BytesStaticProperty));

        public static ReadOnlySpan<char> CharsSpanStaticProperty => nameof(CharsSpanStaticProperty).ToCharArray();

        public static char[] CharsStaticProperty => nameof(CharsStaticProperty).ToCharArray();

        public static string StringStaticProperty => nameof(StringStaticProperty);

        public static TestData EnumStaticProperty => TestData.EnumStaticProperty;

        public static DateTimeOffset DateTimeOffsetStaticProperty => new(2000, 1, 3, 0, 0, 0, TimeSpan.FromHours(9));

        public ReadOnlySpan<byte> BytesSpanProperty => Encoding.UTF8.GetBytes(nameof(BytesSpanProperty));

        public byte[] BytesProperty => Encoding.UTF8.GetBytes(nameof(BytesProperty));

        public ReadOnlySpan<char> CharsSpanProperty => nameof(CharsSpanProperty).ToCharArray();

        public char[] CharsProperty => nameof(CharsProperty).ToCharArray();

        public string StringProperty => nameof(StringProperty);

        public TestData EnumProperty => TestData.EnumProperty;

        public DateTimeOffset DateTimeOffsetProperty => new(2000, 1, 4, 0, 0, 0, TimeSpan.FromHours(9));
    }
}
