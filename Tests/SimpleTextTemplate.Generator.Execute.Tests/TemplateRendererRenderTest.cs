using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using FluentAssertions;
using Xunit;

namespace SimpleTextTemplate.Generator.Execute.Tests;

public sealed class TemplateRendererRenderTest
{
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1122:Use string.Empty for empty strings", Justification = "テスト")]
    [Fact]
    public void 空白_出力なし()
    {
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, "");
        TemplateRenderer.Render(ref writer, string.Empty);
        writer.Flush();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be(string.Empty);
    }

    [Fact]
    public void 短い文字列_そのまま出力()
    {
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, "A");
        writer.Flush();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("A");
    }

    [Fact]
    public void 長い文字列_そのまま出力()
    {
        const string Text = """
            Minim eos vel labore eos consectetuer invidunt diam labore. Accumsan eirmod dolore kasd sed laoreet sadipscing consetetur est rebum dolore lorem. Accumsan vulputate laoreet enim iusto amet dolore ut tempor stet gubergren lorem no in facilisis justo sit. Augue ut eirmod elit ut. Ut clita at ea mazim consetetur. Iusto ad at takimata consectetuer amet justo amet ullamcorper id. Sanctus quod facer nonummy justo tempor. At ex justo velit aliquip sadipscing diam lorem lorem erat ullamcorper sea tation stet consetetur labore tempor. Labore nulla dolore erat. Sadipscing lorem et takimata clita kasd sed.
            """;
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, Text);
        writer.Flush();

        Encoding.UTF8.GetString(bufferWriter.WrittenSpan)
            .Should()
            .Be("""
            Minim eos vel labore eos consectetuer invidunt diam labore. Accumsan eirmod dolore kasd sed laoreet sadipscing consetetur est rebum dolore lorem. Accumsan vulputate laoreet enim iusto amet dolore ut tempor stet gubergren lorem no in facilisis justo sit. Augue ut eirmod elit ut. Ut clita at ea mazim consetetur. Iusto ad at takimata consectetuer amet justo amet ullamcorper id. Sanctus quod facer nonummy justo tempor. At ex justo velit aliquip sadipscing diam lorem lorem erat ullamcorper sea tation stet consetetur labore tempor. Labore nulla dolore erat. Sadipscing lorem et takimata clita kasd sed.
            """);
    }

    [Fact]
    public void 複数のWrite_そのまま出力()
    {
        const string Text = """
            Minim eos vel labore eos consectetuer invidunt diam labore. Accumsan eirmod dolore kasd sed laoreet sadipscing consetetur est rebum dolore lorem. Accumsan vulputate laoreet enim iusto amet dolore ut tempor stet gubergren lorem no in facilisis justo sit. Augue ut eirmod elit ut. Ut clita at ea mazim consetetur. Iusto ad at takimata consectetuer amet justo amet ullamcorper id. Sanctus quod facer nonummy justo tempor. At ex justo velit aliquip sadipscing diam lorem lorem erat ullamcorper sea tation stet consetetur labore tempor. Labore nulla dolore erat. Sadipscing lorem et takimata clita kasd sed.

            """;
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, Text);
        TemplateRenderer.Render(ref writer, Text);
        TemplateRenderer.Render(ref writer, Text);
        TemplateRenderer.Render(ref writer, Text);
        TemplateRenderer.Render(ref writer, Text);
        writer.Flush();

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
