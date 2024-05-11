using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using FluentAssertions;
using Xunit;

namespace SimpleTextTemplate.Generator.Execute.Tests;

public sealed class TemplateWriterWriteTest
{
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1122:Use string.Empty for empty strings", Justification = "テスト")]
    [Fact]
    public void 空白_出力なし()
    {
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = TemplateWriter.Create(bufferWriter))
        {
            writer.Write("");
            writer.Write(string.Empty);
        }

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be(string.Empty);
    }

    [Fact]
    public void 短い文字列_そのまま出力()
    {
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = TemplateWriter.Create(bufferWriter))
        {
            writer.Write("A");
        }

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("A");
    }

    [Fact]
    public void 長い文字列_そのまま出力()
    {
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = TemplateWriter.Create(bufferWriter))
        {
            writer.Write("""
                Minim eos vel labore eos consectetuer invidunt diam labore. Accumsan eirmod dolore kasd sed laoreet sadipscing consetetur est rebum dolore lorem. Accumsan vulputate laoreet enim iusto amet dolore ut tempor stet gubergren lorem no in facilisis justo sit. Augue ut eirmod elit ut. Ut clita at ea mazim consetetur. Iusto ad at takimata consectetuer amet justo amet ullamcorper id. Sanctus quod facer nonummy justo tempor. At ex justo velit aliquip sadipscing diam lorem lorem erat ullamcorper sea tation stet consetetur labore tempor. Labore nulla dolore erat. Sadipscing lorem et takimata clita kasd sed.
                """);
        }

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("""
            Minim eos vel labore eos consectetuer invidunt diam labore. Accumsan eirmod dolore kasd sed laoreet sadipscing consetetur est rebum dolore lorem. Accumsan vulputate laoreet enim iusto amet dolore ut tempor stet gubergren lorem no in facilisis justo sit. Augue ut eirmod elit ut. Ut clita at ea mazim consetetur. Iusto ad at takimata consectetuer amet justo amet ullamcorper id. Sanctus quod facer nonummy justo tempor. At ex justo velit aliquip sadipscing diam lorem lorem erat ullamcorper sea tation stet consetetur labore tempor. Labore nulla dolore erat. Sadipscing lorem et takimata clita kasd sed.
            """);
    }

    [Fact]
    public void 複数のWrite_そのまま出力()
    {
        var bufferWriter = new ArrayBufferWriter<byte>();

        using (var writer = TemplateWriter.Create(bufferWriter))
        {
            writer.Write("""
                Minim eos vel labore eos consectetuer invidunt diam labore. Accumsan eirmod dolore kasd sed laoreet sadipscing consetetur est rebum dolore lorem. Accumsan vulputate laoreet enim iusto amet dolore ut tempor stet gubergren lorem no in facilisis justo sit. Augue ut eirmod elit ut. Ut clita at ea mazim consetetur. Iusto ad at takimata consectetuer amet justo amet ullamcorper id. Sanctus quod facer nonummy justo tempor. At ex justo velit aliquip sadipscing diam lorem lorem erat ullamcorper sea tation stet consetetur labore tempor. Labore nulla dolore erat. Sadipscing lorem et takimata clita kasd sed.

                """);
            writer.Write("""
                Minim eos vel labore eos consectetuer invidunt diam labore. Accumsan eirmod dolore kasd sed laoreet sadipscing consetetur est rebum dolore lorem. Accumsan vulputate laoreet enim iusto amet dolore ut tempor stet gubergren lorem no in facilisis justo sit. Augue ut eirmod elit ut. Ut clita at ea mazim consetetur. Iusto ad at takimata consectetuer amet justo amet ullamcorper id. Sanctus quod facer nonummy justo tempor. At ex justo velit aliquip sadipscing diam lorem lorem erat ullamcorper sea tation stet consetetur labore tempor. Labore nulla dolore erat. Sadipscing lorem et takimata clita kasd sed.

                """);
            writer.Write("""
                Minim eos vel labore eos consectetuer invidunt diam labore. Accumsan eirmod dolore kasd sed laoreet sadipscing consetetur est rebum dolore lorem. Accumsan vulputate laoreet enim iusto amet dolore ut tempor stet gubergren lorem no in facilisis justo sit. Augue ut eirmod elit ut. Ut clita at ea mazim consetetur. Iusto ad at takimata consectetuer amet justo amet ullamcorper id. Sanctus quod facer nonummy justo tempor. At ex justo velit aliquip sadipscing diam lorem lorem erat ullamcorper sea tation stet consetetur labore tempor. Labore nulla dolore erat. Sadipscing lorem et takimata clita kasd sed.

                """);
            writer.Write("""
                Minim eos vel labore eos consectetuer invidunt diam labore. Accumsan eirmod dolore kasd sed laoreet sadipscing consetetur est rebum dolore lorem. Accumsan vulputate laoreet enim iusto amet dolore ut tempor stet gubergren lorem no in facilisis justo sit. Augue ut eirmod elit ut. Ut clita at ea mazim consetetur. Iusto ad at takimata consectetuer amet justo amet ullamcorper id. Sanctus quod facer nonummy justo tempor. At ex justo velit aliquip sadipscing diam lorem lorem erat ullamcorper sea tation stet consetetur labore tempor. Labore nulla dolore erat. Sadipscing lorem et takimata clita kasd sed.

                """);
            writer.Write("""
                Minim eos vel labore eos consectetuer invidunt diam labore. Accumsan eirmod dolore kasd sed laoreet sadipscing consetetur est rebum dolore lorem. Accumsan vulputate laoreet enim iusto amet dolore ut tempor stet gubergren lorem no in facilisis justo sit. Augue ut eirmod elit ut. Ut clita at ea mazim consetetur. Iusto ad at takimata consectetuer amet justo amet ullamcorper id. Sanctus quod facer nonummy justo tempor. At ex justo velit aliquip sadipscing diam lorem lorem erat ullamcorper sea tation stet consetetur labore tempor. Labore nulla dolore erat. Sadipscing lorem et takimata clita kasd sed.
                """);
        }

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("""
            Minim eos vel labore eos consectetuer invidunt diam labore. Accumsan eirmod dolore kasd sed laoreet sadipscing consetetur est rebum dolore lorem. Accumsan vulputate laoreet enim iusto amet dolore ut tempor stet gubergren lorem no in facilisis justo sit. Augue ut eirmod elit ut. Ut clita at ea mazim consetetur. Iusto ad at takimata consectetuer amet justo amet ullamcorper id. Sanctus quod facer nonummy justo tempor. At ex justo velit aliquip sadipscing diam lorem lorem erat ullamcorper sea tation stet consetetur labore tempor. Labore nulla dolore erat. Sadipscing lorem et takimata clita kasd sed.
            Minim eos vel labore eos consectetuer invidunt diam labore. Accumsan eirmod dolore kasd sed laoreet sadipscing consetetur est rebum dolore lorem. Accumsan vulputate laoreet enim iusto amet dolore ut tempor stet gubergren lorem no in facilisis justo sit. Augue ut eirmod elit ut. Ut clita at ea mazim consetetur. Iusto ad at takimata consectetuer amet justo amet ullamcorper id. Sanctus quod facer nonummy justo tempor. At ex justo velit aliquip sadipscing diam lorem lorem erat ullamcorper sea tation stet consetetur labore tempor. Labore nulla dolore erat. Sadipscing lorem et takimata clita kasd sed.
            Minim eos vel labore eos consectetuer invidunt diam labore. Accumsan eirmod dolore kasd sed laoreet sadipscing consetetur est rebum dolore lorem. Accumsan vulputate laoreet enim iusto amet dolore ut tempor stet gubergren lorem no in facilisis justo sit. Augue ut eirmod elit ut. Ut clita at ea mazim consetetur. Iusto ad at takimata consectetuer amet justo amet ullamcorper id. Sanctus quod facer nonummy justo tempor. At ex justo velit aliquip sadipscing diam lorem lorem erat ullamcorper sea tation stet consetetur labore tempor. Labore nulla dolore erat. Sadipscing lorem et takimata clita kasd sed.
            Minim eos vel labore eos consectetuer invidunt diam labore. Accumsan eirmod dolore kasd sed laoreet sadipscing consetetur est rebum dolore lorem. Accumsan vulputate laoreet enim iusto amet dolore ut tempor stet gubergren lorem no in facilisis justo sit. Augue ut eirmod elit ut. Ut clita at ea mazim consetetur. Iusto ad at takimata consectetuer amet justo amet ullamcorper id. Sanctus quod facer nonummy justo tempor. At ex justo velit aliquip sadipscing diam lorem lorem erat ullamcorper sea tation stet consetetur labore tempor. Labore nulla dolore erat. Sadipscing lorem et takimata clita kasd sed.
            Minim eos vel labore eos consectetuer invidunt diam labore. Accumsan eirmod dolore kasd sed laoreet sadipscing consetetur est rebum dolore lorem. Accumsan vulputate laoreet enim iusto amet dolore ut tempor stet gubergren lorem no in facilisis justo sit. Augue ut eirmod elit ut. Ut clita at ea mazim consetetur. Iusto ad at takimata consectetuer amet justo amet ullamcorper id. Sanctus quod facer nonummy justo tempor. At ex justo velit aliquip sadipscing diam lorem lorem erat ullamcorper sea tation stet consetetur labore tempor. Labore nulla dolore erat. Sadipscing lorem et takimata clita kasd sed.
            """);
    }
}
