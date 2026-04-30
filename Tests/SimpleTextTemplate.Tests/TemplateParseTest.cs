using System.Globalization;
using System.Text;

namespace SimpleTextTemplate.Tests;

public sealed class TemplateParseTest
{
    [Test]
    public async Task 文字列が空_Templateを返す()
    {
        var template = Template.Parse([]);

        await Assert.That(template.Blocks.Length)
            .IsZero();
    }

    [Test]
    [Arguments("{")]
    [Arguments("a")]
    [Arguments("ab")]
    [Arguments("abc")]
    [Arguments("{ A }")]
    [Arguments("}}")]
    public Task 文字列_Templateを返す(string input) => Execute(input);

    [Test]
    [Arguments("{{ A }}{{ B }}")]
    [Arguments("{{ AAA }}{{ BBB }}")]
    public Task 識別子_識別子_Templateを返す(string input) => Execute(input);

    [Test]
    [Arguments("z{{A}}z")]
    [Arguments("z{{ A }}z")]
    public Task 文字列_識別子_文字列_Templateを返す(string input) => Execute(input);

    [Test]
    [Arguments("{{ AAA }}z{{ BBB }}")]
    [Arguments("{{ A }}123{{ B }}")]
    public Task 識別子_文字列_識別子_Templateを返す(string input) => Execute(input);

    [Test]
    [Arguments("x{{ A }}123{{ B }}x")]
    public Task 文字列_識別子_文字列_識別子_文字列_Templateを返す(string input) => Execute(input);

    [Test]
    [Arguments("{{ 1:N }}")]
    public Task 書式指定あり_Templateを返す(string input) => Execute(input);

    [Test]
    [Arguments("{{ 1::ja-JP }}")]
    [Arguments("{{ 1:N:ja-JP }}")]
    public Task カルチャー指定あり_Templateを返す(string input) => Execute(input);

    [Test]
    [Arguments("{{ A: }}")]
    [Arguments("{{ A:: }}")]
    [Arguments("{{ A:N: }}")]
    public Task 書式指定またはカルチャー指定が空_Templateを返す(string input) => Execute(input);

    [Test]
    [Arguments("{{", 2)]
    [Arguments("{{ ", 3)]
    [Arguments("{{ }", 3)]
    public async Task 識別子終了タグなし_TemplateException(string input, int position)
    {
        await Assert.That(() => Execute(input))
            .ThrowsExactly<TemplateException>()
            .And
            .Member(static x => x.Position, x => x.IsEqualTo((nuint)position));
    }

    [Test]
    [Arguments("{{ : }}")]
    [Arguments("{{ :: }}")]
    public async Task 識別子が空_TemplateException(string input)
    {
        await Assert.That(() => Execute(input))
            .ThrowsExactly<TemplateException>();
    }

    [Test]
    public async Task 無効なカルチャー_TemplateException()
    {
        await Assert.That(static () => Template.Parse("{{ A::B }}"u8))
            .ThrowsExactly<CultureNotFoundException>();
    }

    static async Task Execute(string input)
    {
        var template = Template.Parse(Encoding.UTF8.GetBytes(input));

        await Assert.That(template.Blocks.Length)
            .IsGreaterThan(0);
    }
}
