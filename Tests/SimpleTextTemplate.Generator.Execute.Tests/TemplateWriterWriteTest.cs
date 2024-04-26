using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using FluentAssertions;
using Xunit;

namespace SimpleTextTemplate.Generator.Execute.Tests;

public sealed class TemplateWriterWriteTest
{
    public enum TestData
    {
        _EnumStaticField,
        _EnumStaticProperty,
        _EnumField,
        _EnumProperty
    }

    [Fact]
    public void Writeメソッドを1回だけ呼び出す()
    {
        var context = new TestContext();
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = TemplateWriter.Create(bufferWriter))
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
            _StringConstantField
            _BytesStaticField
            _CharsStaticField
            _StringStaticField
            _EnumStaticField
            01/01/2000 00:00:00 +09:00
            _BytesField
            _CharsField
            _StringField
            _EnumField
            01/02/2000 00:00:00 +09:00
            _BytesSpanStaticProperty
            _BytesStaticProperty
            _CharsSpanStaticProperty
            _CharsStaticProperty
            _StringStaticProperty
            _EnumStaticProperty
            01/03/2000 00:00:00 +09:00
            _BytesSpanProperty
            _BytesProperty
            _CharsSpanProperty
            _CharsProperty
            _StringProperty
            _EnumProperty
            01/04/2000 00:00:00 +09:00
            """);
    }

    [Fact]
    public void Writeメソッドを複数回呼び出す()
    {
        var context = new TestContext();
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = TemplateWriter.Create(bufferWriter))
        {
            writer.Write("{{ StringConstantField }}", context);
            writer.Write("{{ BytesStaticField }}", context);
        }

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("_StringConstantField_BytesStaticField");
    }

    [Fact]
    public void Format指定()
    {
        var context = new TestContext();
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = TemplateWriter.Create(bufferWriter))
        {
            writer.Write(
                """
                {{ Iso8601 }}
                {{ EnumValue }}
                """,
                context);
        }

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("""
            2000-01-01T00:00:00.0000000+09:00
            0
            """);
    }

    [SuppressMessage("Design", "CA1051:参照可能なインスタンス フィールドを宣言しません", Justification = "テストに必要")]
    [SuppressMessage("Performance", "CA1802:適切な場所にリテラルを使用します", Justification = "テストに必要")]
    [SuppressMessage("Performance", "CA1819:プロパティは配列を返すことはできません", Justification = "テストに必要")]
    [SuppressMessage("Performance", "CA1822:メンバーを static に設定します", Justification = "テストに必要")]
    [SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1516:Elements should be separated by blank line", Justification = "テスト")]
    internal sealed record TestContext()
    {
        public const string StringConstantField = "_StringConstantField";

        [Identifier("o")]
        public static readonly DateTimeOffset Iso8601 = new(2000, 1, 1, 0, 0, 0, TimeSpan.FromHours(9));

        [Identifier("d")]
        public static readonly TestData EnumValue = TestData._EnumStaticField;

        public static readonly byte[] BytesStaticField = "_BytesStaticField"u8.ToArray();
        public static readonly char[] CharsStaticField = [.. "_CharsStaticField"];
        public static readonly string StringStaticField = "_StringStaticField";
        public static readonly TestData EnumStaticField = TestData._EnumStaticField;
        public static readonly DateTimeOffset DateTimeOffsetStaticField = new(2000, 1, 1, 0, 0, 0, TimeSpan.FromHours(9));

        public readonly byte[] BytesField = "_BytesField"u8.ToArray();
        public readonly char[] CharsField = [.. "_CharsField"];
        public readonly string StringField = "_StringField";
        public readonly TestData EnumField = TestData._EnumField;
        public readonly DateTimeOffset DateTimeOffsetField = new(2000, 1, 2, 0, 0, 0, TimeSpan.FromHours(9));

        public static ReadOnlySpan<byte> BytesSpanStaticProperty => "_BytesSpanStaticProperty"u8;
        public static byte[] BytesStaticProperty => "_BytesStaticProperty"u8.ToArray();
        public static ReadOnlySpan<char> CharsSpanStaticProperty => "_CharsSpanStaticProperty";
        public static char[] CharsStaticProperty => [.. "_CharsStaticProperty"];
        public static string StringStaticProperty => "_StringStaticProperty";
        public static TestData EnumStaticProperty => TestData._EnumStaticProperty;
        public static DateTimeOffset DateTimeOffsetStaticProperty => new(2000, 1, 3, 0, 0, 0, TimeSpan.FromHours(9));

        public ReadOnlySpan<byte> BytesSpanProperty => "_BytesSpanProperty"u8;
        public byte[] BytesProperty => "_BytesProperty"u8.ToArray();
        public ReadOnlySpan<char> CharsSpanProperty => "_CharsSpanProperty";
        public char[] CharsProperty => [.. "_CharsProperty"];
        public string StringProperty => "_StringProperty";
        public TestData EnumProperty => TestData._EnumProperty;
        public DateTimeOffset DateTimeOffsetProperty => new(2000, 1, 4, 0, 0, 0, TimeSpan.FromHours(9));
    }
}
