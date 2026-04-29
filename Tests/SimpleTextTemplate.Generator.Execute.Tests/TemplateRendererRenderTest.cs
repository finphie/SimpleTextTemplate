using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using SimpleTextTemplate.Generator.Tests.TestData;
using SimpleTextTemplate.Tests.Assertions;
using SimpleTextTemplate.Tests.Buffers;

namespace SimpleTextTemplate.Generator.Execute.Tests;

public sealed class TemplateRendererRenderTest
{
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1122:Use string.Empty for empty strings")]
    [Test]
    public async Task 空白_出力なし()
    {
        var bufferWriter = new ArrayBufferWriter<byte>();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, "");
        TemplateRenderer.Render(ref writer, string.Empty);
        writer.Flush();

        await Assert.That(bufferWriter.WrittenMemory)
            .IsEmpty();
    }

    [Test]
    public async Task 短い文字列_そのまま出力()
    {
        var bufferWriter = new ExactSizeBufferWriter();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, "A");
        writer.Flush();

        await Assert.That(bufferWriter.WrittenMemory)
            .IsUtf8SequenceEqualTo("A");
    }

    [Test]
    public async Task 長い文字列_そのまま出力()
    {
        const string Text = """
            Minim eos vel labore eos consectetuer invidunt diam labore. Accumsan eirmod dolore kasd sed laoreet sadipscing consetetur est rebum dolore lorem. Accumsan vulputate laoreet enim iusto amet dolore ut tempor stet gubergren lorem no in facilisis justo sit. Augue ut eirmod elit ut. Ut clita at ea mazim consetetur. Iusto ad at takimata consectetuer amet justo amet ullamcorper id. Sanctus quod facer nonummy justo tempor. At ex justo velit aliquip sadipscing diam lorem lorem erat ullamcorper sea tation stet consetetur labore tempor. Labore nulla dolore erat. Sadipscing lorem et takimata clita kasd sed.
            """;
        var bufferWriter = new ExactSizeBufferWriter();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, Text);
        writer.Flush();

        await Assert.That(bufferWriter.WrittenMemory)
            .IsUtf8SequenceEqualTo("""
                Minim eos vel labore eos consectetuer invidunt diam labore. Accumsan eirmod dolore kasd sed laoreet sadipscing consetetur est rebum dolore lorem. Accumsan vulputate laoreet enim iusto amet dolore ut tempor stet gubergren lorem no in facilisis justo sit. Augue ut eirmod elit ut. Ut clita at ea mazim consetetur. Iusto ad at takimata consectetuer amet justo amet ullamcorper id. Sanctus quod facer nonummy justo tempor. At ex justo velit aliquip sadipscing diam lorem lorem erat ullamcorper sea tation stet consetetur labore tempor. Labore nulla dolore erat. Sadipscing lorem et takimata clita kasd sed.
                """);
    }

    [Test]
    public async Task 複数のWrite_そのまま出力()
    {
        const string Text = """
            Minim eos vel labore eos consectetuer invidunt diam labore. Accumsan eirmod dolore kasd sed laoreet sadipscing consetetur est rebum dolore lorem. Accumsan vulputate laoreet enim iusto amet dolore ut tempor stet gubergren lorem no in facilisis justo sit. Augue ut eirmod elit ut. Ut clita at ea mazim consetetur. Iusto ad at takimata consectetuer amet justo amet ullamcorper id. Sanctus quod facer nonummy justo tempor. At ex justo velit aliquip sadipscing diam lorem lorem erat ullamcorper sea tation stet consetetur labore tempor. Labore nulla dolore erat. Sadipscing lorem et takimata clita kasd sed.

            """;
        var bufferWriter = new ExactSizeBufferWriter();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, Text);
        TemplateRenderer.Render(ref writer, Text);
        TemplateRenderer.Render(ref writer, Text);
        TemplateRenderer.Render(ref writer, Text);
        TemplateRenderer.Render(ref writer, Text);
        writer.Flush();

        await Assert.That(bufferWriter.WrittenMemory)
            .IsUtf8SequenceEqualTo("""
                Minim eos vel labore eos consectetuer invidunt diam labore. Accumsan eirmod dolore kasd sed laoreet sadipscing consetetur est rebum dolore lorem. Accumsan vulputate laoreet enim iusto amet dolore ut tempor stet gubergren lorem no in facilisis justo sit. Augue ut eirmod elit ut. Ut clita at ea mazim consetetur. Iusto ad at takimata consectetuer amet justo amet ullamcorper id. Sanctus quod facer nonummy justo tempor. At ex justo velit aliquip sadipscing diam lorem lorem erat ullamcorper sea tation stet consetetur labore tempor. Labore nulla dolore erat. Sadipscing lorem et takimata clita kasd sed.
                Minim eos vel labore eos consectetuer invidunt diam labore. Accumsan eirmod dolore kasd sed laoreet sadipscing consetetur est rebum dolore lorem. Accumsan vulputate laoreet enim iusto amet dolore ut tempor stet gubergren lorem no in facilisis justo sit. Augue ut eirmod elit ut. Ut clita at ea mazim consetetur. Iusto ad at takimata consectetuer amet justo amet ullamcorper id. Sanctus quod facer nonummy justo tempor. At ex justo velit aliquip sadipscing diam lorem lorem erat ullamcorper sea tation stet consetetur labore tempor. Labore nulla dolore erat. Sadipscing lorem et takimata clita kasd sed.
                Minim eos vel labore eos consectetuer invidunt diam labore. Accumsan eirmod dolore kasd sed laoreet sadipscing consetetur est rebum dolore lorem. Accumsan vulputate laoreet enim iusto amet dolore ut tempor stet gubergren lorem no in facilisis justo sit. Augue ut eirmod elit ut. Ut clita at ea mazim consetetur. Iusto ad at takimata consectetuer amet justo amet ullamcorper id. Sanctus quod facer nonummy justo tempor. At ex justo velit aliquip sadipscing diam lorem lorem erat ullamcorper sea tation stet consetetur labore tempor. Labore nulla dolore erat. Sadipscing lorem et takimata clita kasd sed.
                Minim eos vel labore eos consectetuer invidunt diam labore. Accumsan eirmod dolore kasd sed laoreet sadipscing consetetur est rebum dolore lorem. Accumsan vulputate laoreet enim iusto amet dolore ut tempor stet gubergren lorem no in facilisis justo sit. Augue ut eirmod elit ut. Ut clita at ea mazim consetetur. Iusto ad at takimata consectetuer amet justo amet ullamcorper id. Sanctus quod facer nonummy justo tempor. At ex justo velit aliquip sadipscing diam lorem lorem erat ullamcorper sea tation stet consetetur labore tempor. Labore nulla dolore erat. Sadipscing lorem et takimata clita kasd sed.
                Minim eos vel labore eos consectetuer invidunt diam labore. Accumsan eirmod dolore kasd sed laoreet sadipscing consetetur est rebum dolore lorem. Accumsan vulputate laoreet enim iusto amet dolore ut tempor stet gubergren lorem no in facilisis justo sit. Augue ut eirmod elit ut. Ut clita at ea mazim consetetur. Iusto ad at takimata consectetuer amet justo amet ullamcorper id. Sanctus quod facer nonummy justo tempor. At ex justo velit aliquip sadipscing diam lorem lorem erat ullamcorper sea tation stet consetetur labore tempor. Labore nulla dolore erat. Sadipscing lorem et takimata clita kasd sed.

                """);
    }

    [Test]
    public async Task 複雑なテンプレート文字列()
    {
        const string Text = """
            A{{ ConstantValue }}{{ ConstantValue }}B{{ ConstantValue }}{{ StringValue }}{{ ConstantValue }}{{ ConstantValue }}{{ Utf16Value }}{{ ConstantValue }}{{ Utf8Value }}{{ DoubleValue }}
            A{{ ConstantValue }}{{ ConstantValue }}B{{ ConstantValue }}{{ StringValue }}{{ ConstantValue }}{{ ConstantValue }}{{ Utf16Value }}{{ ConstantValue }}{{ Utf8Value }}

            """;
        var bufferWriter = new ExactSizeBufferWriter();
        var context = new ContextTestData();

        var writer = TemplateWriter.Create(bufferWriter);
        TemplateRenderer.Render(ref writer, Text, context);
        TemplateRenderer.Render(ref writer, Text, context);
        writer.Flush();

        await Assert.That(bufferWriter.WrittenMemory)
            .IsUtf8SequenceEqualTo("""
                A_ConstantValue_ConstantValueB_ConstantValue_StringValue_ConstantValue_ConstantValue_Utf16Value_ConstantValue_Utf8Value1
                A_ConstantValue_ConstantValueB_ConstantValue_StringValue_ConstantValue_ConstantValue_Utf16Value_ConstantValue_Utf8Value
                A_ConstantValue_ConstantValueB_ConstantValue_StringValue_ConstantValue_ConstantValue_Utf16Value_ConstantValue_Utf8Value1
                A_ConstantValue_ConstantValueB_ConstantValue_StringValue_ConstantValue_ConstantValue_Utf16Value_ConstantValue_Utf8Value

                """);
    }
}
